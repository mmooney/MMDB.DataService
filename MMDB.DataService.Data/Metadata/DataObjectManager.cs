using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Raven.Client;

namespace MMDB.DataService.Data.Metadata
{
	public class DataObjectManager
	{
		private IDocumentSession DocumentSession { get; set; }

		public DataObjectManager(IDocumentSession documentSession)
		{
			this.DocumentSession = documentSession;
		}

		public List<DataObjectMetadata> GetMetadataList()
		{
			return this.DocumentSession.Query<DataObjectMetadata>().ToList();
		}

		public DataObjectMetadata CreateMetadata(string objectName, string assemblyName, string className)
		{
			var item = new DataObjectMetadata
			{
				ObjectName = objectName,
				AssemblyName = assemblyName,
				ClassName = className
			};
			this.DocumentSession.Store(item);
			this.DocumentSession.SaveChanges();
			return item;
		}

		public DataObjectMetadata GetMetadata(int id)
		{
			return this.DocumentSession.Load<DataObjectMetadata>(id);
		}

		public DataObjectMetadata GetMetadata(string objectName)
		{
			return this.DocumentSession.Query<DataObjectMetadata>().Single(i=>i.ObjectName == objectName);
		}

		public IList<object> LoadData(int id)
		{
			var metadata = this.GetMetadata(id);
			return this.LoadData(metadata);
		}

		public IList<object> LoadData(DataObjectMetadata metadata)
		{
			var assembly = Assembly.Load(metadata.AssemblyName.Replace(".dll", ""));
			var type = assembly.GetType(metadata.ClassName);
			var entityName = this.DocumentSession.Advanced.DocumentStore.Conventions.GetTypeTagName(type);
			var itemQuery = this.DocumentSession.Advanced.LuceneQuery<object>().WhereEquals("@metadata.Raven-Entity-Name", entityName).ToList();
			return itemQuery;
		}

		public object GetDataObject(DataObjectMetadata metadata, int objectId)
		{
			var assembly = Assembly.Load(metadata.AssemblyName.Replace(".dll", ""));
			var type = assembly.GetType(metadata.ClassName);
			var entityName = this.DocumentSession.Advanced.DocumentStore.Conventions.GetTypeTagName(type);
			var itemQuery = this.DocumentSession.Advanced.LuceneQuery<dynamic>().WhereEquals("@metadata.Raven-Entity-Name", entityName);
			var item = itemQuery.Single(i=>i.Id == objectId);
			return item;
		}


		public object GetDataObject(string objectName, int objectId)
		{
			var metadata = this.GetMetadata(objectName);
			return this.GetDataObject(metadata, objectId);
		}
	}
}
