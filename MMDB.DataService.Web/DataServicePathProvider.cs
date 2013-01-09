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
		private DataServiceViewManager ViewManager { get; set; }
		public const string CacheKey = "DataServiceViewList";

		public DataServicePathProvider(DataServiceViewManager viewManager)
		{
			this.ViewManager = viewManager;
		}

		public override bool FileExists(string virtualPath)
		{
			var viewList = GetViewList();
			var matchingView = viewList.Any(i=>virtualPath.Equals(i.VirtualPath, StringComparison.CurrentCultureIgnoreCase));
			if(matchingView)
			{
				return true;
			}
			else 
			{
				return base.FileExists(virtualPath);
			} 
		}

		public override VirtualDirectory GetDirectory(string virtualDir)
		{
			return base.GetDirectory(virtualDir);
		}

		public override VirtualFile GetFile(string virtualPath)
		{
			var viewList = GetViewList();
			var matchingView = viewList.FirstOrDefault(i => virtualPath.Equals(i.VirtualPath, StringComparison.CurrentCultureIgnoreCase));
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