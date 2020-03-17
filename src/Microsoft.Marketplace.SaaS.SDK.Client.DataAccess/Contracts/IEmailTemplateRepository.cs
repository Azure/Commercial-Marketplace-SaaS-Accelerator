using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    public interface IEmailTemplateRepository
    {
        string GetSubject(string status);
        string GetTemplateBody(string status);
        string GetCCRecipients(string status);
        string GetToRecipients(string status);
        string GetBccRecipients(string status);
    }
}
