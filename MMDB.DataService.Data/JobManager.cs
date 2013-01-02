using System;
using System.Collections.Generic;
using System.Linq;
using MMDB.DataService.Data.Dto.Assemblies;
using MMDB.DataService.Data.Dto.Jobs;
using Quartz;
using Quartz.Impl;
using Raven.Client;
using Quartz.Impl.Triggers;
using System.Reflection;

namespace MMDB.DataService.Data
{
	public class JobManager
	{
		private IDocumentSession DocumentSession { get; set; } 
		private EventReporter EventReporter { get; set; }
		private IScheduler Scheduler { get; set; }

		public JobManager(IDocumentSession documentSession, EventReporter eventReporter, IScheduler scheduler)
		{
			this.DocumentSession = documentSession;
			this.EventReporter = eventReporter;
			this.Scheduler = scheduler;
		}

		public void StartJobs()
		{
			var jobDefintionList = this.DocumentSession.Query<JobDefinition>();
			foreach(var jobDefinition in jobDefintionList)
			{
				this.StartJob(jobDefinition);
			}
			this.Scheduler.Start();
		}

		private void StartJob(JobDefinition jobDefinition)
		{
			this.EventReporter.Trace("Creating " + jobDefinition.JobName);
			var jobAssembly = Assembly.LoadFrom(jobDefinition.AssemblyName);
			var jobType = jobAssembly.GetType(jobDefinition.ClassName);
			var wrapperType = typeof(JobWrapper<>).MakeGenericType(jobType);
			var jobDetail = new JobDetailImpl(jobDefinition.JobName, wrapperType);

			if(jobDefinition.Schedule is JobSimpleSchedule)
			{
				var schedule = (JobSimpleSchedule)jobDefinition.Schedule;
				var trigger = new SimpleTriggerImpl(jobDefinition.JobName + "Trigger", DateBuilder.FutureDate(schedule.DelayStartMinutes, IntervalUnit.Minute), null, SimpleTriggerImpl.RepeatIndefinitely, TimeSpan.FromMinutes(schedule.IntervalMinutes));
				this.Scheduler.ScheduleJob(jobDetail, trigger);
			}
			else if (jobDefinition.Schedule is JobCronSchedule)
			{
				//string jobTypeName = typeof(JobType).Name;
				//this.Logger.Trace("Creating " + jobTypeName);
				//var jobDetail = new JobDetailImpl(jobDefinition.JobName, typeof(ServiceJob<JobType>));
				//var trigger = new CronTriggerImpl(jobTypeName + "Trigger", jobTypeName + "Group", cronExpression);
				//this.Scheduler.ScheduleJob(jobDetail, trigger);
				//this.Logger.Trace("Done Creating " + jobTypeName);
			}
			this.EventReporter.Trace("Done Creating " + jobDefinition.JobName);
		}


		public JobDefinition CreateSimpleJob(string jobName, string assemblyName, string className, int intervalMinutes, int delayStartMinutes)
		{
			var item = new JobDefinition
			{
				JobName = jobName,
				AssemblyName = assemblyName,
				ClassName = className,
				Schedule = new JobSimpleSchedule
				{
					IntervalMinutes = intervalMinutes,
					DelayStartMinutes = delayStartMinutes
				}
			};
			this.DocumentSession.Store(item);
			this.DocumentSession.SaveChanges();
			return item;
		}

		public JobDefinition CreateCronJob(string jobName, string assemblyName, string className, string cronScheduleExpression)
		{
			var item = new JobDefinition
			{
				JobName = jobName,
				AssemblyName = assemblyName,
				ClassName = className, 
				Schedule  = new JobCronSchedule
				{
					CronScheduleExpression = cronScheduleExpression
				}
			};
			this.DocumentSession.Store(item);
			this.DocumentSession.SaveChanges();
			return item;
		}
		public List<JobDefinition> LoadJobList()
		{
			return this.DocumentSession.Query<JobDefinition>().ToList();
		}


		public List<JobAssembly> LoadAssemblyList()
		{
			return this.DocumentSession.Query<JobAssembly>().ToList();
		}


		public JobDefinition LoadJob(int id)
		{
			return this.DocumentSession.Load<JobDefinition>(id);
		}

		public void UpdateSimpleJob(int id, string jobName, string assemblyName, string className, int intervalMinutes, int delayStartMinutes)
		{
			var item = this.LoadJob(id);
			item.JobName = jobName;
			item.AssemblyName = assemblyName;
			item.ClassName = className;
			var schedule = item.Schedule as JobSimpleSchedule;
			if(schedule == null)
			{
				item.Schedule = schedule = new JobSimpleSchedule();
			}
			schedule.IntervalMinutes = intervalMinutes;
			schedule.DelayStartMinutes = delayStartMinutes;
			this.DocumentSession.SaveChanges();
		}

		public void UpdateCronJob(int id, string jobName, string assemblyName, string className, string cronScheduleExpression)
		{
			var item = this.LoadJob(id);
			item.JobName = jobName;
			item.AssemblyName = assemblyName;
			item.ClassName = className;
			var schedule = item.Schedule as JobCronSchedule;
			if (schedule == null)
			{
				item.Schedule = schedule = new JobCronSchedule();
			}
			schedule.CronScheduleExpression = cronScheduleExpression;
			this.DocumentSession.SaveChanges();
		}

		public void DeleteJob(int id)
		{
			var job = this.LoadJob(id);
			this.DocumentSession.Delete(job);
			this.DocumentSession.SaveChanges();
		}
	}
}
