using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;

namespace MMDB.DataService.Data.Settings
{
	public class SettingsManager
	{
		private IDocumentSession DocumentSession { get; set; }

		public SettingsManager(IDocumentSession documentSession)
		{
			this.DocumentSession = documentSession;
		}

		public SettingsContainer Get<T>() where T:SettingsBase
		{
			return this.DocumentSession.Query<SettingsContainer>().Where(i=>i.IsActive == true && i.Settings.TypeName == typeof(T).FullName).FirstOrDefault();
		}

		public IEnumerable<SettingsContainer> GetList()
		{
			return this.DocumentSession.Query<SettingsContainer>().ToList();
		}
	}
}
