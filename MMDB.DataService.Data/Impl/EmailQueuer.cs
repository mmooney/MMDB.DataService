using MMDB.DataService.Data.Dto.Email;
using MMDB.RazorEmail;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace MMDB.DataService.Data.Impl
{
    public class EmailQueuer : IEmailQueuer
    {
        private readonly IDocumentSession _documentSession;

        public EmailQueuer(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public EmailJobData SendEmail(EnumSettingSource settingsSource, string settingsKey, string subject, string body, IEnumerable<MailAddress> toAddressList, MailAddress fromAddress, params EmailAttachmentData[] attachments)
        {
            var jobData = new EmailJobData
            {
                SettingsSource = settingsSource,
                SettingsKey = settingsKey,
                Subject = subject,
                Body = body,
                FromAddress = new DataServiceMailAddress(fromAddress),
                ToAddressList = DataServiceMailAddress.GetList(toAddressList),
                Attachments = attachments.ToList(),
                Status = EnumJobStatus.New,
                QueuedDateTimeUtc = DateTime.UtcNow,
                FailureCount = 0
            };
            return _documentSession.StoreSaveEvict(jobData);
        }

        public EmailJobData SendRazorEmail<T>(EnumSettingSource settingsSource, string settingsKey, string subject, T model, string razorView, IEnumerable<MailAddress> toAddressList, MailAddress fromAddress, params EmailAttachmentData[] attachments)
        {
            try
            {
                string body = RazorEngine.Razor.Parse<T>(razorView, model);
                return this.SendEmail(settingsSource, settingsKey, subject, body, toAddressList, fromAddress, attachments);
            }
            catch (RazorEngine.Templating.TemplateCompilationException ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Error(s) compiling template for email: ");
                foreach (var item in ex.Errors)
                {
                    sb.AppendLine("-" + item.Line.ToString() + ": " + item.ErrorText);
                }
                throw new Exception(sb.ToString(), ex);
            }

        }

        public EmailJobData SendRazorEmail<T>(EnumSettingSource settingsSource, string settingsKey, string subject, T model, string razorView, List<string> toAddressList, string fromAddress, params EmailAttachmentData[] attachments)
        {
            var toList = toAddressList.Select(i => new MailAddress(i)).ToList();
            var from = new MailAddress(fromAddress);
            return this.SendRazorEmail(settingsSource, settingsKey, subject, model, razorView, toList, from, attachments);
        }
    }
}
