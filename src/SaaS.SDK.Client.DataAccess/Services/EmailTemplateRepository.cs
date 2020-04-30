using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    public class EmailTemplateRepository : IEmailTemplateRepository
    {
        private readonly SaasKitContext Context;

        public EmailTemplateRepository(SaasKitContext context)
        {
            Context = context;
        }

        public string GetSubject(string status)
        {
            string subject = "";
            var template = Context.EmailTemplate.Where(s => s.Status == status).FirstOrDefault();
            if (template != null)
            {
                subject = template.Subject;


            }
            return subject;
        }

        public bool? GetIsActive(string status)
        {
            bool? isactive = false;
            var template = Context.EmailTemplate.Where(s => s.Status == status).FirstOrDefault();
            if (template != null)
            {
                isactive = template.IsActive;


            }
            return isactive;


        }

        public string GetTemplateBody(string status)
        {
            string templateBody = "";
            var template = Context.EmailTemplate.Where(s => s.Status == status).FirstOrDefault();
            if (template != null)
            {
                templateBody = template.TemplateBody;

            }
            return templateBody;
        }

        public string GetCCRecipients(string status)
        {
            string cc = "";
            var cclist = Context.EmailTemplate.Where(s => s.Status == status).FirstOrDefault();
            if (cclist != null)
            {

                cc = cclist.Cc;

            }

            return cc;
        }

        public string GetToRecipients(string status)
        {
            string toRecipients = "";
            var tempalte = Context.EmailTemplate.Where(s => s.Status == status).FirstOrDefault();
            if (tempalte != null)
            {
                toRecipients = tempalte.ToRecipients;
            }
            return toRecipients;
        }

        public string GetBccRecipients(string status)
        {
            string bcc = "";
            var tempalte = Context.EmailTemplate.Where(s => s.Status == status).FirstOrDefault();
            if (tempalte != null)
            {
                bcc = tempalte.Bcc;
            }
            return bcc;


        }
    }
}
