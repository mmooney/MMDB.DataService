using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace MMDB.DataEmail
{
	public class RazorEmailEngine
	{
		//private RazorHosting.RazorEngine< RazorEngine { get; set; }
		private EmailSender EmailSender { get; set; }

		public EmailServerSettings EmailServerSettings 
		{ 
			get
			{
				return this.EmailSender.EmailServerSettings;
			}
			set 
			{
				this.EmailSender.EmailServerSettings = value;
			}
		}


		public RazorEmailEngine(EmailSender emailSender)
		{
			this.EmailSender = emailSender;
		}

		public void SendEmail<T>(string subject, T model, string razorView, IEnumerable<MailAddress> toAddressList, MailAddress fromAddress, params EmailAttachementData[] attachments)
		{
			string body = RazorEngine.Razor.Parse<T>(razorView, model);
			this.EmailSender.SendEmail(subject, body, toAddressList, fromAddress, attachments);
		}

		public void SendEmail<T>(string subject, T model, string razorView, IEnumerable<string> toAddressList, string fromAddress, params EmailAttachementData[] attachments)
		{
			var to = toAddressList.Select(i=>new MailAddress(i));
			var from = new MailAddress(fromAddress);
			this.SendEmail(subject, model, razorView, to, from, attachments);
		}
	}
}
