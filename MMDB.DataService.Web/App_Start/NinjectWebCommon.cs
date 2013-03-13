[assembly: WebActivator.PreApplicationStartMethod(typeof(MMDB.DataService.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(MMDB.DataService.Web.App_Start.NinjectWebCommon), "Stop")]

namespace MMDB.DataService.Web.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
	using Raven.Client.Document;
	using System.Configuration;
	using Raven.Client;
using Ninject.Activation;
	using Quartz.Impl;
	using Quartz;
	using MMDB.DataService.Data.DataProvider;
	using System.Web.Mvc;
	using MMDB.DataService.Data.Jobs;
	using Ninject.Extensions.Conventions;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
		internal static T Get<T>()
		{
			return bootstrapper.Kernel.Get<T>();
		}
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel(new NinjectSettings() { UseReflectionBasedInjection=false});
			kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            
            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
			kernel.Rebind<IDocumentStore>().ToMethod(CreateDocumentStore).InSingletonScope();
			kernel.Rebind<IDocumentSession>().ToMethod(c => c.Kernel.Get<IDocumentStore>().OpenSession()).InRequestScope();
			kernel.Rebind<ISchedulerFactory>().To<StdSchedulerFactory>();
			kernel.Bind<IScheduler>().ToMethod(c => c.Kernel.Get<ISchedulerFactory>().GetScheduler());

			kernel.Bind(x =>
			{
				x.FromAssembliesMatching("*") // Scans currently assembly
				 .SelectAllClasses() // Retrieve all non-abstract classes
				 .BindDefaultInterface();// Binds the default interface to them;
			});

			ModelBinders.Binders.Add(typeof(JobConfigurationBase), kernel.Get<ConfigurationModelBinder>());
		}

		public static IDocumentStore CreateDocumentStore(IContext context)
		{
			return RavenHelper.CreateDocumentStore();
		}
	}
}
