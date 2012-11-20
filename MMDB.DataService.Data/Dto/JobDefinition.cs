using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Dto
{
	public class JobDefinition
	{
		public string Id { get; set; }
		public string JobName { get; set; }
		public string AssemblyName { get; set; }
		public string ClassName { get; set; }
		public string ScheduleExpression { get; set; }
	}
}
