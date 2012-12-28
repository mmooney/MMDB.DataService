using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Dto.Jobs
{
	public class JobCronSchedule : JobSchedule
	{
		public override string DisplayValue
		{
			get
			{
				return "Cron Job: " + this.CronScheduleExpression;
			}
		}

		public string CronScheduleExpression { get; set; }
	}
}
