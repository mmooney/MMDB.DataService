﻿using System;
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

		public T Get<T>() where T:SettingsBase
		{
			var container = this.DocumentSession.Query<SettingsContainer>().Where(i=>i.IsActive == true && i.Settings.TypeName == typeof(T).FullName).FirstOrDefault();
			return (T)container.Settings;
		}

		public object Get(Type type)
		{
			var container = this.DocumentSession.Query<SettingsContainer>().Where(i=>i.IsActive == true && i.Settings.TypeName == type.FullName).FirstOrDefault();
			return container.Settings;
		}

		public IEnumerable<SettingsContainer> GetList()
		{
			return this.DocumentSession.Query<SettingsContainer>().ToList();
		}

	}
}
