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

		public RazorEmailEngine(RazorEngine razorEngine, EmailSender emailSender)
		{
			this.RazorEngine = razorEngine;
			this.EmailSender = emailSender;
		}

		public void SendEmail<T>(string subject, T model, string razorView, IEnumerable<MailAddress> toAddressList, MailAddress fromAddress)
		{
			this.RazorEngine.AddAssemblyFromType(typeof(T));
			string body = this.RazorEngine.RenderTemplate(razorView, model);

			this.EmailSender.SendEmail(subject, body, toAddressList, fromAddress);
		}
	}
}
