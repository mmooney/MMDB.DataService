using MMDB.DataService.Data.Dto.Email;
using MMDB.RazorEmail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace MMDB.DataService.Data
{
    public interface IEmailQueuer
    {
        EmailJobData SendEmail(EnumSettingSource settingsSource, string settingsKey, string subject, string body, List<string> toAddressList, string fromAddress, params EmailAttachmentData[] attachments);
        EmailJobData SendEmail(EnumSettingSource settingsSource, string settingsKey, string subject, string body, IEnumerable<MailAddress> toAddressList, MailAddress fromAddress, params EmailAttachmentData[] attachments);
        EmailJobData SendRazorEmail<T>(EnumSettingSource settingsSource, string settingsKey, string subject, T model, string razorView, IEnumerable<MailAddress> toAddressList, MailAddress fromAddress, params EmailAttachmentData[] attachments);
        EmailJobData SendRazorEmail<T>(EnumSettingSource settingsSource, string settingsKey, string subject, T model, string razorView, List<string> toAddressList, string fromAddress, params EmailAttachmentData[] attachments);
    }
}
