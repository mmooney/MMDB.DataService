using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using Raven.Imports.Newtonsoft.Json;
using MMDB.DataService.Data;

namespace System
{
	public static class MMDBJsonExtensions
	{
		public static string ToJson(this object obj, bool format=false)
		{
			return MMDBJsonHelper.ToJson(obj, format);
		}
	}
}