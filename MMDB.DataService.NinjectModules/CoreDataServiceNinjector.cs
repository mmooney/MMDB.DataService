using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Data.DataService;
using MMDB.DataService.Data;
using MMDB.DataService.Data.DataProvider;
using MMDB.DataService.Data.Metadata;
using MMDB.DataService.Data.Settings;
using Ninject;
using Ninject.Activation;
using Quartz;
using Quartz.Impl;
using Raven.Client;

namespace MMDB.DataService.NinjectModules
{
	public class CoreDataServiceNinjector : IDataServiceNinjector
	{
		public void Setup(IKernel kernel)
		{
			kernel.Bind<IRavenProvider>().To<RavenServerProvider>();
			kernel.Bind<IDataServiceEmailSender>().To<DataServiceEmailSender>();
			kernel.Bind<IEventReporter>().To<EventReporter>();
			kernel.Bind<IFtpCommunicator>().To<FtpCommunicator>();
			kernel.Bind<IFtpJobManager>().To<FtpJobManager>();
			kernel.Bind<ILogPurger>().To<LogPurger>();
			kernel.Bind<IRavenManager>().To<RavenManager>();
			kernel.Bind<ICustomControllerManager>().To<CustomControllerManager>();
			kernel.Bind<ISettingsManager>().To<SettingsManager>();
			kernel.Bind<IJobManager>().To<JobManager>();
			kernel.Bind<IJobImporterExporter>().To<JobImporterExporter>();
			kernel.Bind<IJobScheduler>().To<JobScheduler>();
			kernel.Bind<ITypeLoader>().To<TypeLoader>();

			kernel.Bind<IDocumentStore>().ToMethod(CreateDocumentStore).InSingletonScope();
			kernel.Bind<IDocumentSession>().ToMethod(c => c.Kernel.Get<IDocumentStore>().OpenSession()).InTransientScope();

			kernel.Bind<ISchedulerFactory>().To<StdSchedulerFactory>();
			kernel.Bind<IScheduler>().ToMethod(CreateScheduler).InSingletonScope();
		}

		public static IDocumentStore CreateDocumentStore(IContext context)
		{
			return RavenHelper.CreateDocumentStore();
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
