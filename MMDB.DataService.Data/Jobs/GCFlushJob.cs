using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Jobs
{
	public class GCFlushJob : DataServiceJobBase<NullJobConfiguration>
	{
		private readonly IEventReporter _eventReporter;

		public GCFlushJob(IEventReporter eventReporter)
		{
			_eventReporter = eventReporter;
		}
		public override void Run(NullJobConfiguration configuration)
		{
			_eventReporter.Info("Starting GCFlushJob.Run");
			var existingMemory = GC.GetTotalMemory(false);
			_eventReporter.Info("GCFlushJob: existing memory: " + existingMemory.ToString());
			GC.Collect();
			GC.WaitForPendingFinalizers();
			var afterMemory = GC.GetTotalMemory(true);
			_eventReporter.Info("GCFlushJob: memory after collection: " + afterMemory.ToString());
			_eventReporter.Info("Done GCFlushJob.Run");
		}
	}
}
