using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Jobs;
using MMDB.DataService.Data.Settings;

namespace MMDB.DataService.Data.Jobs
{
	public class FtpDownloadServiceSettings : SettingsBase
	{
		public List<FtpDownloadSettings> FtpDownloadSettings { get; set; }
	}
}
