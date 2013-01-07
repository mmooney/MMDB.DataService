using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Raven.Client;
using Ninject;
using Ninject.Activation;
using Raven.Client.Document;
using Quartz.Impl;
using Quartz;
using MMDB.DataService.Data.Settings;
using MMDB.DataService.Data.DataProvider;
using MMDB.DataService.Data.Jobs;

namespace MMDB.DataService.WindowsService
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			NinjectBootstrapper.Initialize();
			NinjectBootstrapper.Kernel.Bind<IDocumentStore>().ToMethod(CreateDocumentStore).InSingletonScope();
			NinjectBootstrapper.Kernel.Bind<IDocumentSession>().ToMethod(c=>c.Kernel.Get<IDocumentStore>().OpenSession()).InTransientScope();
			NinjectBootstrapper.Kernel.Bind<ISchedulerFactory>().To<StdSchedulerFactory>();
			NinjectBootstrapper.Kernel.Bind<IScheduler>().ToMethod(CreateScheduler).InSingletonScope();
			
			var settings = NinjectBootstrapper.Kernel.Get<CoreDataServiceSettings>();
			if (args.Length > 0 && args[0].ToLower() == "/debug")
			{
				var service = NinjectBootstrapper.Get<WinService>();
				service.DebugStart();
			}
			else 
			{
				ServiceBase[] ServicesToRun;
				ServicesToRun = new ServiceBase[] 
				{ 
					NinjectBootstrapper.Get<WinService>() 
				};
				ServiceBase.Run(ServicesToRun);
			}
		}

		public static IDocumentStore CreateDocumentStore(IContext context)
		{
			var store = RavenHelper.CreateDocumentStore();
			return store;
		}

		public static IScheduler CreateScheduler(IContext context)
		{
			var schedulerFactory = context.Kernel.Get<ISchedulerFactory>();
			var scheduler = schedulerFactory.GetScheduler();
			scheduler.JobFactory = context.Kernel.Get<NinjectJobFactory>();
			return scheduler;
		}
	}
}
