using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Metadata
{
	public interface IDataServiceViewManager
	{
		string GetViewNameForObjectType(string objectTypeName);
		List<DataObjectView> GetViewList();
		string GetViewData(DataObjectView viewObject);
		void CreateView(string objectTypeName, string viewName, string resourceAssemblyName, string resourceIdentifier);
		DataObjectView GetView(int id);
		void UpdateView(int id, string objectTypeName, string viewName, string resourceAssemblyName, string resourceIdentifier);
	}
}
