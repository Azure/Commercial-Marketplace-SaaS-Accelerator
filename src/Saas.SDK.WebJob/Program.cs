using Microsoft.Azure.WebJobs;
using Microsoft.Marketplace.SaasKit.WebJob.Models;
using System;
using System.IO;

namespace Microsoft.Marketplace.SaasKit.WebJob
{
    class Program
    {
        static void Main(string[] args)
        {
            JobHost jobHost = new JobHost();
            jobHost.RunAndBlock();
        }
        public static void ProcessQueueMessage([QueueTrigger("queue-SaaSAppwebJob")] SubscriptionProcessQueueModel model, TextWriter logger)
        {
            Method(model);
            Method2(model);
        }
        public static void Method(SubscriptionProcessQueueModel model)
        {
        }
        public static void Method2(SubscriptionProcessQueueModel model)
        {
        }
    }
}
