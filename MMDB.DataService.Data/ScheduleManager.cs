using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto;
using Quartz;

namespace MMDB.DataService.Data
{
	public class ScheduleManager
	{
		private IScheduler Scheduler { get; set; }
		private JobManager JobManager { get; set; }

		public ScheduleManager(JobManager jobManager, IScheduler scheduler)
		{
			this.JobManager = jobManager;
			this.Scheduler = scheduler;
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
