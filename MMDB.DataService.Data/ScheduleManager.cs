using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto;
using Quartz;
using MMDB.DataService.Data.Dto.Jobs;
using Quartz.Impl;
using Quartz.Impl.Triggers;

namespace MMDB.DataService.Data
{
	public class ScheduleManager
	{
		private IScheduler Scheduler { get; set; }
		private JobManager JobManager { get; set; }
		private EventReporter EventReporter { get; set; }
		private TypeLoader TypeLoader { get; set; }

		public ScheduleManager(JobManager jobManager, IScheduler scheduler, EventReporter eventReporter, TypeLoader typeLoader)
		{
			this.JobManager = jobManager;
			this.Scheduler = scheduler;
			this.EventReporter = eventReporter;
			this.TypeLoader = typeLoader;
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
			this.EventReporter.Trace("Creating job {0}",job.JobName);
			if(job.Schedule is JobCronSchedule)
			{
			}
			else if(job.Schedule is JobSimpleSchedule) 
			{
				var type = this.TypeLoader.LoadType(job.AssemblyName, job.ClassName);
				var genericJobWrapperType = typeof(JobWrapper<>);
				var combinedJobType = genericJobWrapperType.MakeGenericType(type);
				var jobDetail = new JobDetailImpl(job.JobName, combinedJobType);
				var simpleSchedule = (JobSimpleSchedule)job.Schedule;
				ITrigger trigger;
				if (simpleSchedule.DelayStartMinutes != 0)
				{
					trigger = new SimpleTriggerImpl(job.JobName + "Trigger", DateBuilder.FutureDate(simpleSchedule.DelayStartMinutes,IntervalUnit.Minute), null, SimpleTriggerImpl.RepeatIndefinitely, TimeSpan.FromMinutes(simpleSchedule.IntervalMinutes));
				}
				else
				{
					trigger = new SimpleTriggerImpl(job.JobName + "Trigger", null, SimpleTriggerImpl.RepeatIndefinitely, TimeSpan.FromMinutes(simpleSchedule.IntervalMinutes));
				}
				this.Scheduler.ScheduleJob(jobDetail, trigger);
			}
			this.EventReporter.Trace("Done Creating " + job.JobName);
		}
	}
}
