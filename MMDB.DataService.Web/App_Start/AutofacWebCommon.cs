[assembly: WebActivator.PreApplicationStartMethod(typeof(MMDB.DataService.Web.App_Start.AutofacWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(MMDB.DataService.Web.App_Start.AutofacWebCommon), "Stop")]

namespace MMDB.DataService.Web.App_Start
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;
	using Autofac;
	using Autofac.Integration.Mvc;
	using MMDB.DataService.AutofacModules;
	using MMDB.DataService.Data.Metadata;

	public class AutofacWebCommon
	{
		public static void Start()
		{
			var builder = new ContainerBuilder();
			builder.RegisterControllers(typeof(AutofacWebCommon).Assembly);
			AutofacBuilder.SetupAll(builder);
			var container = builder.Build();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
		}
		
		public static void Stop()
		{
		}

		internal static T Get<T>()
		{
			return DependencyResolver.Current.GetService<T>();
		}
	}
}