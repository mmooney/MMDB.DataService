using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using MMDB.DataService.Data;
using MMDB.DataService.Data.Settings;
using MMDB.RazorEmail;

namespace MMDB.Data.DataService
{
	public interface IDataServiceEmailSender
	{
		void SendRazorEmail<T>(EnumSettingSource settingsSource, string settingsKey, string subject, T model, string razorView, IEnumerable<MailAddress> toAddressList, MailAddress fromAddress, params EmailAttachmentData[] attachments);
		void SendRazorEmail<T>(EnumSettingSource settingsSource, string settingsKey, string subject, T model, string razorView, List<string> toAddressList, string fromAddress, params EmailAttachmentData[] attachments);
		void SendRazorEmail<T>(EmailServerSettings emailSettings, string subject, T model, string razorView, List<string> toAddressList, string fromAddress, params EmailAttachmentData[] attachments);
		void SendRazorEmail<T>(EmailConnectionSettings emailSettings, string subject, T model, string razorView, List<string> toAddressList, string fromAddress, params EmailAttachmentData[] attachments);
	}
}
