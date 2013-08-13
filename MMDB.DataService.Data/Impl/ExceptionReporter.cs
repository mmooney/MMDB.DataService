using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using MMDB.DataService.Data.Dto.Logging;
using MMDB.DataService.Data.Settings;
using MMDB.RazorEmail;

namespace MMDB.DataService.Data.Impl
{
	public class ExceptionReporter : IExceptionReporter
	{
		private ISettingsManager SettingsManager { get; set; }
		private IConnectionSettingsManager ConnectionSettingsManager { get; set; }
		private EmailSender EmailSender { get; set; }

		public ExceptionReporter(ISettingsManager settingsManager, EmailSender emailSender, IConnectionSettingsManager connectionSettingsManager)
		{
			this.SettingsManager = settingsManager;
			this.EmailSender = emailSender;
			this.ConnectionSettingsManager = connectionSettingsManager;
		}

		public void Exception(ServiceMessage exception)
		{
			try
			{
				string subject = "DataServiceException: " + exception.Message + "(" + exception.Id + ")";
				StringBuilder body = new StringBuilder();
				body.AppendLine(string.Format("Exception Date: {0}", exception.MessageDateTimeUtc));
				body.AppendLine(string.Format("Exception Message: {0}", exception.Message));
				body.AppendLine("Exception Detail:");
				body.AppendLine(exception.Detail);
				if (!string.IsNullOrWhiteSpace(exception.DataObjectJson))
				{
					body.AppendLine();
					body.AppendLine("Data Object JSON: ");
					body.AppendLine(exception.DataObjectJson);
				}

				var settings = this.SettingsManager.Get<CoreDataServiceSettings>();
				//var emailSettings = new EmailServerSettings 
				//{
				//	Host = settings.Email.Host,
				//	Port = settings.Email.Port,
				//	UserName = settings.Email.UserName,
				//	Password = settings.Email.Password
				//};
				var emailSettings = this.ConnectionSettingsManager.Load<EmailConnectionSettings>(settings.EmailSettingSource, settings.EmailSettingKey);
				var emailServerSettings = new EmailServerSettings
				{
					Host = emailSettings.Host,
					Password = emailSettings.Password,
					Port = emailSettings.Port,
					UserName = emailSettings.UserName
				};
				var from = new MailAddress(settings.ExceptionNotificationFromEmailAddress);
				var toList = new List<MailAddress>();
				if(settings.ExceptionNotificationFromEmailAddress != null)
				{
					foreach(var item in settings.ExceptionNotificationEmailAddressList)
					{
						var to = new MailAddress(item);
						toList.Add(to);
					}
				}
				string overrideEmailAddress = ConfigurationManager.AppSettings["OverrideAllEmailAddress"];
				if(!string.IsNullOrWhiteSpace(overrideEmailAddress))
				{
					toList = new List<MailAddress>()
					{
						new MailAddress(overrideEmailAddress)
					};
				}
				this.EmailSender.SendEmail(emailServerSettings, subject, body.ToString(), toList, from, null);
			}
			catch (Exception err)
			{
				Debug.WriteLine("Error sending email: " + err.ToString());
				//Who you gonna go cry to now?
			}
		}
	}
}
