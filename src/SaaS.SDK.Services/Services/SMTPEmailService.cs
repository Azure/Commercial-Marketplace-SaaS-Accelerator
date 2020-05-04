using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    public class SMTPEmailService : IEmailService
    {
        private readonly IApplicationConfigRepository applicationConfigRepository;

        public SMTPEmailService(IApplicationConfigRepository applicationConfigRepository)
        {
            this.applicationConfigRepository = applicationConfigRepository;

        }
        public void SendEmail(string to, string cc, string bcc, string body)
        {


            //var applicationConfigs = this.applicationConfigRepository.GetValuefromApplicationConfig();
            MailMessage mail = new MailMessage();
            string FromMail = this.applicationConfigRepository.GetValuefromApplicationConfig("SMTPFromEmail");
            string password = applicationConfigRepository.GetValuefromApplicationConfig("SMTPPassword");
            string username = applicationConfigRepository.GetValuefromApplicationConfig("SMTPUserName");
            string Subject = string.Empty;
            bool smtpSsl = bool.Parse(applicationConfigRepository.GetValuefromApplicationConfig("SMTPSslEnabled"));
            mail.From = new MailAddress(FromMail);
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = applicationConfigRepository.GetValuefromApplicationConfig("SMTPHost");
            smtp.Port = int.Parse(applicationConfigRepository.GetValuefromApplicationConfig("SMTPPort"));
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(
                username, password);
            smtp.EnableSsl = smtpSsl;
            smtp.Send(mail);



        }
    }
}
