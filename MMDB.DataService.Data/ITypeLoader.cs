using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data
{
	public interface ITypeLoader
	{
		Type LoadType(string assemblyName, string className);
	}
}
