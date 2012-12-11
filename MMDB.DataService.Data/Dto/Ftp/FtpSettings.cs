using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Dto.Ftp
{
	public class FtpSettings : SettingBase
	{
		public string FtpUserName { get; set; }
		public string FtpPassword { get; set; }
		public string FtpHost { get; set; }
	}
}
