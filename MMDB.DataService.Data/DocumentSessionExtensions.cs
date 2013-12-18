using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Raven.Client.Linq;
using System.Linq.Expressions;
using System.Transactions;
using PagedList;
using MMDB.DataService.Data.Dto;
using MMDB.Shared;

namespace Raven.Client
{
	public static class DocumentSessionExtensions
	{
		public static object Load(this IDocumentSession session, Type type, int id)
		{
			var entityName = session.Advanced.DocumentStore.Conventions.GetTypeTagName(type);
			string idStringValue = entityName + "/" + id.ToString();
			var itemQuery = session.Advanced.LuceneQuery<dynamic>().WaitForNonStaleResultsAsOfNow(TimeSpan.FromSeconds(120)).WhereEquals("@metadata.Raven-Entity-Name", entityName).AndAlso().WhereEquals("Id", idStringValue);
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
			var itemQuery = session.Advanced.LuceneQuery<dynamic>().WaitForNonStaleResultsAsOfNow(TimeSpan.FromSeconds(120)).WhereEquals("@metadata.Raven-Entity-Name", entityName).AndAlso().WhereEquals("Id", id);
			var item = itemQuery.SingleOrDefault(i => i.Id == id);
			if(item == null)
			{
				throw new Exception(string.Format("No {0} document found for ID {1}", entityName, id));
			}
			return item;
		}

        public static IRavenQueryable<T> QueryNoCache<T>(this IDocumentSession documentSession)
        {
            return documentSession.Query<T>().Customize(i => i.NoCaching()).Customize(i => i.NoTracking());
        }

        public static IRavenQueryable<T> QueryNoCacheNotStale<T>(this IDocumentSession documentSession)
        {
            return documentSession.Query<T>().Customize(i => i.NoCaching()).Customize(i => i.NoTracking()).Customize(i => i.WaitForNonStaleResultsAsOfNow());
        }

        public static IRavenQueryable<T> QueryNotStale<T>(this IDocumentSession documentSession)
        {
            return documentSession.Query<T>().Customize(i => i.WaitForNonStaleResultsAsOfLastWrite());
        }

        public static T LoadNoCache<T>(this IDocumentSession session, string id)
        {
            var returnValue = session.Load<T>(id);
            if (returnValue != null)
            {
                session.Advanced.Evict(returnValue);
            }
            return returnValue;
        }

        public static T LoadEnsure<T>(this IDocumentSession documentSession, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(string.Format("Missing {0} id", typeof(T).Name));
            }
            var returnValue = documentSession.Load<T>(id);
            if (returnValue == null)
            {
                throw new RecordNotFoundException(typeof(T), "Id", id);
            }
            return returnValue;
        }

        public static T LoadEnsureNoCache<T>(this IDocumentSession documentSession, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(string.Format("Missing {0} id", typeof(T).Name));
            }
            var returnValue = documentSession.Load<T>(id);
            if (returnValue == null)
            {
                throw new RecordNotFoundException(typeof(T), "Id", id);
            }
            documentSession.Advanced.Evict(returnValue);
            return returnValue;
        }

        public static T StoreSaveEvict<T>(this IDocumentSession documentSession, T data)
        {
            documentSession.Store(data);
            documentSession.SaveChanges();
            documentSession.Advanced.Evict(data);
            return data;
        }

        public static T DeleteSaveEvict<T>(this IDocumentSession documentSession, T data)
        {
            documentSession.Delete(data);
            documentSession.SaveChanges();
            documentSession.Advanced.Evict(data);
            return data;
        }

        public static T SaveEvict<T>(this IDocumentSession documentSession, T data)
        {
            documentSession.SaveChanges();
            documentSession.Advanced.Evict(data);
            return data;
        }

        public static T Pop<T, SortKey>(this IDocumentSession documentSession, Expression<Func<T, bool>> matchExpr, Expression<Func<T, SortKey>> sortExpr, Action<T> updateAction)
            where T : class
        {
            string itemId = null;
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
            {
                var tempItem = documentSession.QueryNoCache<T>()
                                        .Customize(i => i.WaitForNonStaleResultsAsOfNow(TimeSpan.FromSeconds(30)))
                                        .OrderBy(sortExpr)
                                        .Where(matchExpr)
                                        .FirstOrDefault();
                if (tempItem == null)
                {
                    return null;
                }

                var idProperty = documentSession.Advanced.DocumentStore.Conventions.GetIdentityProperty(tempItem.GetType());
                itemId = (string)idProperty.GetValue(tempItem, null);
                var reloadedItem = documentSession.LoadEnsure<T>(itemId);
                var matchFunc = matchExpr.Compile();
                if (!matchFunc(reloadedItem))
                {
                    //this._logger.Warn("Stale cleanup task found, actual status: " + reloadedItem.Status.ToString());
                    return null;
                }

                updateAction(reloadedItem);
                documentSession.SaveEvict(reloadedItem);

                transaction.Complete();
            }
            if (string.IsNullOrEmpty(itemId))
            {
                return null;
            }
            else
            {
                return documentSession.LoadEnsureNoCache<T>(itemId);
            }

        }

        public static IPagedList<T> QueryPageAndSort<T>(this IDocumentSession documentSession, ListOptions listOptions, string defaultSortField, bool defaultSortAscending = true, Func<IDocumentQuery<T>, IDocumentQuery<T>> filterFunc = null)
        {
            listOptions = listOptions ?? new ListOptions();
            RavenQueryStatistics stats;
            var documentQuery = documentSession.Advanced.LuceneQuery<T>()
                            .Statistics(out stats);


            if (filterFunc != null)
            {
                documentQuery = filterFunc(documentQuery);
            }
            listOptions.SortField = StringHelper.IsNullOrEmpty(listOptions.SortField, defaultSortField);
            listOptions.SortAscending = listOptions.SortAscending.GetValueOrDefault(defaultSortAscending);
            if (listOptions.SortAscending.Value)
            {
                documentQuery = documentQuery.OrderBy(listOptions.SortField);
            }
            else
            {
                documentQuery = documentQuery.OrderByDescending(listOptions.SortField);
            }

            int pageNumber = listOptions.PageNumber.GetValueOrDefault(1);
            int pageSize = listOptions.PageSize.GetValueOrDefault(20);

            var resultQuery = documentQuery
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize);
            var resultList = resultQuery.ToList();
            return new StaticPagedList<T>(resultList, pageNumber, pageSize, stats.TotalResults);
        }
    }
}
