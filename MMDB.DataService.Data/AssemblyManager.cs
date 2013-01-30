using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using MMDB.DataService.Data.Dto.Assemblies;

namespace MMDB.DataService.Data
{
	public class AssemblyManager
	{
		//public AssemblyImportResult ImportAssembly(string fileName, Stream stream)
		//{
		//	var memoryStream = new MemoryStream();
		//	stream.CopyTo(memoryStream);
		//	var byteArray = memoryStream.ToArray();			
		//	Assembly assembly;
		//	try 
		//	{
		//		assembly = Assembly.Load(byteArray);
		//	}
		//	catch(Exception err)
		//	{
		//		return new AssemblyImportResult
		//		{
		//			Success = false,
		//			ErrorMessage = "Failed to load assembly: " + err.ToString()
		//		};
		//	}

		//	JobAssembly jobAssembly = new JobAssembly
		//	{
		//		FileName = fileName,
		//		AssemblyName = assembly.FullName,
		//		ClassNameList = new List<string>()
		//	};

		//	var typeList = assembly.GetTypes();
		//	foreach(var typeItem in typeList)
		//	{
		//		if(typeItem.IsClass && typeItem.IsAssignableFrom(typeof(IDataServiceJob)))
		//		{
		//			jobAssembly.ClassNameList.Add(typeItem.FullName);
		//		}
		//	}
		//	if(jobAssembly.ClassNameList.Count == 0)
		//	{
		//		return new AssemblyImportResult
		//		{
		//			Success = false,
		//			ErrorMessage = "Assembly does not contain any classes that implement " + typeof(IDataServiceJob).FullName
		//		};
		//	}
		//	else 
		//	{
		//		return new AssemblyImportResult
		//		{
		//			Success = true,
		//			ImportedAssembly = jobAssembly
		//		};
		//	}
		//}
	}
}
