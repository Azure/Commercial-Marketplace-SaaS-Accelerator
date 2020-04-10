using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Services;
using Microsoft.Marketplace.SaasKit.Client.Models;
using System.Collections;
using Commons.Collections;
using NVelocity.App;
using NVelocity;
using System.IO;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Models;

namespace Microsoft.Marketplace.SaasKit.Client.Services
{
    public class TemplateService
    {
        public static string ProcessTemplate(SubscriptionResult Subscription, IEmailTemplateRepository emailTemplateRepository, IApplicationConfigRepository applicationConfigRepository)
        {
            string body = emailTemplateRepository.GetTemplateBody(Subscription.SaasSubscriptionStatus.ToString());
            string applicationName = applicationConfigRepository.GetValuefromApplicationConfig("ApplicationName");
            Hashtable hashTable = new Hashtable();
            hashTable.Add("ApplicationName", applicationName);
            hashTable.Add("CustomerEmailAddress", Subscription.CustomerEmailAddress);
            hashTable.Add("CustomerName", Subscription.CustomerName);
            hashTable.Add("Id", Subscription.Id);
            hashTable.Add("SubscriptionName", Subscription.Name);
            hashTable.Add("SaasSubscriptionStatus", Subscription.SaasSubscriptionStatus);

            ExtendedProperties p = new ExtendedProperties();

            VelocityEngine v = new VelocityEngine();
            v.Init(p);

            VelocityContext context = new VelocityContext(hashTable);
            StringWriter writer = new StringWriter();
            v.Evaluate(context, writer, string.Empty, body);
            return writer.ToString();
        }
    }
}
