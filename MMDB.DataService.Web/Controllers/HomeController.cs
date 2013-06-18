using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMDB.DataService.Data;
using MMDB.DataService.Web.Models;

namespace MMDB.DataService.Web.Controllers
{
    public class HomeController : Controller
    {
		private IJobManager JobManager { get; set; }

		public HomeController(IJobManager jobManager)
		{
			this.JobManager = jobManager;
		}
        //
        // GET: /Home/

        public ActionResult Index()
        {
			var viewModel = new HomePageViewModel()
			{
				JobStatusList = this.JobManager.GetAllJobStatus()
			};
			return View(viewModel);
        }

    }
}
