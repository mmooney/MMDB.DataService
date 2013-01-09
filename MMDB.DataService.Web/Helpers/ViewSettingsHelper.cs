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
				var settingsManager = NinjectWebCommon.Get<SettingsManager>();
				var settings = settingsManager.TryGet<CoreDataServiceSettings>();
				if(settings == null)
				{
					return "MMDB Data Service";
				}
				else 
				{
					return settings.ApplicationName;
				}
			}
		}

		public static string LogoUrl 
		{
			get 
			{
				var settingsManager = NinjectWebCommon.Get<SettingsManager>();
				var settings = settingsManager.TryGet<CoreDataServiceSettings>();
				if(settings == null)
				{
					return "http://mmdbsolutions.com/Portals/6/logo_web.jpg";
				}
				else 
				{
					return settings.LogoUrl;
				}
			}
		}
	}
}