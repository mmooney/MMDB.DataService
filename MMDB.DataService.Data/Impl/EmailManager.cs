using MMDB.DataService.Data.Dto.Email;
using MMDB.DataService.Data.Dto.Jobs;
using MMDB.DataService.Data.Settings;
using MMDB.RazorEmail;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Transactions;

namespace MMDB.DataService.Data.Impl
{
    public class EmailManager : IEmailManager
    {
        private readonly IDocumentSession _documentSession;
        private readonly IEventReporter _eventReporter;
        private readonly IConnectionSettingsManager _connectionSettingsManager;
        private readonly EmailSender _emailSender;

        public EmailManager(IDocumentSession documentSession, IEventReporter eventReporter, IConnectionSettingsManager connectionSettingsManager, EmailSender emailSender)
        {
            _documentSession = documentSession;
            _eventReporter = eventReporter;
            _connectionSettingsManager = connectionSettingsManager;
            _emailSender = emailSender;
        }

        public EmailJobData PopNextJobData()
        {
            EmailJobData returnValue = null;
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
            {
                returnValue = _documentSession.Query<EmailJobData>()
                                                    .Customize(i => i.WaitForNonStaleResultsAsOfLastWrite(TimeSpan.FromSeconds(120)))
                                                    .OrderBy(i => i.QueuedDateTimeUtc)
                                                    .FirstOrDefault(i => i.Status == EnumJobStatus.New);
                if (returnValue != null)
                {
                    returnValue = _documentSession.Load<EmailJobData>(returnValue.Id);
                    if (returnValue.Status != EnumJobStatus.New)
                    {
                        _eventReporter.WarningForObject("EmailJobData was returned as New from query but was really " + returnValue.Status.ToString(), returnValue);
                        return null;
                    }
                    else
                    {
                        returnValue.Status = EnumJobStatus.InProcess;
                        _documentSession.SaveChanges();
                        transaction.Complete();
                    }
                }
            }
            return returnValue;
        }

        public void ProcessItem(EmailJobData jobItem)
        {
            var emailConnectionSettings = _connectionSettingsManager.Load<EmailConnectionSettings>(jobItem.SettingsSource, jobItem.SettingsKey);
            var emailServerSettings = new EmailServerSettings
            {
                Host = emailConnectionSettings.Host,
                Port = emailConnectionSettings.Port,
                UserName = emailConnectionSettings.UserName,
                Password = emailConnectionSettings.Password
            };
            string overrideEmailAddress = ConfigurationManager.AppSettings["OverrideAllEmailAddress"];
            var toAddressList = DataServiceMailAddress.ToMailAddressList(jobItem.ToAddressList);
            if (!string.IsNullOrWhiteSpace(overrideEmailAddress))
            {
                toAddressList = new List<MailAddress>()
                {
                    new MailAddress(overrideEmailAddress)
                };
            }
            string subject = FilterSubject(jobItem.Subject);
            try
            {
                EmailAttachmentData[] attachments = null;
                if(jobItem.Attachments != null)
                {
                    attachments = jobItem.Attachments.ToArray();
                }
                _emailSender.SendEmail(emailServerSettings, subject, jobItem.Body, toAddressList, jobItem.FromAddress.ToMailAddress(), attachments);
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

        private string FilterSubject(string value)
        {
            if(string.IsNullOrEmpty(value))
            {
                return value;
            }
            else 
            {
                return value.Replace("\r\n", " ").Replace("\r"," ").Replace("\n"," ");
            }
        }

        public void MarkItemSuccessful(EmailJobData jobData)
        {
            jobData.Status = EnumJobStatus.Complete;
            _documentSession.SaveChanges();
        }

        public void MarkItemFailed(EmailJobData jobData, Exception err)
        {
            jobData.FailureCount++;
            jobData.AddException(err);

            if(jobData.FailureCount > 3)
            {
                jobData.Status = EnumJobStatus.Error;
            }
            else 
            {
                jobData.Status = EnumJobStatus.New;
                jobData.QueuedDateTimeUtc = DateTime.UtcNow;
            }
            _documentSession.SaveChanges();
        }
    }
}
