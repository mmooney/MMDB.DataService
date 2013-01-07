using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMDB.DataService.Data;
using MMDB.DataService.Web.Models;
using MMDB.DataService.Data.Dto.Jobs;

namespace MMDB.DataService.Web.Controllers
{
    public class JobManagerController : Controller
    {
        private JobManager JobManager { get; set; }

		public JobManagerController(JobManager jobManager)
		{
			this.JobManager = jobManager;
		}

        public ActionResult Index()
        {
			var list = JobManager.LoadJobList();
            return View(list);
        }

    
        // GET: /JobManager/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /JobManager/Create

        [HttpPost]
        public ActionResult Create(string jobName, string assemblyName, string className, string schedule, int intervalMinutes, int delayStartMinutes, string cronScheduleExpression)
        {
			if(schedule == "Simple")
			{
	            this.JobManager.CreateSimpleJob(jobName, assemblyName, className, intervalMinutes, delayStartMinutes);
			}
			else if (schedule == "CRON")
			{
				this.JobManager.CreateCronJob(jobName, assemblyName, className, cronScheduleExpression);
			}
			else 
			{
				throw new Exception("Unexpected schedule value: " + schedule);
			}
            return RedirectToAction("Index");
        }

		public ActionResult Edit(int id)
		{
			var item = this.JobManager.LoadJob(id);
			return View(item);
		}        

		[HttpPost]
        public ActionResult Edit(int id, string jobName, string assemblyName, string className, string schedule, int intervalMinutes, int delayStartMinutes, string cronScheduleExpression)
        {
			var item = this.JobManager.LoadJob(id);
			if (schedule == "Simple")
			{
				this.JobManager.UpdateSimpleJob(id, jobName, assemblyName, className, intervalMinutes, delayStartMinutes);
			}
			else if (schedule == "CRON")
			{
				this.JobManager.UpdateCronJob(id, jobName, assemblyName, className, cronScheduleExpression);
			}
			else
			{
				throw new Exception("Unexpected schedule value: " + schedule);
			}
			return RedirectToAction("Index");
		}

        // GET: /JobManager/Delete/5
 
        public ActionResult Delete(int id)
        {
			var item = this.JobManager.LoadJob(id);
            return View(item);
        }

        //
        // POST: /JobManager/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
			this.JobManager.DeleteJob(id);
            return RedirectToAction("Index");
        }

		public ActionResult Status()
		{
			var list = this.JobManager.GetAllJobStatus();
			return View(list);
		}
    }
}
