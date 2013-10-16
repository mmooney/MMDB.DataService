using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using MMDB.Data.DataService;
using MMDB.DataService.Data;
using MMDB.DataService.Data.DataProvider;
using MMDB.DataService.Data.Impl;
using MMDB.DataService.Data.Jobs;
using MMDB.DataService.Data.Metadata;
using MMDB.DataService.Data.Metadata.MetadataImpl;
using MMDB.DataService.Data.Settings;
using MMDB.DataService.Data.Settings.SettingsImpl;
using MMDB.RazorEmail;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Raven.Client;

namespace MMDB.DataService.AutofacModules
{
	public class CoreDataServiceAutofacer : DataServiceAutofacModule
	{
		protected override void Load(ContainerBuilder builder)
		{
			ServiceStartupLogger.LogMessage("Start CoreDataServiceAutofacter.Load");
			//builder.RegisterType<RavenServerProvider>().As<raven>();
			builder.RegisterType<DataServiceEmailSender>().As<IDataServiceEmailSender>();
			builder.RegisterType<EventReporter>().As<IEventReporter>();
			builder.RegisterType<FtpCommunicator>().As<IFtpCommunicator>();
			builder.RegisterType<FtpJobManager>().As<IFtpJobManager>();
			builder.RegisterType<LogPurger>().As<ILogPurger>();
			builder.RegisterType<RavenManager>().As<IRavenManager>();
			builder.RegisterType<CustomControllerManager>().As<ICustomControllerManager>();
			builder.RegisterType<DataObjectManager>().As<IDataObjectManager>();
			builder.RegisterType<DataServiceViewManager>().As<IDataServiceViewManager>();
			builder.RegisterType<SettingsManager>().As<ISettingsManager>();
			builder.RegisterType<JobManager>().As<IJobManager>();
			builder.RegisterType<JobImporterExporter>().As<IJobImporterExporter>();
			builder.RegisterType<JobScheduler>().As<IJobScheduler>();
			builder.RegisterType<TypeLoader>().As<ITypeLoader>();
			builder.RegisterType<DataServiceLogger>().As<IDataServiceLogger>();
			builder.RegisterType<ExceptionReporter>().As<IExceptionReporter>();
			builder.RegisterType<ConnectionSettingsManager>().As<IConnectionSettingsManager>();
			builder.RegisterType<ScheduleManager>().As<IScheduleManager>();

			builder.RegisterType<FtpDownloadJob>().AsSelf();
			builder.RegisterType<FtpUploadJob>().AsSelf();
			builder.RegisterType<LogPurgeJob>().AsSelf();
			builder.RegisterType<GCFlushJob>().AsSelf();

			builder.RegisterGeneric(typeof(JobWrapper<,>)).AsSelf();

			builder.RegisterType<EmailSender>().AsSelf();
			builder.RegisterType<RazorEmailEngine>().AsSelf();

			builder.Register(ctx =>
			{
				ServiceStartupLogger.LogMessage("Resolving IDocumentSession");
				var store = RavenHelper.CreateDocumentStore();
				ServiceStartupLogger.LogMessage("Done Resolving IDocumentSession");
				return store;
			}
			).As<IDocumentStore>().SingleInstance();
			builder.Register(ctx=>
			{
				ServiceStartupLogger.LogMessage("Resolving IDocumentSession, getting IDocumentStore");
				var store = ctx.Resolve<IDocumentStore>();
				ServiceStartupLogger.LogMessage("Resolving IDocumentSession, got IDocumentStore, calling open session");
				var session = ctx.Resolve<IDocumentStore>().OpenSession();
				ServiceStartupLogger.LogMessage("Done Resolving IDocumentSession");
				return session;
			}).As<IDocumentSession>();

			builder.RegisterGeneric(typeof(AutofacJobWrapper<>)); 
			builder.RegisterType<StdSchedulerFactory>().As<ISchedulerFactory>();
			builder.RegisterType<AutofacJobFactory>().As<IJobFactory>();
			builder.Register(ctx=>
				{
					ServiceStartupLogger.LogMessage("Resolving IScheduler");
					var schedulerFactory = ctx.Resolve<ISchedulerFactory>();
					var scheduler = schedulerFactory.GetScheduler();
					scheduler.JobFactory = ctx.Resolve<IJobFactory>();
					ServiceStartupLogger.LogMessage("Done Resolving IScheduler");
					return scheduler;
				}).As<IScheduler>();
			ServiceStartupLogger.LogMessage("End CoreDataServiceAutofacter.Load");
		}
	}
}
