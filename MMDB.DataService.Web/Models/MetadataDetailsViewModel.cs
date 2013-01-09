using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MMDB.DataService.Data.Metadata;

namespace MMDB.DataService.Web.Models
{
	public class MetadataDetailsViewModel
	{
		public DataObjectMetadata Metadata { get; set; }
		public IList<object> DataList { get; set; }
	}
}