using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMDB.DataService.Data;

namespace MMDB.DataService.Web.Controllers
{
    public class WorkflowController : Controller
    {
		private WorkflowManager WorkflowManager { get; set; }

		public WorkflowController(WorkflowManager workflowManager)
		{
			this.WorkflowManager = workflowManager;
		}
        //
        // GET: /Workflow/

        public ActionResult Index()
        {
			var workflowList = this.WorkflowManager.GetList();
            return View(workflowList);
        }

		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Create(string workflowName)
		{
			var workflow = this.WorkflowManager.CreateWorkflow(workflowName);
			return RedirectToAction("Edit", new { id=workflow.Id });
		}

		public ActionResult Edit(int id)
		{
			var workflow = this.WorkflowManager.GetWorkflow(id);
			return View(workflow);
		}
    }
}
