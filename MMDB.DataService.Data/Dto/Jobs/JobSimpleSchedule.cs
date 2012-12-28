using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;

namespace MMDB.DataService.Data.Dto.Jobs
{
	public class JobSimpleSchedule : JobSchedule
	{
		public override string DisplayValue
		{
			get 
			{ 
				string returnValue = string.Format("Every {0} minutes", this.IntervalMinutes);
				if(this.DelayStartMinutes > 0)
				{
					returnValue += string.Format(", delay start {0} minutes", this.DelayStartMinutes);
				}
				return returnValue;
			}
		}

		public int IntervalMinutes { get; set; }
		public int DelayStartMinutes { get; set; }
	}
}
