using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using Ninject;

namespace MMDB.DataService.NinjectModules
{
	public static class NinjectBinder
	{
		public static void SetupAll(IKernel kernel)
		{
			//Do me first
			var coreNinjector = new CoreDataServiceNinjector();
			coreNinjector.Setup(kernel);
			
			var processedTypes = new List<Type>
			{
				coreNinjector.GetType()
			};
			//and then the other drooges
			var type = typeof(IDataServiceNinjector);
			var types = AppDomain.CurrentDomain.GetAssemblies().ToList()
				.SelectMany(s => s.GetTypes())
				.Where(p => type.IsAssignableFrom(p) 
							&& p != typeof(CoreDataServiceNinjector)
							&& !p.IsInterface
							&& !p.IsAbstract);
			foreach(var t in types)
			{
				if(!processedTypes.Contains(t))
				{
					var instance = (IDataServiceNinjector)Activator.CreateInstance(t);
					instance.Setup(kernel);
					processedTypes.Add(t);
				}
			}

			string assemblyListString = ConfigurationManager.AppSettings["NinjectAssemblyList"];
			if(!string.IsNullOrEmpty(assemblyListString))
			{
				var list = assemblyListString.Split(';');
				foreach(string assemblyName in list)
				{
					var assembly = Assembly.Load(assemblyName.Replace(".dll",""));
					var assemblyTypes = assembly.GetTypes().Where(p => type.IsAssignableFrom(p) 
																&& p != typeof(CoreDataServiceNinjector)
																&& !p.IsInterface
																&& !p.IsAbstract);
					foreach (var t in assemblyTypes)
					{
						if (!processedTypes.Contains(t))
						{
							var instance = (IDataServiceNinjector)Activator.CreateInstance(t);
							instance.Setup(kernel);
							processedTypes.Add(t);
						}
					}
				}
			}
			
		}
	}
}
