using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Dto
{
	public abstract class JobSchedule
	{
		public abstract JobScheduleType ScheduleType { get; }
	}
}
