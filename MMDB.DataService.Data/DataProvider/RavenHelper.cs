using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Settings;
using Raven.Client;
using Raven.Client.Document;

namespace MMDB.DataService.Data.DataProvider
{
	public static class RavenHelper
	{
		public static IDocumentStore CreateDocumentStore()
		{
			return new DocumentStore
			{
				ConnectionStringName = "RavenDB",
				Conventions =
				{
					FindTypeTagName = type =>
					{
						if (typeof(SettingsBase).IsAssignableFrom(type))
							return "SettingsBases";
						return DocumentConvention.DefaultTypeTagName(type);
					}
				}
			}.Initialize();
		}
	}
}
