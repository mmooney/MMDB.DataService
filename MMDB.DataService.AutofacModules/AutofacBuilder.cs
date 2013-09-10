using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;

namespace MMDB.DataService.AutofacModules
{
	public static class AutofacBuilder 
	{
		public static void SetupAll(ContainerBuilder builder)
		{
			try 
			{
				//Do me first
				var coreAutofacer = new CoreDataServiceAutofacer();
				builder.RegisterModule(coreAutofacer);

				builder.RegisterModule(new Whitebox.Containers.Autofac.WhiteboxProfilingModule());

				var processedTypes = new List<Type>
				{
					coreAutofacer.GetType()
				};
				//and then the other drooges
				var type = typeof(DataServiceAutofacModule);
				var types = AppDomain.CurrentDomain.GetAssemblies().ToList()
					.SelectMany(s => s.GetTypes())
					.Where(p => type.IsAssignableFrom(p)
								&& p != typeof(CoreDataServiceAutofacer)
								&& !p.IsInterface
								&& !p.IsAbstract);
				foreach (var t in types)
				{
					if (!processedTypes.Contains(t))
					{
						var instance = (DataServiceAutofacModule)Activator.CreateInstance(t);
						builder.RegisterModule(instance);
						processedTypes.Add(t);
					}
				}

				string assemblyListString = ConfigurationManager.AppSettings["AutofacAssemblyList"];
				if (!string.IsNullOrEmpty(assemblyListString))
				{
					var list = assemblyListString.Split(';');
					foreach (string assemblyName in list)
					{
						var assembly = Assembly.Load(assemblyName.Replace(".dll", ""));
						var assemblyTypes = assembly.GetTypes().Where(p => type.IsAssignableFrom(p)
																	&& p != typeof(CoreDataServiceAutofacer)
																	&& !p.IsInterface
																	&& !p.IsAbstract);
						foreach (var t in assemblyTypes)
						{
							if (!processedTypes.Contains(t))
							{
								var instance = (Autofac.Core.IModule)Activator.CreateInstance(t);
								builder.RegisterModule(instance);
								processedTypes.Add(t);
							}
						}
					}
				}
			}
			catch(Exception err)
			{
				string x= err.ToString();
				throw;
			}
		}
	}
}
