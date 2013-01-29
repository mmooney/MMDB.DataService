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

		private EventReporter EventReporter { get; set; }
		private DateTime UtcNow { get; set; }

		public LogPurgeJob(IDocumentSession documentSession, EventReporter eventReporter, DateTime? utcNow=null) : base(documentSession)
		{
			this.EventReporter = eventReporter;
			this.UtcNow = utcNow.GetValueOrDefault(DateTime.UtcNow);
		}

		public override void Run(LogPurgeJobConfiguration configuration)
		{
			this.EventReporter.Trace(this.GetType().Name + ": job run started");
			this.PurgeData(EnumServiceMessageLevel.Debug, configuration.DebugAgeMinutes);
			this.PurgeData(EnumServiceMessageLevel.Trace, configuration.TraceAgeMinutes);
			this.PurgeData(EnumServiceMessageLevel.Info, configuration.InfoAgeMinutes);
			this.PurgeData(EnumServiceMessageLevel.Warning, configuration.WarningAgeMinutes);
			this.PurgeData(EnumServiceMessageLevel.Error, configuration.ErrorAgeMinutes);
			this.EventReporter.Trace(this.GetType().Name + ": job run done");
		}

		private void PurgeData(EnumServiceMessageLevel messageLevel, int? ageMinutes)
		{
			if(ageMinutes.HasValue)
			{
				DateTime maxDate = this.UtcNow.AddMinutes(0-ageMinutes.Value);
				this.EventReporter.Trace("Deleting {0} messages older than {1}", messageLevel, maxDate);
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
