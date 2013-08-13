using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Raven.Client;

namespace MMDB.DataService.Data.Metadata.MetadataImpl
{
	public class DataObjectManager : IDataObjectManager
	{
		private IDocumentSession DocumentSession { get; set; }
		private TypeLoader TypeLoader { get; set; }

		public DataObjectManager(IDocumentSession documentSession, TypeLoader typeLoader)
		{
			this.DocumentSession = documentSession;
			this.TypeLoader = typeLoader;
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
			var type = this.TypeLoader.LoadType(metadata.AssemblyName, metadata.ClassName);
			var entityName = this.DocumentSession.Advanced.DocumentStore.Conventions.GetTypeTagName(type);
			var itemQuery = this.DocumentSession.Advanced.LuceneQuery<object>().WhereEquals("@metadata.Raven-Entity-Name", entityName).ToList();
			return itemQuery;
		}

		public object GetDataObject(DataObjectMetadata metadata, int objectId)
		{
			var type = this.TypeLoader.LoadType(metadata.AssemblyName, metadata.ClassName);
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

		public void UpdateMetadata(int id, string objectName, string assemblyName, string className)
		{
			var item = this.GetMetadata(id);
			item.ObjectName = objectName;
			item.AssemblyName = assemblyName;
			item.ClassName = className;
			this.DocumentSession.SaveChanges();
		}
	}
}
