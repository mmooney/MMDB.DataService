using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Dto
{
	public class JobCronSchedule : JobSchedule
	{
		public override JobScheduleType ScheduleType
		{
			get { return JobScheduleType.Cron; }
		}

		public string CronScheduleExpression { get; set; }
	}
}
