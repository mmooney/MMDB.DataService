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
		private IEventReporter EventReporter { get; set; }
		private ILogPurger LogPurger { get; set; }

		public LogPurgeJob(IEventReporter eventReporter, ILogPurger logPurger) : base()
		{
			this.EventReporter = eventReporter;
			this.LogPurger = logPurger;
		}

		public override void Run(LogPurgeJobConfiguration configuration)
		{
			DateTime utcNow = configuration.UtcNow.GetValueOrDefault(DateTime.UtcNow);
			this.EventReporter.Trace(this.GetType().Name + ": job run started");
			this.LogPurger.PurgeData(utcNow, EnumServiceMessageLevel.Debug, configuration.DebugAgeMinutes);
			this.LogPurger.PurgeData(utcNow, EnumServiceMessageLevel.Trace, configuration.TraceAgeMinutes);
			this.LogPurger.PurgeData(utcNow, EnumServiceMessageLevel.Info, configuration.InfoAgeMinutes);
			this.LogPurger.PurgeData(utcNow, EnumServiceMessageLevel.Warning, configuration.WarningAgeMinutes);
			this.LogPurger.PurgeData(utcNow, EnumServiceMessageLevel.Error, configuration.ErrorAgeMinutes);
			this.EventReporter.Trace(this.GetType().Name + ": job run done");
		}
	}
}
