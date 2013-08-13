using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Metadata
{
	public interface IDataObjectManager
	{
		List<DataObjectMetadata> GetMetadataList();
		DataObjectMetadata CreateMetadata(string objectName, string assemblyName, string className);
		DataObjectMetadata GetMetadata(int id);
		DataObjectMetadata GetMetadata(string objectName);
		IList<object> LoadData(int id);
		IList<object> LoadData(DataObjectMetadata metadata);
		object GetDataObject(DataObjectMetadata metadata, int objectId);
		object GetDataObject(string objectName, int objectId);
		void UpdateMetadata(int id, string objectName, string assemblyName, string className);
	}
}
