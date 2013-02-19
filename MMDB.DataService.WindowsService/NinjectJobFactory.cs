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


		public void ReturnJob(IJob job)
		{
			//https://groups.google.com/forum/?fromgroups=#!topic/quartznet/nu_OKpi3rLw
			/* 
				This method is for job factory to allow returning of the instance back 
				to IoC container for proper cleanup. By default you don't need to do 
				anything if you haven't managed destroying of object using the 
				container before. 
			 */
		}
	}
}
