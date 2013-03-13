using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Logging;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Indexes;

namespace MMDB.DataService.Data.Jobs
{
	public class LogPurgeJob : DataServiceJobBase<LogPurgeJobConfiguration>
	{
		public class ServiceMessagesByDateIndex : AbstractIndexCreationTask<ServiceMessage>
		{
			public ServiceMessagesByDateIndex()
			{
				Map = messages => from i in messages
									select new { i.MessageDateTimeUtc, i.Level };
				Index(x=>x.MessageDateTimeUtc, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
			}
		}

		private IEventReporter EventReporter { get; set; }

		public LogPurgeJob(IDocumentSession documentSession, IEventReporter eventReporter) : base(documentSession)
		{
			this.EventReporter = eventReporter;
		}

		public override void Run(LogPurgeJobConfiguration configuration)
		{
			DateTime utcNow = configuration.UtcNow.GetValueOrDefault(DateTime.UtcNow);
			this.EventReporter.Trace(this.GetType().Name + ": job run started");
			this.PurgeData(utcNow, EnumServiceMessageLevel.Debug, configuration.DebugAgeMinutes);
			this.PurgeData(utcNow, EnumServiceMessageLevel.Trace, configuration.TraceAgeMinutes);
			this.PurgeData(utcNow, EnumServiceMessageLevel.Info, configuration.InfoAgeMinutes);
			this.PurgeData(utcNow, EnumServiceMessageLevel.Warning, configuration.WarningAgeMinutes);
			this.PurgeData(utcNow, EnumServiceMessageLevel.Error, configuration.ErrorAgeMinutes);
			this.EventReporter.Trace(this.GetType().Name + ": job run done");
		}

		private void PurgeData(DateTime utcNow, EnumServiceMessageLevel messageLevel, int? ageMinutes)
		{
			if(ageMinutes.HasValue)
			{
				DateTime maxDate = utcNow.AddMinutes(0-ageMinutes.Value);
				this.EventReporter.Trace(string.Format("Deleting {0} messages older than {1}", messageLevel, maxDate));
				var query = this.DocumentSession.Advanced.LuceneQuery<ServiceMessage, ServiceMessagesByDateIndex>()
												.WhereEquals("Level", messageLevel)
												.AndAlso().WhereLessThan("MessageDateTimeUtc", maxDate);
				var queryString = query.ToString();
				this.DocumentSession.Advanced.DocumentStore.DatabaseCommands.DeleteByIndex(
					typeof(ServiceMessagesByDateIndex).Name, new IndexQuery
					{
						Query = queryString
					}, 
					allowStale:true
				);
				this.DocumentSession.SaveChanges();
			}
		}
	}
}
