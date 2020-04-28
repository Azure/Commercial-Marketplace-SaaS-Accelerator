using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Services;
using System.Collections;
using Commons.Collections;
using NVelocity.App;
using NVelocity;
using System.IO;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Models;
using Saas.SDK.WebJob.Models;
using System.Linq;

namespace Saas.SDK.WebJob.Services
{
    public class TemplateService
    {
        public static string ProcessTemplate(SubscriptionResultExtension Subscription, IEmailTemplateRepository emailTemplateRepository, IApplicationConfigRepository applicationConfigRepository,string planEvent,SubscriptionStatusEnum oldValue, string newValue)
        {
            string body = string.Empty;
            string parameter = string.Empty;
            string value = string.Empty;
            string parameterType = string.Empty;


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


            ExtendedProperties p = new ExtendedProperties();

            VelocityEngine v = new VelocityEngine();
            v.Init(p);

            VelocityContext context = new VelocityContext(hashTable);
            IList list;
            IList arminputlist;
            IList armoutputlist;
            if (Subscription.SubscriptionParameters != null && Subscription.SubscriptionParameters.Count > 0)
            {
                list = Subscription.SubscriptionParameters.Where(s => s.Type.ToLower() == "input").ToList();
                if (list.Count > 0)
                    context.Put("parms", list);
            }
            if (Subscription.ARMTemplateParameters != null && Subscription.ARMTemplateParameters.Count > 0)
            {
                arminputlist = Subscription.ARMTemplateParameters.Where(s => s.ParameterType.ToLower() == "input" && s.EventsName == "Active").ToList();
                if (arminputlist.Count > 0)
                    context.Put("arminputparms", arminputlist);
            }
            if (Subscription.ARMTemplateParameters!= null && Subscription.ARMTemplateParameters.Count>0)
            {
                armoutputlist = Subscription.ARMTemplateParameters.Where(s => s.ParameterType.ToLower() == "output" && s.EventsName == "Active").ToList();
                if (armoutputlist.Count > 0)
                    context.Put("armoutputparms", armoutputlist);
            }
            StringWriter writer = new StringWriter();
            v.Evaluate(context, writer, string.Empty, body);
            return writer.ToString();

        }
    }
}
