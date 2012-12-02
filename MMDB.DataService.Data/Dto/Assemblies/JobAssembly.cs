using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Dto.Assemblies
{
	public class JobAssembly
	{
		public string Id { get; set; }
		public string FileName { get; set; }
		public string AssemblyName { get; set; }
		public List<string> ClassNameList { get; set; }
		public string AttachmentId { get; set; }
	}
}
