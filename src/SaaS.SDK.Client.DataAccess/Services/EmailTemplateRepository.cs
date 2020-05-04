using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
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

        public EmailTemplate GetEmailTemplateOnStatus(string status)
        {
            var template = Context.EmailTemplate.Where(s => s.Status == status).FirstOrDefault();
            if (template != null)
            {
                return template;
            }
            return null;
        }
    }
}
