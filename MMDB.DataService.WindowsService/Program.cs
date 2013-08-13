using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Raven.Client;
using Raven.Client.Document;
using Quartz.Impl;
using Quartz;
using MMDB.DataService.Data.Settings;
using MMDB.DataService.Data.DataProvider;
using MMDB.DataService.Data.Jobs;
using MMDB.DataService.Data;
using System.Reflection;
using System.IO;
using Autofac;
using MMDB.DataService.AutofacModules;

namespace MMDB.DataService.WindowsService
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			try
			{
				Console.WriteLine("Starting MMDB.DataService.WindowsService");
				//Console.WriteLine("\t-Initializing Ninject...");
				//NinjectBootstrapper.Initialize();
				//Console.WriteLine("\t-Ninject initialization complete...");
				var builder = new ContainerBuilder();
				AutofacBuilder.SetupAll(builder);
				builder.RegisterType<WinService>().AsSelf();
				var container = builder.Build();
			
				if (args.Length > 0 && args[0].ToLower() == "/debug")
				{
					var updater = new ContainerBuilder();
					updater.RegisterType<ConsoleDataServiceLogger>().As<IDataServiceLogger>();
					updater.Update(container);

					Console.WriteLine("\t-Starting in debug mode...");
					//var service = NinjectBootstrapper.Get<WinService>();
					Console.WriteLine("\t-Resolving WinService...");
					var service = container.Resolve<WinService>();
					Console.WriteLine("\t-service.DebugStart...");
					service.DebugStart();
				}
				else if (args.Length > 0 && args[0].ToLower() == "/runjobnow")
				{
					int jobID = 0;
					if(args.Length < 2 || !int.TryParse(args[1], out jobID))
					{
						Console.WriteLine("/runjobname [jobid] - jobid must be an integer");
					}
					var updater = new ContainerBuilder();
					updater.RegisterType<ConsoleDataServiceLogger>().As<IDataServiceLogger>();
					updater.Update(container);

					//NinjectBootstrapper.Kernel.Bind<IDataServiceLogger>().To<ConsoleDataServiceLogger>();
					//var jobScheduler = NinjectBootstrapper.Kernel.Get<IJobScheduler>();
					var jobScheduler = container.Resolve<IJobScheduler>();
					jobScheduler.RunJobNow(jobID);
					System.Environment.Exit(0);
				}
				else if (args.Length > 0 && args[0].ToLower() == "/exportjobs")
				{
					Console.WriteLine("Exporting jobs");
					string exportPath;
					if(args.Length >= 2)
					{
						exportPath = args[1];
					}
					else
					{
						exportPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "MMDB.DataService.Jobs.json");
					}
					Console.WriteLine("\tExporting to " + exportPath);
					//var importerExporter = NinjectBootstrapper.Kernel.Get<IJobImporterExporter>();
					var importerExporter = container.Resolve<IJobImporterExporter>();
					importerExporter.ExportJobsToFile(exportPath);
					Console.WriteLine("\t-Exporting jobs Done");
					return;
				}
				else if (args.Length > 0 && args[0].ToLower() == "/importjobs")
				{
					Console.WriteLine("Importing jobs");
					string importPath;
					if (args.Length >= 2)
					{
						importPath = args[1];
					}
					else
					{
						importPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "MMDB.DataService.Jobs.json");
					}
					Console.WriteLine("\t-Importing from: " + importPath);
					//var importerExporter = NinjectBootstrapper.Kernel.Get<IJobImporterExporter>();
					var importerExporter = container.Resolve<IJobImporterExporter>();
					importerExporter.ImportJobsFromFile(importPath);
					Console.WriteLine("\t-Importing jobs done");
				}
				else 
				{
					Console.WriteLine("\t-Starting in service mode...");
					ServiceBase[] ServicesToRun;
					ServicesToRun = new ServiceBase[] 
					{ 
						//NinjectBootstrapper.Get<WinService>() 
						container.Resolve<WinService>()
					};
					ServiceBase.Run(ServicesToRun);
				}
			}
			catch(Exception err)
			{
				Console.WriteLine("Error: " + err.ToString());
				Console.WriteLine("Press any key to continue");
				Console.ReadKey();
			}
		}

		//public static IDocumentStore CreateDocumentStore(IContext context)
		//{
		//	var store = RavenHelper.CreateDocumentStore();
		//	return store;
		//}

		//public static IScheduler CreateScheduler(IContext context)
		//{
		//	var schedulerFactory = context.Kernel.Get<ISchedulerFactory>();
		//	var scheduler = schedulerFactory.GetScheduler();
		//	scheduler.JobFactory = context.Kernel.Get<NinjectJobFactory>();
		//	return scheduler;
		//}
	}
}
