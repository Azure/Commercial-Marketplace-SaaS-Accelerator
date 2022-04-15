using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Microsoft.Marketplace.SaaS.SDK.Services.Models;
using Microsoft.Marketplace.SaaS.SDK.Services.Services;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System.Text.Json;
using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
using Microsoft.Marketplace.SaaS.SDK.Services.Exceptions;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Microsoft.Extensions.Configuration;
using Microsoft.Marketplace.SaaS.SDK.Services.Configurations;

namespace SaaS.SDK.MeteredSchedulerProcessor
{
    public class MeteredSchedulerProcessor
    {
        private MeteredPlanSchedulerManagementService scheudelerService;
        private ISchedulerFrequencyRepository frequencyRepository;
        private IMeteredPlanSchedulerManagementRepository schedulerRepository;
        private ISchedulerManagerViewRepository schedulerViewRepository;
        private ISubscriptionUsageLogsRepository subscriptionUsageLogsRepository;

        private readonly IMeteredBillingApiService billingApiService;
        private readonly SaaSApiClientConfiguration saaasConfiguration;

        public MeteredSchedulerProcessor(SaaSApiClientConfiguration saaasConfiguration, ISchedulerFrequencyRepository frequencyRepository,
                               IMeteredPlanSchedulerManagementRepository schedulerRepository,
                               ISchedulerManagerViewRepository schedulerViewRepository, ISubscriptionUsageLogsRepository subscriptionUsageLogsRepository, IMeteredBillingApiService billingApiService)
        {
            this.frequencyRepository = frequencyRepository;
            this.schedulerRepository = schedulerRepository;
            this.schedulerViewRepository = schedulerViewRepository;
            this.subscriptionUsageLogsRepository = subscriptionUsageLogsRepository;
            this.scheudelerService = new MeteredPlanSchedulerManagementService(this.frequencyRepository, this.schedulerRepository, this.schedulerViewRepository, this.subscriptionUsageLogsRepository);
            this.billingApiService = billingApiService;
            this.saaasConfiguration = saaasConfiguration;
        }

        [FunctionName("MeteredSchedulerProcessor")]
        public void Run([TimerTrigger("0 0 */1 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            if (this.saaasConfiguration.SupportMeteredBilling)
            {
                //Get all Scheduled Data
                List<SchedulerManagerViewModel> getAllSchedulerManagerViewData = this.scheudelerService.GetAllSchedulerManagerList();

                //Process each scheduler frequency
                foreach (SchedulerFrequencyEnum frequency in Enum.GetValues(typeof(SchedulerFrequencyEnum)))
                {
                    ProcessSchedulerSet(getAllSchedulerManagerViewData, frequency, log);
                }
            }
            else
                log.LogInformation($"This Feature is not enabled");


        }

        private void ProcessSchedulerSet(List<SchedulerManagerViewModel> item, SchedulerFrequencyEnum frequency, ILogger log)
        {
            var scheduledItems = item.Where(a => a.Frequency == frequency.ToString()).ToList();
            foreach (var scheduledItem in scheduledItems)
            {
                // Get next run time based on Schedule Frequency

                DateTime? _nextRunTime;
                if (scheduledItem.NextRunTime is not null)
                    _nextRunTime = GetNextRunTime(scheduledItem.NextRunTime, frequency);
                else
                    _nextRunTime = GetNextRunTime(scheduledItem.StartDate, frequency);

                // Print the scheduled Item and the expected run date
                PrintScheduler(scheduledItem, _nextRunTime, log);

                //Trigger the Item Now
                if ((_nextRunTime.HasValue) && (DateTime.Now >= _nextRunTime.Value))
                {
                    TriggerSchedulerItem(scheduledItem, frequency,log);
                }
                else
                {
                    log.LogInformation($"Item Id: {scheduledItem.Id} next run will be {scheduledItem.NextRunTime}");
                }
            }
        }


        public void TriggerSchedulerItem(SchedulerManagerViewModel item, SchedulerFrequencyEnum frequency, ILogger log)
        {
            try
            {
                log.LogInformation($"Trigger Scheduler Item Id: {item.Id}");
                    
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
                    log.LogInformation($"Execute EmitUsageEventAsync for request {requestJson}");
                    meteringUsageResult = this.billingApiService.EmitUsageEventAsync(subscriptionUsageRequest).ConfigureAwait(false).GetAwaiter().GetResult();
                    responseJson = JsonSerializer.Serialize(meteringUsageResult);
                    log.LogInformation($"Got the following result{responseJson}");
                }
                catch (MarketplaceException mex)
                {
                    responseJson = JsonSerializer.Serialize(mex.Message);
                    meteringUsageResult.Status = mex.ErrorCode;
                    log.LogInformation($"Error during executing EmitUsageEventAsync got the following error {responseJson}");
                }

                item.NextRunTime = GetNextRunTime(DateTime.Now, frequency);
                UpdateSchedulerItem(item, requestJson, responseJson, meteringUsageResult.Status, log);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }

        }
        public void UpdateSchedulerItem(SchedulerManagerViewModel item, string requestJson, string responseJson, string status, ILogger log)
        {
            try
            {
                log.LogInformation($"Save Audit information for metered: {item.Dimension}");
                var scheduler = this.scheudelerService.GetSchedulerDetailById(item.Id);
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
                this.subscriptionUsageLogsRepository.Save(newMeteredAuditLog);

                if ((status == "Accepted") )
                {
                    log.LogInformation($"Save Scheduler Item Id: {item.Id}");
                    scheduler.NextRunTime = item.NextRunTime;
                    this.scheudelerService.SaveSchedulerDetail(scheduler);
                }
            }
            catch (Exception ex)
            {
                    log.LogError(ex.Message);
            }
        }

        private void PrintScheduler(SchedulerManagerViewModel item, DateTime? nextRun, ILogger log)
        {
            log.LogInformation($"Scheduler : {item.Frequency}");
            log.LogInformation($"SubId : {item.AMPSubscriptionId}");
            log.LogInformation($"Plan : {item.PlanId}");
            log.LogInformation($"Dim : {item.Dimension}");
            log.LogInformation($"Start Date : {item.StartDate}");
            log.LogInformation($"current NextRun : {item.NextRunTime}");
            log.LogInformation($"Expected NextRun : {nextRun}");

        }

        public DateTime? GetNextRunTime(DateTime? startDate, SchedulerFrequencyEnum frequency)
        {
            switch (frequency)
            {
                case SchedulerFrequencyEnum.Hourly: { return startDate.Value.AddHours(1); }
                case SchedulerFrequencyEnum.Daily: { return startDate.Value.AddDays(1); }
                case SchedulerFrequencyEnum.Weekly: { return startDate.Value.AddDays(7); }
                case SchedulerFrequencyEnum.Monthly: { return startDate.Value.AddMonths(1); }
                case SchedulerFrequencyEnum.Yearly: { return startDate.Value.AddYears(1); }
                default:
                    { return null; }
            }
        }

    }
}
