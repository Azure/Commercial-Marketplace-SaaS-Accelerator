using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Exceptions;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.Services;

namespace Marketplace.SaaS.Accelerator.MeteredTriggerJob;

public class Executor
{

    private MeteredPlanSchedulerManagementService schedulerService;
    private ISchedulerFrequencyRepository frequencyRepository;
    private IMeteredPlanSchedulerManagementRepository schedulerRepository;
    private ISchedulerManagerViewRepository schedulerViewRepository;
    private ISubscriptionUsageLogsRepository subscriptionUsageLogsRepository;
    private readonly IMeteredBillingApiService billingApiService;
    private readonly IApplicationConfigRepository applicationConfigRepository;


    public Executor(ISchedulerFrequencyRepository frequencyRepository,
        IMeteredPlanSchedulerManagementRepository schedulerRepository,
        ISchedulerManagerViewRepository schedulerViewRepository, 
        ISubscriptionUsageLogsRepository subscriptionUsageLogsRepository, 
        IMeteredBillingApiService billingApiService,
        IApplicationConfigRepository applicationConfigRepository)
    {
        this.frequencyRepository = frequencyRepository;
        this.schedulerRepository = schedulerRepository;
        this.schedulerViewRepository = schedulerViewRepository;
        this.subscriptionUsageLogsRepository = subscriptionUsageLogsRepository;
        this.schedulerService = new MeteredPlanSchedulerManagementService(this.frequencyRepository, this.schedulerRepository, this.schedulerViewRepository, this.subscriptionUsageLogsRepository, this.applicationConfigRepository);
        this.billingApiService = billingApiService;
        this.applicationConfigRepository = applicationConfigRepository;


    }

    public void Execute()
    {
        schedulerService = new MeteredPlanSchedulerManagementService(frequencyRepository, 
            schedulerRepository, 
            schedulerViewRepository, 
            subscriptionUsageLogsRepository,applicationConfigRepository);
            

        //Get all Scheduled Data
        List<SchedulerManagerViewModel> getAllSchedulerManagerViewData = schedulerService.GetAllSchedulerManagerList();

        //GetCurrentUTC time
        DateTime _currentUTCTime = DateTime.UtcNow;
        TimeSpan ts = new TimeSpan(DateTime.UtcNow.Hour, 0, 0);
        _currentUTCTime = _currentUTCTime.Date + ts;

        //Process each scheduler frequency
        foreach (SchedulerFrequencyEnum frequency in Enum.GetValues(typeof(SchedulerFrequencyEnum)))
        {
            var ableToParse = bool.TryParse(this.applicationConfigRepository.GetValueByName($"Enable{frequency}MeterSchedules"), out bool runSchedulerForThisFrequency);

            if (ableToParse && runSchedulerForThisFrequency)
            {
                Console.WriteLine();
                Console.WriteLine($"==== Checking all {frequency} scheduled items at {_currentUTCTime} UTC. ====");

                var scheduledItems = getAllSchedulerManagerViewData
                    .Where(a => a.Frequency == frequency.ToString())
                    .ToList();

                foreach (var scheduledItem in scheduledItems)
                {
                    // Get the run time.
                    //Always pickup the NextRuntime, durnig firstRun or OneTime then pickup StartDate, as the NextRunTime will be null
                    DateTime? _nextRunTime = scheduledItem.NextRunTime ?? scheduledItem.StartDate;
                    int timeDifferentInHours = (int)_currentUTCTime.Subtract(_nextRunTime.Value).TotalHours;

                    // Print the scheduled Item and the expected run date
                    PrintScheduler(scheduledItem,
                        _nextRunTime,
                        timeDifferentInHours);

                    //Past scheduler items
                    if (timeDifferentInHours > 0)
                    {
                        Console.WriteLine($"Item Id: {scheduledItem.Id} will not run as {_nextRunTime} has passed. Please check audit logs if its has run previously.");
                        continue;
                    }
                    else if (timeDifferentInHours < 0)
                    {
                        Console.WriteLine($"Item Id: {scheduledItem.Id} future run will be at {_nextRunTime} UTC.");
                        continue;
                    }
                    else if (timeDifferentInHours == 0)
                    {
                        TriggerSchedulerItem(scheduledItem,
                            frequency,
                            billingApiService,
                            schedulerService,
                            subscriptionUsageLogsRepository);
                    }
                    else
                    {
                        Console.WriteLine($"Item Id: {scheduledItem.Id} will not run as it doesn't match any time difference logic. {_nextRunTime} UTC.");
                    }
                }
            }
            else
            {
                Console.WriteLine($"{frequency} scheduled items will not be run as it's disabled in the application config.");
            }
        }
    }

    public static void TriggerSchedulerItem(SchedulerManagerViewModel item, 
        SchedulerFrequencyEnum frequency, 
        IMeteredBillingApiService billingApiService,
        MeteredPlanSchedulerManagementService schedulerService,
        ISubscriptionUsageLogsRepository subscriptionUsageLogsRepository)
    {
        try
        {
            Console.WriteLine($"---- Item Id: {item.Id} Start Triggering meter event ----");

            var subscriptionUsageRequest = new MeteringUsageRequest()
            {
                Dimension = item.Dimension,
                EffectiveStartTime = DateTime.UtcNow,
                PlanId = item.PlanId,
                Quantity = item.Quantity,
                ResourceId = item.AMPSubscriptionId,
            };
            var meteringUsageResult = new MeteringUsageResult();
            var requestJson = JsonSerializer.Serialize(subscriptionUsageRequest);
            var responseJson = string.Empty;
            try
            {
                Console.WriteLine($"Item Id: {item.Id} Request {requestJson}");
                meteringUsageResult = billingApiService.EmitUsageEventAsync(subscriptionUsageRequest).ConfigureAwait(false).GetAwaiter().GetResult();
                responseJson = JsonSerializer.Serialize(meteringUsageResult);
                Console.WriteLine($"Item Id: {item.Id} Response {responseJson}");
            }
            catch (MarketplaceException marketplaceException)
            {
                responseJson = JsonSerializer.Serialize(marketplaceException.Message);
                meteringUsageResult.Status = marketplaceException.ErrorCode;
                Console.WriteLine($" Item Id: {item.Id} Error during EmitUsageEventAsync {responseJson}");
            }

            UpdateSchedulerItem(item,
                requestJson,
                responseJson,
                meteringUsageResult.Status,
                schedulerService,
                subscriptionUsageLogsRepository);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }
    public static void UpdateSchedulerItem(SchedulerManagerViewModel item, 
        string requestJson, 
        string responseJson, 
        string status, 
        MeteredPlanSchedulerManagementService schedulerService,
        ISubscriptionUsageLogsRepository subscriptionUsageLogsRepository)
    {
        try
        {
            Console.WriteLine($"Item Id: {item.Id} Saving Audit information");
            var scheduler = schedulerService.GetSchedulerDetailById(item.Id);
            var newMeteredAuditLog = new MeteredAuditLogs()
            {
                RequestJson = requestJson,
                ResponseJson = responseJson,
                StatusCode = status,
                RunBy = $"Scheduler - {scheduler.SchedulerName}",
                SubscriptionId = scheduler.SubscriptionId,
                SubscriptionUsageDate = DateTime.UtcNow,
                CreatedBy = 0,
                CreatedDate = DateTime.Now,
            };
            subscriptionUsageLogsRepository.Save(newMeteredAuditLog);

            if ((status == "Accepted"))
            {
                Console.WriteLine($"Item Id: {item.Id} Meter event Accepted");

                //Ignore updating NextRuntime value for OneTime frequency as they always depend on StartTime value
                Enum.TryParse(item.Frequency, out SchedulerFrequencyEnum itemFrequency);
                if (itemFrequency != SchedulerFrequencyEnum.OneTime)
                {
                    scheduler.NextRunTime = GetNextRunTime(item.NextRunTime ?? item.StartDate, itemFrequency);

                    Console.WriteLine($"Item Id: {item.Id} Updating Scheduler NextRunTime from {item.NextRunTime} to {scheduler.NextRunTime}");

                    schedulerService.UpdateSchedulerNextRunTime(scheduler);
                }
            }
            else
            {
                Console.WriteLine($"Item Id: {item.Id} failed with status {status}. NextRunTime will not be updated.");
            }
            Console.WriteLine($"Item Id: {item.Id} Complete Triggering Meter event.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static void PrintScheduler(SchedulerManagerViewModel item, 
        DateTime? nextRun, 
        int timeDifferenceInHours)
    {
        Console.WriteLine($"Item Id: {item.Id} " +
                          $"Expected NextRun : {nextRun} " +
                          $"SubId : {item.AMPSubscriptionId} " +
                          $"Plan : {item.PlanId} " +
                          $"Dim : {item.Dimension} " +
                          $"Start Date : {item.StartDate} " +
                          $"NextRun : {item.NextRunTime}" +
                          $"TimeDifferenceInHours : {timeDifferenceInHours}");
    }
    public static DateTime? GetNextRunTime(DateTime? startDate, SchedulerFrequencyEnum frequency)
    {
        switch (frequency)
        {
            case SchedulerFrequencyEnum.Hourly: { return startDate.Value.AddHours(1); }
            case SchedulerFrequencyEnum.Daily: { return startDate.Value.AddDays(1); }
            case SchedulerFrequencyEnum.Weekly: { return startDate.Value.AddDays(7); }
            case SchedulerFrequencyEnum.Monthly: { return startDate.Value.AddMonths(1); }
            case SchedulerFrequencyEnum.Yearly: { return startDate.Value.AddYears(1); }
            case SchedulerFrequencyEnum.OneTime: { return startDate; }
            default:
            { return null; }
        }
    }
}