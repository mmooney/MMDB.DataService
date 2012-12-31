using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MMDB.DataService.Data.Dto;
using MMDB.DataService.Data.Dto.Jobs;
using Moq;
using Quartz;

namespace MMDB.DataService.Data.Tests
{
	public class JobManangerTests
	{
		[Test]
		public void CanCreateACronJobDefinition()
		{
			using(var session = EmbeddedRavenProvider.DocumentStore.OpenSession())
			{
				var sut = new JobManager(session, DataServiceTestHelper.GetEventReporter().Object, new Mock<IScheduler>().Object);
				string assemblyName = Guid.NewGuid().ToString();
				string className = Guid.NewGuid().ToString();
				string scheduleExpression = Guid.NewGuid().ToString();
				string jobName = Guid.NewGuid().ToString();
			
				var result = sut.CreateCronJob(jobName, assemblyName, className, scheduleExpression);
				Assert.IsNotNull(result);
				Assert.IsNotNullOrEmpty(result.Id);
				
				var dbItem = session.Load<JobDefinition>(result.Id);
				Assert.IsNotNull(dbItem);
				Assert.AreEqual(jobName, dbItem.JobName);
				Assert.AreEqual(assemblyName, dbItem.AssemblyName);
				Assert.AreEqual(className, dbItem.ClassName);
				Assert.IsInstanceOf<JobCronSchedule>(dbItem.Schedule);
				Assert.AreEqual(scheduleExpression, ((JobCronSchedule)dbItem.Schedule).CronScheduleExpression);

				session.Delete(dbItem);
				session.SaveChanges();
			}
		}

		[Test]
		public void CanCreateASimpleJobDefinition()
		{
			using (var session = EmbeddedRavenProvider.DocumentStore.OpenSession())
			{
				var sut = new JobManager(session, DataServiceTestHelper.GetEventReporter().Object, new Mock<IScheduler>().Object);
				string assemblyName = Guid.NewGuid().ToString();
				string className = Guid.NewGuid().ToString();
				int intervalMinutes = 10;
				int delayStartMinutes = 20;
				string jobName = Guid.NewGuid().ToString();

				var result = sut.CreateSimpleJob(jobName, assemblyName, className, intervalMinutes, delayStartMinutes);
				Assert.IsNotNull(result);
				Assert.IsNotNullOrEmpty(result.Id);

				var dbItem = session.Load<JobDefinition>(result.Id);
				Assert.IsNotNull(dbItem);
				Assert.AreEqual(jobName, dbItem.JobName);
				Assert.AreEqual(assemblyName, dbItem.AssemblyName);
				Assert.AreEqual(className, dbItem.ClassName);
				Assert.IsInstanceOf<JobSimpleSchedule>(dbItem.Schedule);
				Assert.AreEqual(intervalMinutes, ((JobSimpleSchedule)dbItem.Schedule).IntervalMinutes);
				Assert.AreEqual(delayStartMinutes, ((JobSimpleSchedule)dbItem.Schedule).DelayStartMinutes);

				session.Delete(dbItem);
				session.SaveChanges();
			}
		}

		[Test]
		public void CanEditAJobDefinition()
		{
			using (var session = EmbeddedRavenProvider.DocumentStore.OpenSession())
			{
				var sut = new JobManager(session, DataServiceTestHelper.GetEventReporter().Object, new Mock<IScheduler>().Object);
				string assemblyName = Guid.NewGuid().ToString();
				string className = Guid.NewGuid().ToString();
				string scheduleExpression = Guid.NewGuid().ToString();
				string jobName = Guid.NewGuid().ToString();
				var newJob = sut.CreateCronJob(jobName, assemblyName, className, scheduleExpression);

				newJob.AssemblyName = Guid.NewGuid().ToString();
				newJob.ClassName = Guid.NewGuid().ToString();
				((JobCronSchedule)newJob.Schedule).CronScheduleExpression = Guid.NewGuid().ToString();
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
				var sut = new JobManager(session, DataServiceTestHelper.GetEventReporter().Object, new Mock<IScheduler>().Object);
				var list = sut.LoadJobList();
				Assert.AreEqual(0, list.Count());
			}
		}
	}
}
