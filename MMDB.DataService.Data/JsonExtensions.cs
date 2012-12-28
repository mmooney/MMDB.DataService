using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace System
{
	public static class JsonExtensions
	{
		public static string ToJson(this object obj)
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