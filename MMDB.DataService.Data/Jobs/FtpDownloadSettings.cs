using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto;

namespace MMDB.DataService.Data.Jobs
{
	public class FtpDownloadSettings
	{
		public EnumSettingSource SettingSource { get; set; }
		public string SettingKey { get; set; }
		public string DownloadDirectory { get; set; }
	}
}
