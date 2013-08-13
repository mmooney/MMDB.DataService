using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Settings
{
	public interface ISettingsManager
	{		
		T Get<T>() where T : SettingsBase;
		object TryGet(Type type);
		T TryGet<T>() where T : SettingsBase;

		List<SettingsContainer> GetList();
		SettingsContainer GetSettings(int id);
		void ImportSettings(List<SettingsContainer> list);

	}
}
