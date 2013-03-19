using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMDB.DataService.Data;
using MMDB.DataService.Data.Metadata;

namespace MMDB.DataService.Web.Areas.admin.Controllers
{
    public class CustomControllersController : Controller
    {
		private ICustomControllerManager CustomControllerManager { get; set; }

		public CustomControllersController(ICustomControllerManager customControllerManager)
		{
			this.CustomControllerManager = customControllerManager;
		}

        public ActionResult Index()
        {
			var list = this.CustomControllerManager.GetControllerList();
            return View(list);
        }

		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Create(string controllerName, string className)
		{
			this.CustomControllerManager.Create(controllerName, className);
			return RedirectToAction("Index");
		}

		public ActionResult Edit(int id)
		{
			var item = this.CustomControllerManager.Load(id);
			return View(item);
		}

		[HttpPost]
		public ActionResult Edit(int id, string controllerName, string className)
		{
			this.CustomControllerManager.Update(id, controllerName, className);
			return RedirectToAction("Index");
		}

		public ActionResult Delete(int id)
		{
			var item = this.CustomControllerManager.Load(id);
			return View(item);
		}

		[HttpPost]
		public ActionResult Delete(int id, FormCollection form)
		{
			this.CustomControllerManager.Delete(id);
			return RedirectToAction("Index");
		}
    }
}
