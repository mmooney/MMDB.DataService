using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MMDB.DataEmail;
using MMDB.DataService.Data.Dto.Logging;
using MMDB.DataService.Data.Settings;

namespace MMDB.DataService.Data
{
	public class ExceptionReporter
	{
		private SettingsManager SettingsManager { get; set; }
		private EmailSender EmailSender { get; set; }

		public ExceptionReporter(SettingsManager settingsManager, EmailSender emailSender)
		{
			this.SettingsManager = settingsManager;
			this.EmailSender = emailSender;
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

				this.EmailSender.SendEmail(subject, body.ToString(), settings.ExceptionNotificationEmailAddressList, settings.ExceptionNotificationFromEmailAddress);
			}
			catch (Exception err)
			{
				Debug.WriteLine("Error sending email: " + err.ToString());
				//Who you gonna go cry to now?
			}
		}
	}
}
