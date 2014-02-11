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
		private readonly ISettingsManager _settingsManager;
        private readonly IConnectionSettingsManager _connectionSettingsManager;
        private readonly IEmailQueuer _emailQueuer;

		public ExceptionReporter(ISettingsManager settingsManager, IEmailQueuer emailQueuer, IConnectionSettingsManager connectionSettingsManager)
		{
			_settingsManager = settingsManager;
			_emailQueuer = emailQueuer;
			_connectionSettingsManager = connectionSettingsManager;
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

				var settings = _settingsManager.Get<CoreDataServiceSettings>();

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
                _emailQueuer.SendEmail(settings.EmailSettingSource, settings.EmailSettingKey, subject, body.ToString(), toList, from, null);
			}
			catch (Exception err)
			{
				Debug.WriteLine("Error sending email: " + err.ToString());
				//Who you gonna go cry to now?
			}
		}
	}
}
