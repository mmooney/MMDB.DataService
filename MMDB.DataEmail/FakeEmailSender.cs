using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace MMDB.DataEmail
{
	public class FakeEmailSender : EmailSender
	{
		public FakeEmailSender() : base(new EmailServerSettings())
		{

		}

		public override void SendEmail(string subject, string body, IEnumerable<string> toAddressList, string fromAddress, params EmailAttachementData[] attachments)
		{
			//Do Nothing
		}


		public override void SendEmail(string subject, string body, IEnumerable<MailAddress> toAddressList, MailAddress fromAddress, params EmailAttachementData[] attachments)
		{
			//Do Nothing
		}
	}
}
