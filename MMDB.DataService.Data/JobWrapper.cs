using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;

namespace MMDB.DataService.Data
{
	public class JobWrapper<T>: IJob where T:IDataServiceJob
	{
		private IDataServiceJob DataServiceJob { get; set; }
		private EventReporter EventReporter { get; set; }

		public JobWrapper(T dataServiceJob, EventReporter eventReporter)
		{
			this.DataServiceJob = dataServiceJob;
			this.EventReporter = eventReporter;
		}

		public void Execute(IJobExecutionContext context)
		{
			this.EventReporter.Trace("Starting Job " + context.JobDetail.Description);
			try 
			{
				this.DataServiceJob.Run();
			}
			catch(Exception err)
			{
				this.EventReporter.Exception(err);
			}
			this.EventReporter.Trace("Completed Job " + context.JobDetail.Description);
		}
	}
}
