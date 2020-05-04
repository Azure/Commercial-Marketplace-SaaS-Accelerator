using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Services;
using Microsoft.Marketplace.SaaS.SDK.Services.Models;
using System.Collections;
using Commons.Collections;
using NVelocity.App;
using NVelocity;
using System.IO;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Models;
//using Microsoft.Marketplace.SaaS.SDK.CustomerProvisioning.Models;
using System.Linq;

namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    public class TemplateService
    {
        public static string ProcessTemplate(SubscriptionResultExtension Subscription, IEmailTemplateRepository emailTemplateRepository, IApplicationConfigRepository applicationConfigRepository, string planEvent, SubscriptionStatusEnumExtension oldValue, string newValue)
        {
            //string body = emailTemplateRepository.GetTemplateBody(Subscription.SaasSubscriptionStatus.ToString());
            string body = string.Empty;
            body = emailTemplateRepository.GetTemplateBody("Template");

            string applicationName = applicationConfigRepository.GetValuefromApplicationConfig("ApplicationName");
            Hashtable hashTable = new Hashtable();
            hashTable.Add("ApplicationName", applicationName);
            hashTable.Add("CustomerEmailAddress", Subscription.CustomerEmailAddress);
            hashTable.Add("CustomerName", Subscription.CustomerName);
            hashTable.Add("Id", Subscription.Id);
            hashTable.Add("SubscriptionName", Subscription.Name);
            hashTable.Add("SaasSubscriptionStatus", Subscription.SaasSubscriptionStatus);
            hashTable.Add("oldValue", oldValue);
            hashTable.Add("newValue", newValue);
            hashTable.Add("planevent", planEvent);


            ExtendedProperties properties = new ExtendedProperties();

            VelocityEngine engine = new VelocityEngine();
            engine.Init(properties);

            VelocityContext context = new VelocityContext(hashTable);

            IList list;
            if (Subscription.SubscriptionParameters != null && Subscription.SubscriptionParameters.Count > 0)
            {
                list = Subscription.SubscriptionParameters.Where(s => s.Type.ToLower() == "input").ToList();
                if (list.Count > 0)
                    context.Put("parms", list);
            }

            StringWriter writer = new StringWriter();
            engine.Evaluate(context, writer, string.Empty, body);
            return writer.ToString();
        }
    }
}
