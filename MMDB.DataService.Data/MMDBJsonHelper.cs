using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Raven.Imports.Newtonsoft.Json;

namespace MMDB.DataService.Data
{
	public static class MMDBJsonHelper
	{
		public static string ToJson(object obj, bool format=false)
		{
			if(format)
			{
				JsonSerializer serializer = new JsonSerializer();
				StringBuilder sb = new StringBuilder();
				using (var writer = new StringWriter(sb))
				{
					using(var jsonWriter = new JsonTextWriter(writer) { Formatting=Formatting.Indented })
					{
						serializer.Serialize(jsonWriter, obj);
						return sb.ToString();
					}
				}
			}
			else 
			{
				JsonSerializer serializer = new JsonSerializer();
				StringBuilder sb = new StringBuilder();
				using(var writer = new StringWriter(sb))
				{
					serializer.Serialize(writer, obj);
					return sb.ToString();
				}
			}
		}
	}
}
