using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Dto.Assemblies
{
	public class AssemblyImportResult
	{
		public bool Success { get; set; }
		public JobAssembly ImportedAssembly { get; set; }
		public string ErrorMessage { get; set; }
	}
}
