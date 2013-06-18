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
using MMDB.DataService.Data;
using System.Reflection;
using System.IO;

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
				Console.WriteLine("\t-Initializing Ninject...");
				NinjectBootstrapper.Initialize();
				Console.WriteLine("\t-Ninject initialization complete...");
			
				if (args.Length > 0 && args[0].ToLower() == "/debug")
				{
					Console.WriteLine("\t-Starting in debug mode...");
					var service = NinjectBootstrapper.Get<WinService>();
					service.DebugStart();
				}
				else if (args.Length > 0 && args[0].ToLower() == "/runjobnow")
				{
					int jobID = 0;
					if(args.Length < 2 || !int.TryParse(args[1], out jobID))
					{
						Console.WriteLine("/runjobname [jobid] - jobid must be an integer");
					}
					NinjectBootstrapper.Kernel.Bind<DataServiceLogger>().To<ConsoleDataServiceLogger>();
					var jobScheduler = NinjectBootstrapper.Kernel.Get<IJobScheduler>();
					jobScheduler.RunJobNow(jobID);
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
					var importerExporter = NinjectBootstrapper.Kernel.Get<IJobImporterExporter>();
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
					var importerExporter = NinjectBootstrapper.Kernel.Get<IJobImporterExporter>();
					importerExporter.ImportJobsFromFile(importPath);
					Console.WriteLine("\t-Importing jobs done");
				}
				else 
				{
					Console.WriteLine("\t-Starting in service mode...");
					ServiceBase[] ServicesToRun;
					ServicesToRun = new ServiceBase[] 
					{ 
						NinjectBootstrapper.Get<WinService>() 
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
