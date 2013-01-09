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
		private DataObjectManager DataObjectManager { get; set; }

		public DataObjectController(DataObjectManager dataObjectManager)
		{
			this.DataObjectManager = dataObjectManager;
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

		public ActionResult ViewData(string objectName, int objectId)
		{
			var viewModel = new ViewDataViewModel
			{
				Metadata = this.DataObjectManager.GetMetadata(objectName)
			};
			viewModel.DataObject = this.DataObjectManager.GetDataObject(viewModel.Metadata, objectId);
			return View(viewModel);
		}

		public ActionResult ViewDataItem(string objectName, int objectId)
		{
			var item = this.DataObjectManager.GetDataObject(objectName, objectId);
			return View(item);
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
