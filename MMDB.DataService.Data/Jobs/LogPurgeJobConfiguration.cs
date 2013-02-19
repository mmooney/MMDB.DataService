using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Jobs
{
	public class LogPurgeJobConfiguration : JobConfigurationBase
	{
		public int? DebugAgeMinutes { get; set; }
		public int? TraceAgeMinutes { get; set; }
		public int? InfoAgeMinutes { get; set; }
		public int? WarningAgeMinutes { get; set; }
		public int? ErrorAgeMinutes { get; set; }
		public DateTime? UtcNow { get; set; }
	}
}
