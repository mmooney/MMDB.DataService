using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace MMDB.DataEmail
{
	public class EmailSender
	{
		public EmailServerSettings EmailServerSettings { get; set; }

		public EmailSender()
		{
		}

		public EmailSender(EmailServerSettings emailServerSettings)
		{
			this.EmailServerSettings = emailServerSettings;
		}

		public virtual void SendEmail(string subject, string body, IEnumerable<string> toAddressList, string fromAddress, params EmailAttachementData[] attachments)
		{
			var toList = toAddressList.Select(i=>new MailAddress(i));
			var from = new MailAddress(fromAddress);
			this.SendEmail(subject, body, toList, from, attachments);
		}

		public virtual void SendEmail(string subject, string body, IEnumerable<MailAddress> toAddressList, MailAddress fromAddress, params EmailAttachementData[] attachments)
		{
			int port = this.EmailServerSettings.Port.GetValueOrDefault(25);
			using (var smtpClient = new SmtpClient(this.EmailServerSettings.Host, port))
			{
				if (!string.IsNullOrEmpty(this.EmailServerSettings.UserName))
				{
					var credential = new NetworkCredential(this.EmailServerSettings.UserName, this.EmailServerSettings.Password);
					credential.GetCredential(smtpClient.Host, port, "Basic");
					smtpClient.UseDefaultCredentials = false;
					smtpClient.Credentials = credential;
				}
				
				var message = new MailMessage();
				message.From = fromAddress;
				message.Body = body;
				message.Subject = subject;
				message.IsBodyHtml = true;
				foreach(var toAddress in toAddressList)
				{
					message.To.Add(toAddress);
				}
				smtpClient.Send(message);
			}
		} 
	}
}
