using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMDB.DataService.Data;
using MvcContrib.Pagination;
using MMDB.DataService.Data.Dto.Logging;
using MMDB.DataService.Web.Models;
using MMDB.Shared;

namespace MMDB.DataService.Web.Controllers
{
    public class EventsController : Controller
    {
		private DataServiceLogger Logger { get; set; }

		public EventsController(DataServiceLogger logger)
		{
			this.Logger = logger;
		}
        //
        // GET: /Events/

		public ActionResult Index(int? page, int? pageSize, EnumServiceMessageLevel? level, bool? json)
        {
			this.ViewBag.Level = level;

			var realPageSize = pageSize.GetValueOrDefault(20);
			var query = this.Logger.GetEventList(page.GetValueOrDefault(1) - 1, realPageSize, level);
			var viewModel = new CustomPagination<ServiceMessage>
				(query, 
				page.GetValueOrDefault(1),
				realPageSize,
				this.Logger.GetEventCount(level));
			if(json.GetValueOrDefault(false))
			{
				return Json(viewModel, JsonRequestBehavior.AllowGet);
			}
			else 
			{
	            return View(viewModel);
			}
        }

        public ActionResult Details(int id)
        {
			var item = this.Logger.GetEventItem(id);
			return View(item);
		}

        // GET: /Events/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Events/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
