using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Jobs;

namespace MMDB.DataService.Data.Dto.Jobs
{
	public class JobDefinition
	{
		public int Id { get; set; }
		public string JobName { get; set; }
		public string AssemblyName { get; set; }
		public string ClassName { get; set; }
		public JobSchedule Schedule { get; set; }
		public JobConfigurationBase Configuration { get; set; }
	}
}
