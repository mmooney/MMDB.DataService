using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Logging;
using MMDB.DataService.Data.Jobs;
using Moq;
using NUnit.Framework;

namespace MMDB.DataService.Data.Tests
{
	public class LogPurgeJobTests
	{
		[Test]
		public void PurgesTraceMessages()
		{
			using(var session = EmbeddedRavenProvider.DocumentStore.OpenSession())
			{
				Mock<EventReporter> eventReporter = new Mock<EventReporter>(null,null);
				var configuration = new LogPurgeJobConfiguration
				{
					TraceAgeMinutes = 1000
				};
				DateTime now = DateTime.UtcNow;
				DateTime cutoff = now.AddMinutes(0-configuration.TraceAgeMinutes.Value);
				for(int i = 0; i < 10; i++)
				{
					var oldMessage = new ServiceMessage
					{
						Level = EnumServiceMessageLevel.Trace,
						Message = Guid.NewGuid().ToString(),
						MessageDateTimeUtc = cutoff.AddMinutes(0-10)
					};
					session.Store(oldMessage);

					var newMessage = new ServiceMessage
					{
						Level = EnumServiceMessageLevel.Trace,
						Message = Guid.NewGuid().ToString(),
						MessageDateTimeUtc = cutoff.AddMinutes(0+10)
					};
					session.Store(newMessage);
				}
				session.SaveChanges();
				var beforeMessages= session.Query<ServiceMessage>().OrderBy(i=>i.MessageDateTimeUtc).ToList();
				int originalOldMesssageCount = session.Query<ServiceMessage>().Where(i=>i.MessageDateTimeUtc < cutoff).Count();
				int originalNewMessageCount = session.Query<ServiceMessage>().Where(i=>i.MessageDateTimeUtc > cutoff).Count();
				var sut = new LogPurgeJob(session, eventReporter.Object, now);
				sut.Run(configuration);

				var afterMesages = session.Query<ServiceMessage>().OrderBy(i => i.MessageDateTimeUtc).ToList();
				int newOldMesssageCount = session.Query<ServiceMessage>().Customize(i=>i.WaitForNonStaleResultsAsOfNow()).Where(i => i.MessageDateTimeUtc < cutoff).Count();
				int newNewMessageCount = session.Query<ServiceMessage>().Customize(i=>i.WaitForNonStaleResultsAsOfNow()).Where(i=>i.MessageDateTimeUtc > cutoff).Count();

				Assert.AreEqual(0, newOldMesssageCount);
				Assert.GreaterOrEqual(newNewMessageCount, originalNewMessageCount);

			}
		}
	}
}
