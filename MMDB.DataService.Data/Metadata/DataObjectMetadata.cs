using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Metadata
{
	public class DataObjectMetadata
	{
		public int Id { get; set; }
		public string ObjectName { get; set; }
		public string AssemblyName { get; set; }
		public string ClassName { get; set; }
	}
}
