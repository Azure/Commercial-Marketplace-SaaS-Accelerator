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

            //Process each scheduler frequency
            foreach (SchedulerFrequencyEnum frequency in Enum.GetValues(typeof(SchedulerFrequencyEnum)))
            {

                var scheduledItems = getAllSchedulerManagerViewData.Where(a => a.Frequency == frequency.ToString()).ToList();
                foreach (var scheduledItem in scheduledItems)
                {
                    // Get next run time based on Schedule Frequency

                    DateTime? _nextRunTime;
                    if (scheduledItem.NextRunTime is not null)
                        _nextRunTime = GetNextRunTime(scheduledItem.NextRunTime, frequency);
                    else
                        _nextRunTime = GetNextRunTime(scheduledItem.StartDate, frequency);

                    // Print the scheduled Item and the expected run date
                    PrintScheduler(scheduledItem, _nextRunTime);

                    //Trigger the Item Now
                    if ((_nextRunTime.HasValue) && (DateTime.Now >= _nextRunTime.Value))
                    {
                        TriggerSchedulerItem(scheduledItem, frequency, billingApiService, schedulerService, subscriptionUsageLogsRepository);
                    }
                    else
                    {
                        Console.WriteLine($"Item Id: {scheduledItem.Id} next run will be {scheduledItem.NextRunTime}");
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
                Console.WriteLine($"Trigger Scheduler Item Id: {item.Id}");

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
                    Console.WriteLine($"Execute EmitUsageEventAsync for request {requestJson}");
                    meteringUsageResult = billingApiService.EmitUsageEventAsync(subscriptionUsageRequest).ConfigureAwait(false).GetAwaiter().GetResult();
                    responseJson = JsonSerializer.Serialize(meteringUsageResult);
                    Console.WriteLine($"Got the following result{responseJson}");
                }
                catch (MarketplaceException marketplaceException)
                {
                    responseJson = JsonSerializer.Serialize(marketplaceException.Message);
                    meteringUsageResult.Status = marketplaceException.ErrorCode;
                    Console.WriteLine($"Error during executing EmitUsageEventAsync got the following error {responseJson}");
                }

                item.NextRunTime = GetNextRunTime(DateTime.Now, frequency);
                UpdateSchedulerItem(item, requestJson, responseJson, meteringUsageResult.Status,schedulerService,subscriptionUsageLogsRepository);
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
                Console.WriteLine($"Save Audit information for metered: {item.Dimension}");
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
                    Console.WriteLine($"Meter event Accepted, Save Scheduler NextRun for ItemId: {item.Id}");

                    //ignore nextruntime for OneTime
                    if(item.Frequency != SchedulerFrequencyEnum.OneTime.ToString())
                    {
                        scheduler.NextRunTime = item.NextRunTime.Value.ToUniversalTime();
                        schedulerService.SaveSchedulerDetail(scheduler);
                    }
                }
                else
                {
                    Console.WriteLine($"Meter reporting for Scheduler Item Id: {item.Id} failed with status {status}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void PrintScheduler(SchedulerManagerViewModel item, DateTime? nextRun)
        {
            Console.WriteLine($"Scheduler : {item.Frequency}");
            Console.WriteLine($"SubId : {item.AMPSubscriptionId}");
            Console.WriteLine($"Plan : {item.PlanId}");
            Console.WriteLine($"Dim : {item.Dimension}");
            Console.WriteLine($"Start Date : {item.StartDate}");
            Console.WriteLine($"current NextRun : {item.NextRunTime}");
            Console.WriteLine($"Expected NextRun : {nextRun}");

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
