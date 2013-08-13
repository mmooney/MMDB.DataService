using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using MMDB.DataService.Data.Metadata;

namespace MMDB.DataService.Web
{
	public class DataServicePathProvider : VirtualPathProvider
	{
		private IDataServiceViewManager ViewManager { get; set; }
		public const string CacheKey = "DataServiceViewList";

		public DataServicePathProvider(IDataServiceViewManager viewManager)
		{
			this.ViewManager = viewManager;
		}

		public override bool FileExists(string virtualPath)
		{
			var viewList = GetViewList();
			if (virtualPath.StartsWith("/Views/DataObject/", StringComparison.CurrentCultureIgnoreCase) && virtualPath.EndsWith(".cshtml", StringComparison.CurrentCultureIgnoreCase))
			{
				var matchingView = viewList.Any(i=>virtualPath.Equals("/Views/DataObject/" + i.ViewName + ".cshtml", StringComparison.CurrentCultureIgnoreCase));
				if(matchingView)
				{
					return true;
				}
				else 
				{
					return base.FileExists(virtualPath);
				} 
			}
			else
			{
				var matchingView = viewList.Any(i=>virtualPath.EndsWith("/" + i.ViewName + ".cshtml", StringComparison.CurrentCultureIgnoreCase));
				if(matchingView)
				{
					return true;
				}
				else 
				{
					return base.FileExists(virtualPath);
				}
			}
		}

		public override VirtualDirectory GetDirectory(string virtualDir)
		{
			return base.GetDirectory(virtualDir);
		}

		public override VirtualFile GetFile(string virtualPath)
		{
			var viewList = GetViewList();
			if (virtualPath.StartsWith("/Views/DataObject/", StringComparison.CurrentCultureIgnoreCase) && virtualPath.EndsWith(".cshtml", StringComparison.CurrentCultureIgnoreCase))
			{
				var matchingView = viewList.FirstOrDefault(i => virtualPath.Equals("/Views/DataObject/" + i.ViewName + ".cshtml", StringComparison.CurrentCultureIgnoreCase));
				if (matchingView != null)
				{
					string viewData = this.ViewManager.GetViewData(matchingView);
					return new DataServiceVirtualFile(viewData, virtualPath);
				}
				else
				{
					return base.GetFile(virtualPath);
				}
			}
			else 
			{
				var matchingView = viewList.FirstOrDefault(i => virtualPath.EndsWith("/" + i.ViewName + ".cshtml", StringComparison.CurrentCultureIgnoreCase));
				if (matchingView != null)
				{
					string viewData = this.ViewManager.GetViewData(matchingView);
					return new DataServiceVirtualFile(viewData, virtualPath);
				}
				else
				{
					return base.GetFile(virtualPath);
				}
			}
		}

		public override System.Web.Caching.CacheDependency GetCacheDependency(string virtualPath, System.Collections.IEnumerable virtualPathDependencies, DateTime utcStart)
		{
			var viewList = GetViewList();
			var matchingView = viewList.FirstOrDefault(i => virtualPath.EndsWith("/" + i.ViewName + ".cshtml", StringComparison.CurrentCultureIgnoreCase));
			if (matchingView != null)
			{
				return null;
			}
			else
			{
				return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
			}
		}

		private List<DataObjectView> GetViewList()
		{
			var viewList = (List<DataObjectView>)HttpContext.Current.Cache[DataServicePathProvider.CacheKey];
			if (viewList == null)
			{
				viewList = this.ViewManager.GetViewList();
				HttpContext.Current.Cache.Insert(DataServicePathProvider.CacheKey, viewList, null, DateTime.Now.AddMinutes(1), System.Web.Caching.Cache.NoSlidingExpiration);
			}
			return viewList;
		}

	}
}