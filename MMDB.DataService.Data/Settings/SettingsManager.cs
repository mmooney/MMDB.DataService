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

		public virtual T Get<T>() where T:SettingsBase
		{
			var container = this.DocumentSession.Query<SettingsContainer>().Where(i=>i.IsActive == true && i.Settings.TypeName == typeof(T).FullName).FirstOrDefault();
			if(container == null)
			{
				return null;
			}
			else 
			{
				return (T)container.Settings;
			}
		}

		public virtual object Get(Type type)
		{
			var container = this.DocumentSession.Query<SettingsContainer>().Where(i=>i.IsActive == true && i.Settings.TypeName == type.FullName).FirstOrDefault();
			if(container == null)
			{
				return null;
			}
			else 
			{
				return container.Settings;
			}
		}

		public virtual IEnumerable<SettingsContainer> GetList()
		{
			return this.DocumentSession.Query<SettingsContainer>().ToList();
		}

	}
}
