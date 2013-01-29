using System;
using System.Diagnostics;

namespace MMDB.DataService.Data.Dto.Logging
{
	[DebuggerDisplay("{Level} - {MessageDateTimeUtc} - {Message}")]
	public class ServiceMessage
	{
		public int Id { get; set; }
		public EnumServiceMessageLevel Level { get; set; }
		public DateTime MessageDateTimeUtc { get; set; }
		public string Message { get; set; }
		public string Detail { get; set; }
		public string DataObjectJson { get; set; }
	}
}
