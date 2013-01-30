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
		private TypeLoader TypeLoader { get; set; }

		public JobManager(IDocumentSession documentSession, EventReporter eventReporter, IScheduler scheduler, TypeLoader typeLoader)
		{
			this.DocumentSession = documentSession;
			this.EventReporter = eventReporter;
			this.Scheduler = scheduler;
			this.TypeLoader = typeLoader;
		}

		public virtual void StartJobs()
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
			var jobType = this.TypeLoader.LoadType(jobDefinition.AssemblyName, jobDefinition.ClassName);
			var configType = jobType.GetGenericArguments()[0];
			var wrapperType = typeof(JobWrapper<,>).MakeGenericType(jobType, configType);
			var jobDetail = new JobDetailImpl(jobDefinition.JobName, wrapperType);
			jobDetail.JobDataMap.Add("Configuration", jobDefinition.Configuration);

			if(jobDefinition.Schedule is JobSimpleSchedule)
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


		public JobDefinition CreateSimpleJob(string jobName, Guid jobGuid, string assemblyName, string className, int intervalMinutes, int delayStartMinutes)
		{
			if(jobGuid == Guid.Empty)
			{
				jobGuid = Guid.NewGuid();
			}
			var item = new JobDefinition
			{
				JobName = jobName,
				JobGuid = jobGuid,
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

		public JobDefinition CreateCronJob(string jobName, Guid jobGuid, string assemblyName, string className, string cronScheduleExpression)
		{
			if(jobGuid == Guid.Empty)
			{
				jobGuid = Guid.NewGuid();
			}
			var item = new JobDefinition
			{
				JobName = jobName,
				JobGuid = jobGuid,
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


		public JobDefinition LoadJobDefinition(int id)
		{
			return this.DocumentSession.Load<JobDefinition>(id);
		}

		public void UpdateSimpleJob(int id, string jobName, Guid jobGuid, string assemblyName, string className, int intervalMinutes, int delayStartMinutes, JobConfigurationBase configuration = null)
		{
			if(jobGuid == Guid.Empty)
			{
				jobGuid = Guid.NewGuid();
			}
			var item = this.LoadJobDefinition(id);
			item.JobName = jobName;
			item.AssemblyName = assemblyName;
			item.ClassName = className;
			item.JobGuid = jobGuid;
			var schedule = item.Schedule as JobSimpleSchedule;
			if(schedule == null)
			{
				item.Schedule = schedule = new JobSimpleSchedule();
			}
			schedule.IntervalMinutes = intervalMinutes;
			schedule.DelayStartMinutes = delayStartMinutes;
			if(configuration != null)
			{
				item.Configuration = configuration;
			}
			this.DocumentSession.SaveChanges();
		}

		public void UpdateCronJob(int id, string jobName, Guid jobGuid, string assemblyName, string className, string cronScheduleExpression, JobConfigurationBase configuration=null)
		{
			if(jobGuid == Guid.Empty)
			{
				jobGuid = Guid.NewGuid();
			}
			var item = this.LoadJobDefinition(id);
			item.JobName = jobName;
			item.AssemblyName = assemblyName;
			item.ClassName = className;
			item.JobGuid = jobGuid;
			var schedule = item.Schedule as JobCronSchedule;
			if (schedule == null)
			{
				item.Schedule = schedule = new JobCronSchedule();
			}
			schedule.CronScheduleExpression = cronScheduleExpression;
			if(configuration != null)
			{
				item.Configuration = configuration;
			}
			this.DocumentSession.SaveChanges();
		}

		public void DeleteJobDefinition(int id)
		{
			var job = this.LoadJobDefinition(id);
			this.DocumentSession.Delete(job);
			this.DocumentSession.SaveChanges();
		}

		public List<JobStatus> GetAllJobStatus()
		{
			var returnList = new List<JobStatus>();
			var jobDefinitionList = this.DocumentSession.Query<JobDefinition>();
			foreach(var jobDefinition in jobDefinitionList)
			{
				Type queueDataType = null;
				queueDataType = TryGetQueueDataType(jobDefinition);

				if(queueDataType != null)
				{
					var itemQuery = GetJobDataQueue(queueDataType);

					var jobStatus = new JobStatus()
					{
						JobDefinition = jobDefinition,
						QueueDataType = queueDataType
					};
					var jobStatusCountQuery = (from i in itemQuery
											group itemQuery by i.Status into g
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


		public JobDefinition GetJobDefinition(int jobDefinitionId)
		{
			return this.DocumentSession.Load<JobDefinition>(jobDefinitionId);
		}

		public IQueryable<JobData> GetJobDataQueue(int jobDefinitionId)
		{
			var jobDefinition = this.GetJobDefinition(jobDefinitionId);
			return GetJobDataQueue(jobDefinition);
		}

		public IQueryable<JobData> GetJobDataQueue(JobDefinition jobDefinition)
		{
			var queueType = TryGetQueueDataType(jobDefinition);
			if(queueType == null)
			{
				throw new Exception(jobDefinition.JobName + " does not have a queue data type");
			}
			return GetJobDataQueue(queueType);
		}

		private IQueryable<JobData> GetJobDataQueue(Type queueDataType)
		{
			var entityName = this.DocumentSession.Advanced.DocumentStore.Conventions.GetTypeTagName(queueDataType);
			var itemQuery = this.DocumentSession.Advanced.LuceneQuery<object>().WhereEquals("@metadata.Raven-Entity-Name", entityName).Select(i => (JobData)i);
			return itemQuery.AsQueryable();
		}

		private Type TryGetQueueDataType(JobDefinition jobDefinition)
		{
			Type queueDataType = null;
			var jobType = this.TypeLoader.LoadType(jobDefinition.AssemblyName, jobDefinition.ClassName);
			//var jobAssembly = Assembly.Load(jobDefinition.AssemblyName.Replace(".dll", ""));
			//var jobType = jobAssembly.GetType(jobDefinition.ClassName);
			var queueInterface = jobType.GetInterfaces().SingleOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(QueueJobBase<,>));
			if (queueInterface != null)
			{
				queueDataType = queueInterface.GetGenericArguments()[0];
			}
			return queueDataType;
		}


		public void SaveChanges()
		{
			this.DocumentSession.SaveChanges();
		}

		public void DeleteJobData(JobData item)
		{
			this.DocumentSession.Delete(item);
			this.DocumentSession.SaveChanges();
		}

		public void UpdateJobDataStatus(JobData item, EnumJobStatus status)
		{
			item.Status = status;
			this.DocumentSession.SaveChanges();
		}

		public void EnsureConfiguration(JobDefinition jobDefinition)
		{
			if (jobDefinition.Configuration == null)
			{
				var jobType = this.TypeLoader.LoadType(jobDefinition.AssemblyName, jobDefinition.ClassName);
				if (jobType != null)
				{
					var configType = this.GetJobConfigurationType(jobType);
					if (configType != null && configType != typeof(NullJobConfiguration))
					{
						jobDefinition.Configuration = (JobConfigurationBase)Activator.CreateInstance(configType);
						this.DocumentSession.SaveChanges();
					}
				}
			}
		}

		public Type GetJobConfigurationType(Type jobType)
		{
			var baseType = FindDataServiceJobBase(jobType);
			if (baseType != null)
			{
				return baseType.GetGenericArguments()[0];
			}
			else 
			{
				return null;
			}
		}

		private Type FindDataServiceJobBase(Type jobType)
		{
			if(jobType.BaseType.IsGenericType &&  jobType.BaseType.GetGenericTypeDefinition() == typeof(DataServiceJobBase<>))
			{
				return jobType.BaseType;
			}
			else if (jobType.BaseType == typeof(object))
			{
				return null;
			}
			else 
			{
				return FindDataServiceJobBase(jobType.BaseType);
			}
		}
	}
}
