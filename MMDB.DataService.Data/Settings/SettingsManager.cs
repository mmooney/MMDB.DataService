using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;
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

		public virtual T Get<T>() where T : SettingsBase
		{
			var returnValue = this.TryGet<T>();
			if(returnValue == null)
			{
				throw new RecordNotFoundException(typeof(T), "Type", typeof(T).Name);
			}
			return returnValue;
		}

		public virtual T TryGet<T>() where T : SettingsBase
		{
			var container = this.DocumentSession.Query<SettingsContainer>().Where(i => i.IsActive == true && i.Settings.TypeName == typeof(T).FullName).FirstOrDefault();
			if (container == null)
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
			var returnValue = this.TryGet(type);
			if(returnValue == null)
			{
				throw new RecordNotFoundException(type, "Type", type.Name);
			}
			return returnValue;
		}

		public virtual object TryGet(Type type)
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

		public virtual List<SettingsContainer> GetList()
		{
			return this.DocumentSession.Query<SettingsContainer>().ToList();
		}


		public SettingsContainer GetSettings(int id)
		{
			return this.DocumentSession.Load<SettingsContainer>(id);
		}

		public void ImportSettings(List<SettingsContainer> list)
		{
			foreach (var newSettings in list)
			{
				var existingSettings = this.DocumentSession.Query<SettingsContainer>().FirstOrDefault(i => i.Settings.TypeName == newSettings.Settings.TypeName);
				if (existingSettings == null)
				{
					newSettings.Id = 0;
					this.DocumentSession.Store(newSettings);
				}
				else
				{
					int originalID = existingSettings.Id;
					AutoMapper.Mapper.Map(newSettings, existingSettings);
					newSettings.Id = originalID;
				}
				this.DocumentSession.SaveChanges();
			}
		}
	}
}
