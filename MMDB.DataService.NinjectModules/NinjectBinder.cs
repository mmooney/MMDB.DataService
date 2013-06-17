using System;
using System.Collections.Generic;
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
				if(t != typeof(CoreDataServiceNinjector))
				{
					var instance = (IDataServiceNinjector)Activator.CreateInstance(t);
					instance.Setup(kernel);
				}
			}
			
		}
	}
}
