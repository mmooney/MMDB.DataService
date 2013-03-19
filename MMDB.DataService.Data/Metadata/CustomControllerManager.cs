using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Raven.Client;

namespace MMDB.DataService.Data.Metadata
{
	public class CustomControllerManager : ICustomControllerManager
	{
		private IDocumentSession DocumentSession { get; set; }

		public CustomControllerManager(IDocumentSession documentSession)
		{
			this.DocumentSession = documentSession;

		}

		public void RegisterRoutes(RouteCollection routes, List<NavigationItem> navigationItems)
		{
			var controllerList = this.GetControllerList();
			foreach(var controller in controllerList)
			{
				routes.MapRoute(controller.ControllerName, controller.ControllerName + "/{action}/{id}", new { controller = controller.ClassName, action = "Index" }, new { id = UrlParameter.Optional });
				navigationItems.Add(new NavigationItem(controller.ControllerName));
			}
		}

		public IEnumerable<CustomControllerMetadata> GetControllerList()
		{
			return this.DocumentSession.Query<CustomControllerMetadata>().Customize(i => i.WaitForNonStaleResultsAsOfNow()).ToList();
		}


		public CustomControllerMetadata Create(string controllerName, string className)
		{
			var newItem = new CustomControllerMetadata
			{
				ControllerName = controllerName,
				ClassName = className
			};
			this.DocumentSession.Store(newItem);
			this.DocumentSession.SaveChanges();
			return newItem;
		}


		public CustomControllerMetadata Load(int id)
		{
			return this.DocumentSession.Load<CustomControllerMetadata>(id);
		}

		public CustomControllerMetadata Update(int id, string controllerName, string className)
		{
			var item = this.DocumentSession.Load<CustomControllerMetadata>(id);
			item.ControllerName = controllerName;
			item.ClassName = className;
			this.DocumentSession.SaveChanges();
			return item;
		}


		public void Delete(int id)
		{
			var item = this.DocumentSession.Load<CustomControllerMetadata>(id);
			this.DocumentSession.Delete(item);
			this.DocumentSession.SaveChanges();
		}
	}
}
