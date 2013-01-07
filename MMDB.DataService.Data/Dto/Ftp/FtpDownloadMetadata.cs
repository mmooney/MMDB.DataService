using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Jobs;

namespace MMDB.DataService.Data.Dto.Ftp
{
	public class FtpDownloadMetadata
	{
		public string Directory { get; set; }
		public string FileName { get; set; }
		public FtpDownloadSettings Settings { get; set; }
	}
}
