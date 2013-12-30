﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using MMDB.Data.DataService;
using MMDB.DataService.Data.Settings;
using MMDB.RazorEmail;

namespace MMDB.DataService.Data.Impl
{
	public class DataServiceEmailSender : IDataServiceEmailSender
	{
		private IConnectionSettingsManager ConnectionSettingsManager { get; set; }
		private RazorEmailEngine EmailEngine { get; set; }

		public DataServiceEmailSender(IConnectionSettingsManager connectionSettingsManger, RazorEmailEngine emailEngine)
		{
			this.ConnectionSettingsManager = connectionSettingsManger;
			this.EmailEngine = emailEngine;
		}

		public void SendRazorEmail<T>(EnumSettingSource settingsSource, string settingsKey, string subject, T model, string razorView, IEnumerable<MailAddress> toAddressList, MailAddress fromAddress, params EmailAttachmentData[] attachments)
		{
			var emailConnectionSettings = this.ConnectionSettingsManager.Load<EmailConnectionSettings>(settingsSource, settingsKey);
			var emailServerSettings = new EmailServerSettings
			{
				Host = emailConnectionSettings.Host,
				Port = emailConnectionSettings.Port,
				UserName = emailConnectionSettings.UserName,
				Password = emailConnectionSettings.Password
			};
			string overrideEmailAddress = ConfigurationManager.AppSettings["OverrideAllEmailAddress"];
			if(!string.IsNullOrWhiteSpace(overrideEmailAddress))
			{
				toAddressList = new List<MailAddress>()
				{
					new MailAddress(overrideEmailAddress)
				};
			}
            try 
            {
    			this.EmailEngine.SendEmail(emailServerSettings, subject, model, razorView, toAddressList, fromAddress, attachments);
            }
            catch(RazorEngine.Templating.TemplateCompilationException ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Error(s) compiling template for email: ");
                foreach(var item in ex.Errors)
                {
                    sb.AppendLine("-" + item.Line.ToString() + ": " + item.ErrorText);
                }
                throw new Exception(sb.ToString(), ex);
            }
		}

		public void SendRazorEmail<T>(EnumSettingSource settingsSource, string settingsKey, string subject, T model, string razorView, List<string> toAddressList, string fromAddress, params EmailAttachmentData[] attachments)
		{
			var toList = toAddressList.Select(i => new MailAddress(i)).ToList();
			var from = new MailAddress(fromAddress);
			this.SendRazorEmail(settingsSource, settingsKey, subject, model, razorView, toList, from, attachments);
		}


		public void SendRazorEmail<T>(EmailServerSettings emailSettings, string subject, T model, string razorView, List<string> toAddressList, string fromAddress, params EmailAttachmentData[] attachments)
		{
			string overrideEmailAddress = ConfigurationManager.AppSettings["OverrideAllEmailAddress"];
			if (!string.IsNullOrWhiteSpace(overrideEmailAddress))
			{
				toAddressList = new List<string>
				{
					overrideEmailAddress
				};
			}
			this.EmailEngine.SendEmail(emailSettings, subject, model, razorView, toAddressList, fromAddress, attachments); 
		}

		public void SendRazorEmail<T>(EmailConnectionSettings emailSettings, string subject, T model, string razorView, List<string> toAddressList, string fromAddress, params EmailAttachmentData[] attachments)
		{
			var emailServerSettings = new  EmailServerSettings
			{
				Host = emailSettings.Host,
				Password = emailSettings.Password,
				Port = emailSettings.Port,
				UserName = emailSettings.UserName
			};
			this.SendRazorEmail(emailServerSettings, subject, model, razorView, toAddressList, fromAddress, attachments);
		}
	}
}
