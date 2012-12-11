using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Dto.Ftp
{
	public class FtpOutboundData
	{
		public string Id { get; set; }
		public EnumSettingSource SettingSource { get; set; }
		public string SettingKey { get; set; }
		//public string FtpUserName { get; set; }
		//public string FtpPassword { get; set; }
		//public string FtpHost { get; set; }
		public string TargetDirectory { get; set; }
		public string TargetFileName { get; set; }
		public string AttachementId { get; set; }
		public DateTime QueuedDateTimeUtc { get; set; }
		public  EnumJobStatus Status { get; set; }}
}
