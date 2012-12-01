using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using System.Web.Routing;
using System.Web;

namespace MMDB.RavenDB.DynamicData
{
	public class RavenDataHandler : IRouteHandler, IHttpHandler
	{
		public static void Register(IDocumentStore documentStore, string prefix)
		{
			var routes = RouteTable.Routes;
			var urls = new []
			{
				""
			};
			var handler = new RavenDataHandler();
			using (routes.GetWriteLock())
			{
				foreach (var url in urls)
				{
					var route = new Route(prefix + url, handler)
					{
						// we have to specify these, so no MVC route helpers will match, e.g. @Html.ActionLink("Home", "Index", "Home")
						Defaults = new RouteValueDictionary(new { controller = "WidgetMetadataHandler", action = "ProcessRequest" })
					};

					// put our routes at the beginning, like a boss
					routes.Insert(0, route);
				}
			}
		}

		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			return this;
		}


		public bool IsReusable
		{
			get { return true; }
		}

		public void ProcessRequest(HttpContext context)
		{
			string output;
			string path = context.Request.AppRelativeCurrentExecutionFilePath;

			//switch (Path.GetFileNameWithoutExtension(path).ToLower())
			//{
			//    case "metadata":
			//        output = GenerateAssemblyMetadataHTML();
			//        break;
			//    case "testwidgetaction":
			//        output = GenerateTestWidget(context);
			//        break;
			//    case "genericproxy":
			//        output = GenericProxy(context);
			//        break;
			//    case "defaultwidgetcss":
			//        output = GetDefaultWidgetCSS();
			//        break;
			//    case "clearcache":
			//        output = GetClearCache();
			//        break;
			//    case "":
			//    case "widgetinformation":
			//        output = GetWidgetInformationHTML();
			//        break;
			//    default:
			//        output = NotFound(context);
			//        break;
			//}

			context.Response.Write("'ello");
		}
	}
}
