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

		public T Get<T>() where T:SettingsBase
		{
			return this.DocumentSession.Query<T>().Where(i=>i.IsActive == true).FirstOrDefault();
		}
	}
}
