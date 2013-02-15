using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;

namespace MMDB.DataService.Data
{
	public static class DocumentSessionExtensions
	{
		public static object Load(this IDocumentSession session, Type type, ValueType id)
		{
			var entityName = session.Advanced.DocumentStore.Conventions.GetTypeTagName(type);
			var itemQuery = session.Advanced.LuceneQuery<dynamic>().WhereEquals("@metadata.Raven-Entity-Name", entityName);
			return itemQuery.Single(i => id.Equals(i.Id));
		}

		public static object Load(this IDocumentSession session, Type type, string id)
		{
			var entityName = session.Advanced.DocumentStore.Conventions.GetTypeTagName(type);
			var itemQuery = session.Advanced.LuceneQuery<dynamic>().WhereEquals("@metadata.Raven-Entity-Name", entityName);
			return itemQuery.Single(i => i.Id == id);
		}
	}
}
