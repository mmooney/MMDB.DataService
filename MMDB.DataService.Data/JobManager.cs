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
using MMDB.DataService.Data.Dto;
using MMDB.DataService.Data.Jobs;

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

		public List<JobStatus> GetAllJobStatus()
		{
			var returnList = new List<JobStatus>();
			var jobDefinitionList = this.DocumentSession.Query<JobDefinition>();
			foreach(var jobDefinition in jobDefinitionList)
			{
				var jobAssembly = Assembly.Load(jobDefinition.AssemblyName.Replace(".dll",""));
				var jobType = jobAssembly.GetType(jobDefinition.ClassName);
				var queueInterface = jobType.GetInterfaces().SingleOrDefault(i=>i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueueJob<>));
				if(queueInterface != null)
				{
					var queueDataType = queueInterface.GetGenericArguments()[0];
					var entityName = this.DocumentSession.Advanced.DocumentStore.Conventions.GetTypeTagName(queueDataType);
					var itemQuery = this.DocumentSession.Advanced.LuceneQuery<object>().WhereEquals("@metadata.Raven-Entity-Name", entityName);

					var jobStatus = new JobStatus()
					{
						JobDefinition = jobDefinition,
						QueueDataType = queueDataType
					};
					var jobStatusCountQuery = (from i in itemQuery
											group itemQuery by ((JobData)i).Status into g
											select new 
											{
												Status = g.Key,
												Count = g.Count()
											});
					foreach(var jobStatusCountItem in jobStatusCountQuery)
					{
						jobStatus.StatusCountList.Add(jobStatusCountItem.Status, jobStatusCountItem.Count);
					}
					returnList.Add(jobStatus);
				}
			}
			return returnList;
		}
	}
}
