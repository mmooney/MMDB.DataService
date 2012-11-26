using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Dto
{
	public class JobSimpleSchedule : JobSchedule
	{
		public override JobScheduleType ScheduleType
		{
			get { return JobScheduleType.Simple; }
		}

		public int IntervalMinutes { get; set; }
		public int DelayStartMinutes { get; set; }
	}
}
