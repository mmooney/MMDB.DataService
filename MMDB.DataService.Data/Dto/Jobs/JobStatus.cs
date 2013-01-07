using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Dto.Jobs
{
	public class JobStatus
	{
		public JobDefinition JobDefinition { get; set; }
		public Type QueueDataType { get; set; }
		public Dictionary<EnumJobStatus, int> StatusCountList { get; set; }

		public JobStatus()
		{
			this.StatusCountList = new Dictionary<EnumJobStatus,int>();
		}

		public int GetStatusCount(EnumJobStatus status)
		{
			int returnValue;
			if(!this.StatusCountList.TryGetValue(status, out returnValue))
			{
				returnValue = 0;
			}
			return returnValue;
		}
	}
}
