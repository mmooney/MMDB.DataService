using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Jobs;
using Raven.Client.UniqueConstraints;

namespace MMDB.DataService.Data.Dto.Ftp
{
	public class FtpOutboundData : JobData
	{
		public EnumSettingSource SettingSource { get; set; }
		public string SettingKey { get; set; }
		public string TargetDirectory { get; set; }
		[UniqueConstraint] public string TargetFileName { get; set; }
		public string AttachmentId { get; set; }
		public DateTime QueuedDateTimeUtc { get; set; }
	}
}
