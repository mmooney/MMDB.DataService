using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Settings;

namespace MMDB.DataService.Data
{
	public interface IConnectionSettingsManager
	{
		T Load<T>(EnumSettingSource source, string key) where T:ConnectionSettingBase, new();
	}
}
