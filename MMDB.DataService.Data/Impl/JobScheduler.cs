using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Jobs;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using Tamir.SharpSsh.java.lang;

namespace MMDB.DataService.Data.Impl
{
	public class JobScheduler : IJobScheduler
	{
		private IScheduler Scheduler { get; set; }
		private IJobManager JobManager { get; set; }
		private IEventReporter EventReporter { get; set; }
		private ITypeLoader TypeLoader { get; set; }

		public JobScheduler(IJobManager jobManager, IScheduler scheduler, IEventReporter eventReporter, ITypeLoader typeLoader)
		{
			this.JobManager = jobManager;
			this.Scheduler = scheduler;
			this.EventReporter = eventReporter;
			this.TypeLoader = typeLoader;
		}

		public virtual void StartJobs()
		{
			var jobDefintionList = this.JobManager.LoadJobList();
			foreach (var jobDefinition in jobDefintionList)
			{
				this.StartJob(jobDefinition);
			}
			this.Scheduler.StartDelayed(TimeSpan.FromMilliseconds(100));
		}

		public void StopJobs()
		{
			this.Scheduler.Shutdown(true);
		}

		public void RunJobNow(int jobID)
		{
			var jobDefinition = this.JobManager.GetJobDefinition(jobID);
			this.StartJob(jobDefinition, runNow: true);
			this.Scheduler.Start();
			Thread.Sleep(1000);
			while (this.Scheduler.GetCurrentlyExecutingJobs().Count > 0)
			{
				Thread.Sleep(100);
			}
			this.StopJobs();
		}


		private void StartJob(JobDefinition jobDefinition, bool runNow = false)
		{
			this.EventReporter.Trace("Creating " + jobDefinition.JobName);
			var jobType = this.TypeLoader.LoadType(jobDefinition.AssemblyName, jobDefinition.ClassName);
			var configType = jobType.BaseType.GetGenericArguments()[0];
			var wrapperType = typeof(JobWrapper<,>).MakeGenericType(jobType, configType);
			var jobDetail = new JobDetailImpl(jobDefinition.JobName, wrapperType);
			jobDetail.JobDataMap.Add("Configuration", jobDefinition.Configuration);

			if (runNow)
			{
				var trigger = new SimpleTriggerImpl(jobDefinition.JobName + "Trigger", DateBuilder.FutureDate(0, IntervalUnit.Minute), null, 1, TimeSpan.FromMinutes(int.MaxValue));
				this.Scheduler.ScheduleJob(jobDetail, trigger);
			}
			else if (jobDefinition.Schedule is JobSimpleSchedule)
			{
				var schedule = (JobSimpleSchedule)jobDefinition.Schedule;
				var trigger = new SimpleTriggerImpl(jobDefinition.JobName + "Trigger", DateBuilder.FutureDate(schedule.DelayStartMinutes, IntervalUnit.Minute), null, SimpleTriggerImpl.RepeatIndefinitely, TimeSpan.FromMinutes(schedule.IntervalMinutes));
				this.Scheduler.ScheduleJob(jobDetail, trigger);
			}
			else if (jobDefinition.Schedule is JobCronSchedule)
			{
				var schedule = (JobCronSchedule)jobDefinition.Schedule;
				var trigger = new CronTriggerImpl(jobDefinition.JobName + "Trigger", jobDefinition.JobName + "Group", schedule.CronScheduleExpression);
				this.Scheduler.ScheduleJob(jobDetail, trigger);
			}
			this.EventReporter.Trace("Done Creating " + jobDefinition.JobName);
		}
	}
}
