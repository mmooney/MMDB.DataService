using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Westwind.RazorHosting;
using System.Net.Mail;

namespace MMDB.DataEmail
{
	public class RazorEmailEngine
	{
		private RazorEngine RazorEngine { get; set; }
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


		public RazorEmailEngine(RazorEngine razorEngine, EmailSender emailSender)
		{
			this.RazorEngine = razorEngine;
			this.EmailSender = emailSender;
		}

		public void SendEmail<T>(string subject, T model, string razorView, IEnumerable<MailAddress> toAddressList, MailAddress fromAddress, params EmailAttachementData[] attachments)
		{
			this.RazorEngine.AddAssemblyFromType(typeof(T));
			string body = this.RazorEngine.RenderTemplate(razorView, model);
			if(body == null)
			{
				throw new Exception(this.RazorEngine.ErrorMessage);
			}
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
