using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MMDB.DataService.Data.Dto;

namespace MMDB.DataService.Data.Tests
{
	public class JobManangerTests
	{
		[Test]
		public void CanCreateAJobDefinition()
		{
			using(var session = EmbeddedRavenProvider.DocumentStore.OpenSession())
			{
				var sut = new JobManager(session);
				string assemblyName = Guid.NewGuid().ToString();
				string className = Guid.NewGuid().ToString();
				string scheduleExpression = Guid.NewGuid().ToString();
				string jobName = Guid.NewGuid().ToString();
			
				var result = sut.CreateJob(jobName, assemblyName, className, scheduleExpression);
				Assert.IsNotNull(result);
				Assert.IsNotNullOrEmpty(result.Id);
				
				var dbItem = session.Load<JobDefinition>(result.Id);
				Assert.IsNotNull(dbItem);
				Assert.AreEqual(jobName, dbItem.JobName);
			}
		}
	}
}
