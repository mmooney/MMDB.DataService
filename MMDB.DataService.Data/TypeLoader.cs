using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

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
			try 
			{
				var assembly = Assembly.Load(assemblyName.Replace(".dll",""));
				//string path = assemblyName;
				//if(!File.Exists(path))
				//{
				//	string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				//	path = Path.Combine(currentDirectory, assemblyName);
				//}
				//assembly = Assembly.LoadFrom(assemblyName);
				if(assembly == null)
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
			catch(Exception err)
			{
				throw new Exception("Failed to load type " + className + " from assembly " + assemblyName, err);
			}
		}
	}
}
