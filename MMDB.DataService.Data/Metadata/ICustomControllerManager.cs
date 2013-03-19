using System;
using System.Collections.Generic;
using System.Web.Routing;

namespace MMDB.DataService.Data.Metadata
{
	public interface ICustomControllerManager
	{
		void RegisterRoutes(RouteCollection routeCollection, List<NavigationItem> navigationList);
		IEnumerable<CustomControllerMetadata> GetControllerList();
		CustomControllerMetadata Create(string controllerName, string className);
		CustomControllerMetadata Load(int id);
		CustomControllerMetadata Update(int id, string controllerName, string className);
		void Delete(int id);
	}
}
