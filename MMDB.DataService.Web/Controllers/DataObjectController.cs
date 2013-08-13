using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMDB.DataService.Data.Metadata;
using MMDB.DataService.Web.Models;

namespace MMDB.DataService.Web.Controllers
{
    public class DataObjectController : Controller
    {
		private IDataObjectManager DataObjectManager { get; set; }
		private IDataServiceViewManager ViewManager { get; set; }

		public DataObjectController(IDataObjectManager dataObjectManager, IDataServiceViewManager viewManager)
		{
			this.DataObjectManager = dataObjectManager;
			this.ViewManager = viewManager;
		}
        //
        // GET: /DataObject/

        public ActionResult Index()
        {
			var metadataList = this.DataObjectManager.GetMetadataList();
            return View(metadataList);
        }

		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Create(string objectName, string assemblyName, string className)
		{
			this.DataObjectManager.CreateMetadata(objectName, assemblyName, className);
			return RedirectToAction("Index");
		}

		public ActionResult Details(int id)
		{
			var viewModel = new MetadataDetailsViewModel
			{
				Metadata = this.DataObjectManager.GetMetadata(id),
			};
			viewModel.DataList = this.DataObjectManager.LoadData(viewModel.Metadata);
			return View(viewModel);
		}

		public ActionResult ViewDataItem(string objectName, int objectId)
		{
			var item = this.DataObjectManager.GetDataObject(objectName, objectId);
			string viewName = this.ViewManager.GetViewNameForObjectType(objectName);
			return View(viewName, item);
		}

		public ActionResult Edit(int id)
		{
			var item = this.DataObjectManager.GetMetadata(id);
			return View(item);
		}

		[HttpPost]
		public ActionResult Edit(int id, string objectName, string assemblyName, string className)
		{
			this.DataObjectManager.UpdateMetadata(id, objectName, assemblyName, className);
			return RedirectToAction("Index");
		}

    }
}
