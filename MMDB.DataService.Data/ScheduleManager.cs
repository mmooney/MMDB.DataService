using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data
{
	public class ScheduleManager
	{
		private JobManager JobManager { get; set; }

		public ScheduleManager(JobManager jobManager)
		{
			this.JobManager = jobManager;
		}
		
		public void StartJobs()
		{
			var jobList = this.JobManager.LoadJobList();
			foreach(var job in jobList)
			{
				this.StartJob(job);
			}	
		}

		private void StartJob(JobDefinition job)
		{
			throw new NotImplementedException();
		}
	}
}
