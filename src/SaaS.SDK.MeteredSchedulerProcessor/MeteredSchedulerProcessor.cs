using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Marketplace.SaaS.SDK.Services.Models;
using Microsoft.Marketplace.SaaS.SDK.Services.Services;

namespace SaaS.SDK.MeteredSchedulerProcessor
{
    public class MeteredSchedulerProcessor
    {
        private MeteredPlanSchedulerManagementService scheudelerService;

        [FunctionName("MeteredSchedulerProcessor")]
        public void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            List<SchedulerManagerViewModel> getAllSchedulerManagerViewData = new List<SchedulerManagerViewModel>();

            foreach (SchedulerManagerViewModel item in getAllSchedulerManagerViewData)
            {
                log.LogInformation($"Processing Subscription : {item.AMPSubscriptionId}");
                log.LogInformation($"Processing Dim : {item.Dimension}");
                log.LogInformation($"Processing Frequecy : {item.Frequency}");
            }

        }
    }
}
