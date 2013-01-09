using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data
{
	public static class StreamHelper
	{
		public static string ReadAll(MemoryStream stream)
		{
			long oldPosition = stream.Position;
			stream.Position = 0;

			var reader = new StreamReader(stream);	//Do NOT dispose this reader, because it will dispose the stream?
			string returnValue = reader.ReadToEnd();

			stream.Position = oldPosition;

			return returnValue;
		}
	}
}
