using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Marketplace.SaaS.SDK.Services.Models;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System.Text.Json;
using Microsoft.Marketplace.SaaS.SDK.Services.Exceptions;
using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
using Microsoft.Marketplace.SaaS.SDK.Services.Services;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaaS.SDK.Services.Configurations;

namespace MeteredTriggerHelper
{
    public class Executor
    {

        private MeteredPlanSchedulerManagementService schedulerService;
        private ISchedulerFrequencyRepository frequencyRepository;
        private IMeteredPlanSchedulerManagementRepository schedulerRepository;
        private ISchedulerManagerViewRepository schedulerViewRepository;
        private ISubscriptionUsageLogsRepository subscriptionUsageLogsRepository;
        private readonly IMeteredBillingApiService billingApiService;
        

        public Executor(ISchedulerFrequencyRepository frequencyRepository,
                               IMeteredPlanSchedulerManagementRepository schedulerRepository,
                               ISchedulerManagerViewRepository schedulerViewRepository, ISubscriptionUsageLogsRepository subscriptionUsageLogsRepository, IMeteredBillingApiService billingApiService)
        {
            this.frequencyRepository = frequencyRepository;
            this.schedulerRepository = schedulerRepository;
            this.schedulerViewRepository = schedulerViewRepository;
            this.subscriptionUsageLogsRepository = subscriptionUsageLogsRepository;
            this.schedulerService = new MeteredPlanSchedulerManagementService(this.frequencyRepository, this.schedulerRepository, this.schedulerViewRepository, this.subscriptionUsageLogsRepository);
            this.billingApiService = billingApiService;
            
        }

        public void Execute()
        {
            schedulerService = new MeteredPlanSchedulerManagementService(frequencyRepository, schedulerRepository, schedulerViewRepository, subscriptionUsageLogsRepository);

            //Get all Scheduled Data
            List<SchedulerManagerViewModel> getAllSchedulerManagerViewData = schedulerService.GetAllSchedulerManagerList();

            //GetCurrentUTC time
            DateTime _currentUTCTime = DateTime.UtcNow;

            //Process each scheduler frequency
            foreach (SchedulerFrequencyEnum frequency in Enum.GetValues(typeof(SchedulerFrequencyEnum)))
            {
                Console.WriteLine($"==== Checking all {frequency} scheduled items ====");

                var scheduledItems = getAllSchedulerManagerViewData.Where(a => a.Frequency == frequency.ToString()).ToList();
                foreach (var scheduledItem in scheduledItems)
                {
                    Console.WriteLine();
                    
                    // Get the run time.
                    //Always pickup the NextRuntime, when its firstRun or OneTime then pickup StartDate as the NextRunTime will be null
                    DateTime? _nextRunTime = scheduledItem.NextRunTime ?? scheduledItem.StartDate;
                    int timeDifferentInHours = _currentUTCTime.Subtract(_nextRunTime.Value).Hours;

                    // Print the scheduled Item and the expected run date
                    PrintScheduler(scheduledItem, _nextRunTime);

                    //Past scheduler items
                    if (timeDifferentInHours > 0)
                    {
                        Console.WriteLine($"Item Id: {scheduledItem.Id} will never be run as {_nextRunTime} has passed. Please check audit logs if its has run previously.");
                        continue;
                    }else if(timeDifferentInHours < 0) 
                    {
                        Console.WriteLine($"Item Id: {scheduledItem.Id} future run will be at {_nextRunTime} UTC. Time for the run is {timeDifferentInHours} hours");
                        continue;
                    }
                    else
                    {
                        TriggerSchedulerItem(scheduledItem, frequency, billingApiService, schedulerService, subscriptionUsageLogsRepository);
                    }
                }
            }

        }

        public static void TriggerSchedulerItem(SchedulerManagerViewModel item, SchedulerFrequencyEnum frequency, IMeteredBillingApiService billingApiService,
                                                MeteredPlanSchedulerManagementService schedulerService,
                                                ISubscriptionUsageLogsRepository subscriptionUsageLogsRepository)
        {
            try
            {
                Console.WriteLine($"---- Item Id: {item.Id} Triggering meter event ----");

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

                UpdateSchedulerItem(item, requestJson, responseJson, meteringUsageResult.Status, schedulerService, subscriptionUsageLogsRepository);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public static void UpdateSchedulerItem(SchedulerManagerViewModel item, string requestJson, string responseJson, string status, 
                                                MeteredPlanSchedulerManagementService schedulerService,
                                                ISubscriptionUsageLogsRepository subscriptionUsageLogsRepository)
        {
            try
            {
                Console.WriteLine($"Item Id: {item.Id} Save Audit information");
                var scheduler = schedulerService.GetSchedulerDetailById(item.Id);
                var newMeteredAuditLog = new MeteredAuditLogs()
                {
                    RequestJson = requestJson,
                    ResponseJson = responseJson,
                    StatusCode = status,
                    SubscriptionId = scheduler.SubscriptionId,
                    SubscriptionUsageDate = DateTime.UtcNow,
                    CreatedBy = 0,
                    CreatedDate = DateTime.Now,
                };
                subscriptionUsageLogsRepository.Save(newMeteredAuditLog);

                if ((status == "Accepted"))
                {
                    Console.WriteLine($"Item Id: {item.Id} Meter event Accepted, save Scheduler NextRun if applicable");

                    //Ignore updating NextRuntime value for OneTime frequency as they always depend on StartTime value
                    Enum.TryParse(item.Frequency, out SchedulerFrequencyEnum itemFrequency);
                    if (itemFrequency != SchedulerFrequencyEnum.OneTime)
                    {
                        scheduler.NextRunTime = GetNextRunTime(item.NextRunTime ?? item.StartDate, itemFrequency);
                        schedulerService.UpdateSchedulerNextRunTime(scheduler);
                    }
                }
                else
                {
                    Console.WriteLine($"Item Id: {item.Id} failed with status {status}");
                }
                Console.WriteLine($"Item Id: {item.Id} Complet Triggering Meter event.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void PrintScheduler(SchedulerManagerViewModel item, DateTime? nextRun)
        {
            Console.WriteLine($"Item Id: {item.Id} " +
                              $"Expected NextRun : {nextRun} " +
                              $"SubId : {item.AMPSubscriptionId} " +
                              $"Plan : {item.PlanId} " +
                              $"Dim : {item.Dimension} " +
                              $"Start Date : {item.StartDate} " +
                              $"NextRun : {item.NextRunTime}");
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

        public enum SchedulerFrequencyEnum
        {
            Hourly=1,
            Daily,
            Weekly,
            Monthly,
            Yearly,
            OneTime
        }
    }
}
