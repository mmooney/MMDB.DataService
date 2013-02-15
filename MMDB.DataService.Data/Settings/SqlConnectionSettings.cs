using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Settings
{
	public class SqlConnectionSettings : ConnectionSettingBase
	{
		public string ConnectionString { get; set; }
	}
}
