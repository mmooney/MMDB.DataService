using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Settings;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

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
					}
				}
			}.Initialize();
			IndexCreation.CreateIndexes(typeof(MMDB.DataService.Data.Jobs.DataServiceJobBase<>).Assembly, documentStore);
			return documentStore;
		}
	}
}
