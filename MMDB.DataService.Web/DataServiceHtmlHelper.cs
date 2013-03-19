using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MMDB.DataService.Data;

namespace MMDB.DataService.Web
{
	public static class DataServiceHtmlHelper
	{
		public static List<NavigationItem> GetNavigationItems() 
		{
			return MvcApplication.NavigationItems;
		}
	}
}