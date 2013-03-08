using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;

namespace MMDB.DataService.Data
{
	public static class DocumentSessionExtensions
	{
		public static object Load(this IDocumentSession session, Type type, int id)
		{
			var entityName = session.Advanced.DocumentStore.Conventions.GetTypeTagName(type);
			string idStringValue = entityName + "/" + id.ToString();
			var itemQuery = session.Advanced.LuceneQuery<dynamic>().WhereEquals("@metadata.Raven-Entity-Name", entityName).AndAlso().WhereEquals("Id", idStringValue);
			var item = itemQuery.SingleOrDefault();//(i => id == i.Id);
			if(item == null)
			{
				throw new Exception(string.Format("No {0} document found for ID {1}", entityName, id));
			}
			return item;
		}

		public static object Load(this IDocumentSession session, Type type, string id)
		{
			var entityName = session.Advanced.DocumentStore.Conventions.GetTypeTagName(type);
			var itemQuery = session.Advanced.LuceneQuery<dynamic>().WhereEquals("@metadata.Raven-Entity-Name", entityName).AndAlso().WhereEquals("Id", id);
			var item = itemQuery.SingleOrDefault(i => i.Id == id);
			if(item == null)
			{
				throw new Exception(string.Format("No {0} document found for ID {1}", entityName, id));
			}
			return item;
		}
	}
}
