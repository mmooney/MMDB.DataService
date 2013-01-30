using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Jobs
{
	public class FtpDownloadJobConfiguration : JobConfigurationBase
	{
		public List<FtpDownloadSettings> FtpDownloadSettings { get; set; }
			
		public FtpDownloadJobConfiguration()
		{
			this.FtpDownloadSettings = new List<FtpDownloadSettings>();
		}
	}
}
