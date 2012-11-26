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

				session.Delete(dbItem);
				session.SaveChanges();
			}
		}

		[Test]
		public void CanEditAJobDefinition()
		{
			using (var session = EmbeddedRavenProvider.DocumentStore.OpenSession())
			{
				var sut = new JobManager(session);
				string assemblyName = Guid.NewGuid().ToString();
				string className = Guid.NewGuid().ToString();
				string scheduleExpression = Guid.NewGuid().ToString();
				string jobName = Guid.NewGuid().ToString();
				var newJob = sut.CreateJob(jobName, assemblyName, className, scheduleExpression);

				newJob.AssemblyName = Guid.NewGuid().ToString();
				newJob.ClassName = Guid.NewGuid().ToString();
				newJob.ScheduleExpression = Guid.NewGuid().ToString();
				newJob.JobName = Guid.NewGuid().ToString();

				session.SaveChanges();

				var dbItem = session.Load<JobDefinition>(newJob.Id);
				Assert.IsNotNull(dbItem);
				Assert.AreEqual(newJob.JobName, dbItem.JobName);

				session.Delete(dbItem);
				session.SaveChanges();
			}
		}

		[Test]
		public void LoadJobList()
		{
			using(var session = EmbeddedRavenProvider.DocumentStore.OpenSession())
			{
				var sut = new JobManager(session);
				var list = sut.LoadJobList();
				Assert.AreEqual(0, list.Count());
			}
		}
	}
}
