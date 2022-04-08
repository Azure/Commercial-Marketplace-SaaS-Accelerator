using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Microsoft.Marketplace.SaaS.SDK.Services.Models;
using Microsoft.Marketplace.SaaS.SDK.Services.Services;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;

namespace SaaS.SDK.MeteredSchedulerProcessor
{
    public class MeteredSchedulerProcessor
    {

        private MeteredPlanSchedulerManagementService scheudelerService;
        private ISchedulerFrequencyRepository frequencyRepository;
        private IMeteredPlanSchedulerManagementRepository schedulerRepository;
        private ISchedulerManagerViewRepository schedulerViewRepository;
        public MeteredSchedulerProcessor(ISchedulerFrequencyRepository frequencyRepository,
                               IMeteredPlanSchedulerManagementRepository schedulerRepository,
                               ISchedulerManagerViewRepository schedulerViewRepository)
        {
            this.frequencyRepository = frequencyRepository;
            this.schedulerRepository = schedulerRepository;
            this.schedulerViewRepository = schedulerViewRepository;

        }

        [FunctionName("MeteredSchedulerProcessor")]
        public void Run([TimerTrigger("0 */1 * * * *", RunOnStartup = true)]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            this.scheudelerService = new MeteredPlanSchedulerManagementService(this.frequencyRepository, this.schedulerRepository, this.schedulerViewRepository);    
            List<SchedulerManagerViewModel> getAllSchedulerManagerViewData = this.scheudelerService.GetAllSchedulerManagerList();
            foreach (SchedulerManagerViewModel item in getAllSchedulerManagerViewData)
            {
                log.LogInformation($"Processing Subscription : {item.AMPSubscriptionId}");
                log.LogInformation($"Processing Dim : {item.Dimension}");
                log.LogInformation($"Processing Frequecy : {item.Frequency}");
            }
        }
    }
}
