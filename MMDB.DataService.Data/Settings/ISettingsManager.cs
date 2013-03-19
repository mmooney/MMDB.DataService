using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Settings
{
	public interface ISettingsManager
	{
		T Get<T>() where T : SettingsBase;
	}
}
