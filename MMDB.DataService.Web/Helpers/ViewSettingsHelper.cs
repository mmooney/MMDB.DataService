using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MMDB.DataService.Data.Settings;
using MMDB.DataService.Web.App_Start;

namespace MMDB.DataService.Web.Helpers
{
	public static class ViewSettingsHelper
	{
		public static string ApplicationDisplayName 
		{
			get 
			{
				return null;
				//var settingsManager = NinjectWebCommon.Get<SettingsManager>();
				//var settings = settingsManager.Get<CoreDataServiceSettings>();
				//return settings.ApplicationDisplayName;
			}
		}

		public static string LogoUrl 
		{
			get 
			{
				return null;
				//var settingsManager = NinjectWebCommon.Get<SettingsManager>();
				//var settings = settingsManager.Get<CoreDataServiceSettings>();
				//return settings.LogoUrl;
			}
		}
	}
}