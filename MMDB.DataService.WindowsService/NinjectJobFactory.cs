using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz.Spi;
using Quartz;
using MMDB.DataService.Data;

namespace MMDB.DataService.WindowsService
{
	public class NinjectJobFactory : IJobFactory
	{
		private EventReporter EventReporter { get; set; }

		public NinjectJobFactory(EventReporter eventReporter)
		{
			this.EventReporter = eventReporter;
		}

		public IJob NewJob(TriggerFiredBundle bundle, Quartz.IScheduler scheduler)
		{
			try 
			{
				var x = NinjectBootstrapper.Get(bundle.JobDetail.JobType);
				return (IJob)x;
			}
			catch(Exception err)
			{
				this.EventReporter.ExceptionForObject(err, bundle.JobDetail);
				throw;
			}
		}
	}
}
