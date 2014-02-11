using MMDB.DataService.Data.Dto.Jobs;
using MMDB.RazorEmail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Dto.Email
{
    public class EmailJobData : JobData
    {
        public EnumSettingSource SettingsSource { get; set; }
        public string SettingsKey { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }
        public List<DataServiceMailAddress> ToAddressList { get; set; }
        public DataServiceMailAddress FromAddress { get; set; }
        public List<EmailAttachmentData> Attachments { get; set; }

        public EmailJobData()
        {
            this.Attachments = new List<EmailAttachmentData>();
        }
    }
}
