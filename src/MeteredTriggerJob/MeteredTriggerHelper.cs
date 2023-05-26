using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Exceptions;
using Marketplace.SaaS.Accelerator.Services.Helpers;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.Services;

namespace Marketplace.SaaS.Accelerator.MeteredTriggerJob;

public class Executor
{
    /// <summary>
    /// Frequency Repository Interface
    /// </summary>
    private readonly ISchedulerFrequencyRepository frequencyRepository;
    /// <summary>
    /// Scheduler Repository Interface
    /// </summary>
    private IMeteredPlanSchedulerManagementRepository schedulerRepository;
    /// <summary>
    /// Scheduler View Repository Interface
    /// </summary>
    private readonly ISchedulerManagerViewRepository schedulerViewRepository;
    /// <summary>
    /// Subscription Usage Logs Repository Interface
    /// </summary>
    private ISubscriptionUsageLogsRepository subscriptionUsageLogsRepository;
    /// <summary>
    /// Metered Billing Api Service Interface
    /// </summary>
    private readonly IMeteredBillingApiService billingApiService;
    /// <summary>
    /// Application Config Repository Interface
    /// </summary>
    private readonly IApplicationConfigRepository applicationConfigRepository;
    /// <summary>
    /// Email Template Repository Interface
    /// </summary>
    private readonly IEmailTemplateRepository emailTemplateRepository;
    /// <summary>
    /// Email Service Interface
    /// </summary>
    private IEmailService emailService;
    /// <summary>
    ///  Metered Plan Scheduler Management Service
    /// </summary>
    private MeteredPlanSchedulerManagementService schedulerService;

    /// <summary>
    /// Initiate dependency components
    /// </summary>
    /// <param name="frequencyRepository"></param>
    /// <param name="schedulerRepository"></param>
    /// <param name="schedulerViewRepository"></param>
    /// <param name="subscriptionUsageLogsRepository"></param>
    /// <param name="billingApiService"></param>
    /// <param name="applicationConfigRepository"></param>
    /// <param name="emailService"></param>
    /// <param name="emailTemplateRepository"></param>
    public Executor(ISchedulerFrequencyRepository frequencyRepository,
        IMeteredPlanSchedulerManagementRepository schedulerRepository,
        ISchedulerManagerViewRepository schedulerViewRepository, 
        ISubscriptionUsageLogsRepository subscriptionUsageLogsRepository,
        IMeteredBillingApiService billingApiService,
        IApplicationConfigRepository applicationConfigRepository,
        IEmailService emailService,
        IEmailTemplateRepository emailTemplateRepository)
    {
        this.frequencyRepository = frequencyRepository;
        this.schedulerRepository = schedulerRepository;
        this.schedulerViewRepository = schedulerViewRepository;
        this.subscriptionUsageLogsRepository = subscriptionUsageLogsRepository;
        this.billingApiService = billingApiService;
        this.applicationConfigRepository = applicationConfigRepository;
        this.emailTemplateRepository = emailTemplateRepository;
        this.emailService = emailService;
        schedulerService = new MeteredPlanSchedulerManagementService(this.frequencyRepository, 
                               this.schedulerRepository, 
                               this.schedulerViewRepository, 
                               this.subscriptionUsageLogsRepository,
                               this.applicationConfigRepository,
                               this.emailTemplateRepository,
                               this.emailService);


        this.billingApiService = billingApiService;
    }

    /// <summary>
    /// Execute the scheduler engine
    /// </summary>
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
                        var msg = $"Item Id: {scheduledItem.Id} will not run as {_nextRunTime} has passed. Please check audit logs if its has run previously.";
                        Console.WriteLine(msg);
                        _ = bool.TryParse(applicationConfigRepository.GetValueByName("EnablesMissingSchedulerEmail"), out bool enablesMissingSchedulerEmail);
                        if (enablesMissingSchedulerEmail)
                        {
                            var newMeteredAuditLog = new MeteredAuditLogs()
                            {
                                StatusCode="Missing",
                                ResponseJson=msg
                            };
                            schedulerService.SendSchedulerEmail(scheduledItem, newMeteredAuditLog);
                        }

                        continue;
                    }
                    else if (timeDifferentInHours < 0)
                    {
                        Console.WriteLine($"Item Id: {scheduledItem.Id} future run will be at {_nextRunTime} UTC.");
                        continue;
                    }
                    else if (timeDifferentInHours == 0)
                    {
                        TriggerSchedulerItem(scheduledItem);
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
    /// <summary>
    /// Trigger scheduler task
    /// </summary>
    /// <param name="item">scheduler task</param>
    private void TriggerSchedulerItem(SchedulerManagerViewModel item)
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

            UpdateSchedulerItem(item,requestJson, responseJson,meteringUsageResult.Status);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }
    /// <summary>
    /// Update Scheduler Item
    /// </summary>
    /// <param name="item">scheduler task</param>
    /// <param name="requestJson">usage post payload</param>
    /// <param name="responseJson">API respond</param>
    /// <param name="status">status code</param>
    private void UpdateSchedulerItem(SchedulerManagerViewModel item,string requestJson,string responseJson,string status)
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

            // Check if Sending Email is Enabled
            _= bool.TryParse(applicationConfigRepository.GetValueByName("EnablesSuccessfulSchedulerEmail"), out bool enablesSuccessfulSchedulerEmail);
            _ = bool.TryParse(applicationConfigRepository.GetValueByName("EnablesFailureSchedulerEmail"), out bool enablesFailureSchedulerEmail);
            if(enablesFailureSchedulerEmail || enablesSuccessfulSchedulerEmail)
            {
                schedulerService.SendSchedulerEmail(item, newMeteredAuditLog);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Print Scheduler item
    /// </summary>
    /// <param name="item">scheduler item</param>
    /// <param name="nextRun">next run time</param>
    /// <param name="timeDifferenceInHours">difference time</param>
    private void PrintScheduler(SchedulerManagerViewModel item, 
        DateTime? nextRun, 
        int timeDifferenceInHours)
    {
        Console.WriteLine($"Item Id: {item.Id} " + Environment.NewLine+
                          $"Expected NextRun : {nextRun} "+Environment.NewLine+
                          $"SubId : {item.AMPSubscriptionId} "+Environment.NewLine+
                          $"Plan : {item.PlanId} " + Environment.NewLine +
                          $"Dim : {item.Dimension} " + Environment.NewLine +
                          $"Start Date : {item.StartDate} " + Environment.NewLine +
                          $"NextRun : {item.NextRunTime}" + Environment.NewLine +
                          $"TimeDifferenceInHours : {timeDifferenceInHours}" + Environment.NewLine );
    }
    /// <summary>
    /// Get Next Run Time
    /// </summary>
    /// <param name="startDate">Start task Date</param>
    /// <param name="frequency">Task frequency</param>
    /// <returns></returns>
    private DateTime? GetNextRunTime(DateTime? startDate, SchedulerFrequencyEnum frequency)
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