using System.Collections.Generic;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using MMDB.DataService.Data;
using MMDB.DataService.Data.Metadata;
using Raven.Client;

namespace MMDB.DataService.Web
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		private IDocumentStore DocumentStore { get; set; }
		
		public static List<NavigationItem> NavigationItems { get; private set; }

		static MvcApplication()
		{
			MvcApplication.NavigationItems = new List<NavigationItem>();
		}

		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);

		}

		protected void Application_Start() 
		{
			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);

			//var pathProvider = App_Start.NinjectWebCommon.Get<DataServicePathProvider>();
			//HostingEnvironment.RegisterVirtualPathProvider(pathProvider);

			MvcApplication.NavigationItems.Add(new NavigationItem("Diagnostics", "Events"));
			MvcApplication.NavigationItems.Add(new NavigationItem("JobManager", "JobManager"));
			//var customControllerManager = App_Start.NinjectWebCommon.Get<ICustomControllerManager>();
			//customControllerManager.RegisterRoutes(RouteTable.Routes, MvcApplication.NavigationItems);
		}
	}
}