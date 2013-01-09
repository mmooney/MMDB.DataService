using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Metadata
{
	public class DataObjectView
	{
		public int Id { get; set; }
		public EnumViewStorageType StorageType { get; set; }
		public string VirtualPath { get; set; }

		public string ResourceAssemblyName { get; set; }
		public string ResourceIdentifier { get; set; }
		
	}
}
