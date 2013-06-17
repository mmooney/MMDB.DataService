using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.RazorEmail;

namespace MMDB.DataService.Data.Settings
{
	public class CoreDataServiceSettings : SettingsBase
	{
		public string ApplicationName { get; set; }
		public string LogoUrl { get; set; }
		public string DisplayTimeZoneIdentifier { get; set; }

		public string ExceptionNotificationFromEmailAddress { get; set; }
		public List<string> ExceptionNotificationEmailAddressList { get; set; }
		//public EmailServerSettings Email { get; set; }

		public EnumSettingSource EmailSettingSource { get; set; }
		public string EmailSettingKey { get; set; }
	}
}
