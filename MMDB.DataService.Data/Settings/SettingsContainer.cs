using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Settings
{
	public class SettingsContainer
	{
		public int Id { get; set; }
		public bool IsActive { get; set; }

		public SettingsBase Settings { get; set; }
	}
}
