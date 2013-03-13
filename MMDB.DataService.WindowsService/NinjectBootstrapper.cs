using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using Ninject.Extensions.Conventions;

namespace MMDB.DataService.WindowsService
{
	public static class NinjectBootstrapper
	{
		public static IKernel Kernel { get; set; }

		public static void Initialize()
		{
			var kernel = new StandardKernel();
			NinjectBootstrapper.Kernel = kernel;
			kernel.Bind(x =>
				{
					x.FromAssembliesMatching("*") // Scans currently assembly
					 .SelectAllClasses() // Retrieve all non-abstract classes
					 .BindDefaultInterface();// Binds the default interface to them;
				});
		}

		public static object Get(Type type)
		{
			return NinjectBootstrapper.Kernel.Get(type);
		}

		public static T Get<T>()
		{
			return NinjectBootstrapper.Kernel.Get<T>();
		}
	}
}
