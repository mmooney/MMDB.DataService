using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Settings;
using MMDB.Shared;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Raven.Client.UniqueConstraints;

namespace MMDB.DataService.Data.DataProvider
{
	public static class RavenHelper
	{
		public static IDocumentStore CreateDocumentStore()
		{
			var documentStore = new DocumentStore
			{
				ConnectionStringName = "RavenDB",
				Conventions =
				{
					FindTypeTagName = type =>
					{
						if (typeof(SettingsBase).IsAssignableFrom(type))
						{
							return "SettingsBases";
						}
						if (typeof(ConnectionSettingBase).IsAssignableFrom(type))
						{
							return "ConnectionSettingBases";
						}
						return DocumentConvention.DefaultTypeTagName(type);
					},
					MaxNumberOfRequestsPerSession = AppSettingsHelper.GetIntSetting("RavenMaxNumberOfRequestsPerSession", 3000)
				}
			}.RegisterListener(new UniqueConstraintsStoreListener()).Initialize();
			IndexCreation.CreateIndexes(typeof(MMDB.DataService.Data.Jobs.DataServiceJobBase<>).Assembly, documentStore);
			return documentStore;
		}
	}
}
