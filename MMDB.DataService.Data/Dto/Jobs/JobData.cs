using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Dto.Jobs
{
	public abstract class JobData
	{
		public string Id { get; set; }
		public List<string> ExceptionIdList { get; set; }
		public EnumJobStatus Status { get; set; }

		public JobData()
		{
			this.ExceptionIdList = new List<string>();
		}
	}
}
