using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using MMDB.DataService.Data;
using Quartz;
using Quartz.Spi;

namespace MMDB.DataService.AutofacModules
{
	public class AutofacJobFactory : IJobFactory
	{
		//private IEventReporter EventReporter { get; set; }
		private IComponentContext Context { get; set; }

		public AutofacJobFactory(/*IEventReporter eventReporter, */IComponentContext context)
		{
			//this.EventReporter = eventReporter;
			this.Context = context;
		}

		public IJob NewJob(TriggerFiredBundle bundle, Quartz.IScheduler scheduler)
		{
			try 
			{
				var genericType = typeof(AutofacJobWrapper<>);
				Type[] typeArgs = { bundle.JobDetail.JobType };
				var actualType = genericType.MakeGenericType(typeArgs);
				var x = this.Context.Resolve(actualType);
				return (IJob)x;
			}
			catch(Exception err)
			{
				//using(var scope = this.Context.BeginLifetimeScope())
				//{
				//	var reporter = scope.Resolve<IEventReporter>();
				//	reporter.ExceptionForObject(err, bundle.JobDetail);
				//}
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
