using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MMDB.DataService.Data
{
	public class TypeLoader
	{
		public Type LoadType(string assemblyName, string className)
		{
			if(string.IsNullOrEmpty(assemblyName))
			{
				throw new ArgumentException("assemblyName cannot be null or empty");
			}
			if(string.IsNullOrEmpty(className))
			{
				throw new ArgumentException("className cannot be null or empty");
			}
			var assembly = Assembly.Load(assemblyName);
			if(assemblyName == null)
			{
				throw new Exception("Failed to load assembly " + assemblyName);
			}
			var type = assembly.GetType(className);
			if(type == null)
			{
				throw new Exception("Failed to load type " + className + " from assembly " + assemblyName);
			}
			return type;
		}
	}
}
