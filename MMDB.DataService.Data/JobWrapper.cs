using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using MMDB.DataService.Data.Jobs;

namespace MMDB.DataService.Data
{
	public class JobWrapper<JobType, ConfigType> : IJob where JobType : DataServiceJobBase<ConfigType> where ConfigType:JobConfigurationBase
	{
		private DataServiceJobBase<ConfigType> DataServiceJob { get; set; }
		private EventReporter EventReporter { get; set; }

		public JobWrapper(JobType dataServiceJob, EventReporter eventReporter)
		{
			this.DataServiceJob = dataServiceJob;
			this.EventReporter = eventReporter;
		}

		public void Execute(IJobExecutionContext context)
		{
			this.EventReporter.Trace("Starting Job " + context.JobDetail.Description);
			try 
			{
				var configuation = (ConfigType)context.JobDetail.JobDataMap["Configuration"];
				this.DataServiceJob.Run(configuation);
			}
			catch(Exception err)
			{
				this.EventReporter.Exception(err);
			}
			this.EventReporter.Trace("Completed Job " + context.JobDetail.Description);
		}
	}
}
