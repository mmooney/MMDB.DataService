using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Dto.Logging
{
	public class ServiceMessage
	{
		public EnumServiceMessageLevel Level { get; set; }
		public DateTime MessageDateTimeUtc { get; set; }
		public string Message { get; set; }
		public string Detail { get; set; }
		public object DataObject { get; set; }
	}
}
