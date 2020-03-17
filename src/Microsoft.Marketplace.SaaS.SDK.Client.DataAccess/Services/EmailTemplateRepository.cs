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
            return Context.EmailTemplate.Where(s => s.Status == status).FirstOrDefault().Subject;
        }

        public string GetTemplateBody(string status)
        {
            return Context.EmailTemplate.Where(s => s.Status == status).FirstOrDefault().TemplateBody;
        }

        public string GetCCRecipients(string status)
        {
            return Context.EmailTemplate.Where(s => s.Status == status).FirstOrDefault().Cc;
        }

        public string GetToRecipients(string status)
        {
            return Context.EmailTemplate.Where(s => s.Status == status).FirstOrDefault().ToRecipients;
        }

        public string GetBccRecipients(string status)
        {
            return Context.EmailTemplate.Where(s => s.Status == status).FirstOrDefault().Bcc;
        }
    }
}
