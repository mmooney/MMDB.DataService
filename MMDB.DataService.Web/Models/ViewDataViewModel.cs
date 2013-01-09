using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MMDB.DataService.Data.Metadata;

namespace MMDB.DataService.Web.Models
{
	public class ViewDataViewModel
	{
		public DataObjectMetadata Metadata { get; set; }
		public dynamic DataObject { get; set; }
	}
}