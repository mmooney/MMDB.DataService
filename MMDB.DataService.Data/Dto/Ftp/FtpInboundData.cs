﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Jobs;

namespace MMDB.DataService.Data.Dto.Ftp
{
	public class FtpInboundData : JobData
	{
		public string Directory { get; set; }
		public string FileName { get; set; }
		public string AttachmentId { get; set; }
		public DateTime QueuedDateTimeUtc { get; set; }
		public string InboundQueueIdentifier { get; set; }
		public bool AttachmentPurged { get; set; }
		public DateTime AttachmentPurgedDateTimeUtc { get; set; }
	}
}
