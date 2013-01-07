using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataEmail
{
	public class FakeEmailSender : EmailSender
	{
		public FakeEmailSender() : base(new EmailServerSettings())
		{

		}

		public override void SendEmail(string subject, string body, IEnumerable<string> toAddressList, string fromAddress)
		{
			//Do Nothing
		}

		public override void SendEmail(string subject, string body, IEnumerable<System.Net.Mail.MailAddress> toAddressList, System.Net.Mail.MailAddress fromAddress)
		{
			//Do Nothing
		}
	}
}
