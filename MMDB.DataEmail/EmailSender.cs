using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace MMDB.DataEmail
{
	public class EmailSender
	{
		private EmailServerSettings EmailServerSettings { get; set; }

		public EmailSender(EmailServerSettings emailServerSettings)
		{
			this.EmailServerSettings = emailServerSettings;
		}

		public virtual void SendEmail(string subject, string body, IEnumerable<MailAddress> toAddressList, MailAddress fromAddress)
		{
			using(SmtpClient smtpClient = new SmtpClient(this.EmailServerSettings.Host, this.EmailServerSettings.Port))
			{
				var message = new MailMessage();
				message.From = fromAddress;
				message.Body = body;
				message.Subject = subject;
				foreach(var toAddress in toAddressList)
				{
					message.To.Add(toAddress);
				}
				smtpClient.Send(message);
			}
		} 
	}
}
