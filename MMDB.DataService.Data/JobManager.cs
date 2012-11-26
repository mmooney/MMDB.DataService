using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.DataProvider;
using MMDB.DataService.Data.Dto;
using Raven.Client;

namespace MMDB.DataService.Data
{
	public class JobManager
	{
		private IDocumentSession DocumentSession { get; set; } 

		public JobManager(IDocumentSession documentSession)
		{
			this.DocumentSession = documentSession;
		}

		public JobDefinition CreateJob(string jobName, string assemblyName, string className, string scheduleExpression)
		{
			var item = new JobDefinition
			{
				JobName = jobName,
				AssemblyName = assemblyName,
				ClassName = className, 
				ScheduleExpression = scheduleExpression
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
