using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.DataProvider;
using MMDB.DataService.Data.Dto;
using Raven.Client;
using MMDB.DataService.Data.Dto.Jobs;

namespace MMDB.DataService.Data
{
	public class JobManager
	{
		private IDocumentSession DocumentSession { get; set; } 

		public JobManager(IDocumentSession documentSession)
		{
			this.DocumentSession = documentSession;
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
			return item;
		}
		public List<JobDefinition> LoadJobList()
		{
			return this.DocumentSession.Query<JobDefinition>().ToList();
		}

	}
}
