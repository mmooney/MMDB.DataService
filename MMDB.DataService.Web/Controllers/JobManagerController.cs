using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMDB.DataService.Data;
using MMDB.DataService.Web.Models;
using MMDB.DataService.Data.Dto.Jobs;
using MMDB.DataService.Data.Dto;
using MMDB.DataService.Data.Jobs;
using Raven.Imports.Newtonsoft.Json;
using System.IO;
using MMDB.Shared;

namespace MMDB.DataService.Web.Controllers
{
    public class JobManagerController : Controller
    {
        private IJobManager JobManager { get; set; }

		public JobManagerController(IJobManager jobManager)
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
        public ActionResult Create(string jobName, string assemblyName, string className, string schedule, int intervalMinutes=0, int delayStartMinutes=0, string cronScheduleExpression=null)
        {
			JobDefinition job;
			if(schedule == "Simple")
			{
				job = this.JobManager.CreateSimpleJob(jobName, Guid.NewGuid(), assemblyName, className, intervalMinutes, delayStartMinutes);
			}
			else if (schedule == "CRON")
			{
				job = this.JobManager.CreateCronJob(jobName, Guid.NewGuid(), assemblyName, className, cronScheduleExpression);
			}
			else 
			{
				throw new Exception("Unexpected schedule value: " + schedule);
			}
            return RedirectToAction("Edit", new {id = job.Id});
        }

		public ActionResult Edit(int id)
		{
			var jobDefinition = this.JobManager.GetJobDefinition(id);
			this.JobManager.EnsureConfiguration(jobDefinition);
			return View(jobDefinition);
		}

		[HttpPost]
		public ActionResult Edit(int id, string jobName, string assemblyName, string className, string schedule, string configJson, int intervalMinutes=0, int delayStartMinutes=0, string cronScheduleExpression=null)
		{
			var jobDefinition = this.JobManager.LoadJobDefinition(id);
			this.JobManager.EnsureConfiguration(jobDefinition);
			if(string.IsNullOrEmpty(configJson))
			{
				configJson = this.Request.Form["Configuration.configJson"];
			}
			if(jobDefinition.Configuration != null && !string.IsNullOrEmpty(configJson))
			{
				var configType = jobDefinition.Configuration.GetType();
				jobDefinition.Configuration = (JobConfigurationBase)JsonConvert.DeserializeObject(configJson,configType); 
				//foreach(var key in form.AllKeys.Where(i=>i.StartsWith("Configuration.")))
				//{
				//	string configKey = key.Substring("Configuration.".Length);
				//	var propInfo = configType.GetProperty(configKey);
				//	object convertedValue;
				//	if(string.IsNullOrEmpty(form[key]))
				//	{
				//		convertedValue = null;
				//	}
				//	else 
				//	{
				//		convertedValue = ConvertValue(propInfo.PropertyType, form[key]);
				//	}
				//	propInfo.SetValue(jobDefinition.Configuration, convertedValue, null);
				//}
			}
			if (schedule == "Simple")
			{
				this.JobManager.UpdateSimpleJob(id, jobName, jobDefinition.JobGuid, assemblyName, className, intervalMinutes, delayStartMinutes, jobDefinition.Configuration);
			}
			else if (schedule == "CRON")
			{
				this.JobManager.UpdateCronJob(id, jobName, jobDefinition.JobGuid, assemblyName, className, cronScheduleExpression, jobDefinition.Configuration);
			}
			else
			{
				throw new Exception("Unexpected schedule value: " + schedule);
			}
			return RedirectToAction("Index");
		}

		private object ConvertValue(Type propertyType, string value)
		{
			object returnValue = null;
			if (value == null)
			{
				returnValue = Convert.ChangeType(value, propertyType);
			}
			else
			{
				if (propertyType == typeof(int) || propertyType == typeof(int?))
				{
					returnValue = int.Parse(value);
				}
				else if (propertyType == typeof(double) || propertyType == typeof(double?))
				{
					returnValue = double.Parse(value);
				}
				else if (propertyType == typeof(float) || propertyType == typeof(float?))
				{
					returnValue = float.Parse(value);
				}
				else if (propertyType == typeof(long) || propertyType == typeof(long?))
				{
					returnValue = long.Parse(value);
				}
				else
				{
					returnValue = Convert.ChangeType(value, propertyType);
				}
			}
			return returnValue;
		}

        // GET: /JobManager/Delete/5
 
        public ActionResult Delete(int id)
        {
			var item = this.JobManager.LoadJobDefinition(id);
            return View(item);
        }

        //
        // POST: /JobManager/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
			this.JobManager.DeleteJobDefinition(id);
            return RedirectToAction("Index");
        }

		public ActionResult Status()
		{
			var list = this.JobManager.GetAllJobStatus();
			return View(list);
		}

		public ActionResult QueueStatusList(int jobDefinitionId, EnumJobStatus status, int? page)
		{
			var jobDefinition = this.JobManager.GetJobDefinition(jobDefinitionId);
			var query = this.JobManager.GetJobDataQueue(jobDefinition);
			query = query.ByStatus(status);
			var viewModel = new JobQueueDataListViewModel()
			{
				Status = status,
				JobDefinition = jobDefinition,
				JobDataList = query.ToList()
			};
			return View(viewModel);
		}

		public ActionResult JobDataDetails(int jobDefinitionId, int jobDataId)
		{
			var query = this.JobManager.GetJobDataQueue(jobDefinitionId);
			var item = query.Single(i=>i.Id == jobDataId);
			return View(item);
		}

		[HttpPost]
		public ActionResult JobDataDetails(int jobDefinitionId, int jobDataId, EnumJobStatus status)
		{
			var query = this.JobManager.GetJobDataQueue(jobDefinitionId);
			var item = query.Single(i => i.Id == jobDataId);
			this.JobManager.UpdateJobDataStatus(item, status);
			return RedirectToAction("JobDataDetails", new { jobDefinitionId, jobDataId });
		}

		public ActionResult JobDataDelete(int jobDefinitionId, int jobDataId)
		{
			var query = this.JobManager.GetJobDataQueue(jobDefinitionId);
			var item = query.Single(i => i.Id == jobDataId);
			this.ViewBag.JobDefinitionId = jobDefinitionId;
			return View(item);
		}

		[HttpPost]
		public ActionResult JobDataDelete(int jobDefinitionId, int jobDataId, FormCollection form)
		{
			var query = this.JobManager.GetJobDataQueue(jobDefinitionId);
			var item = query.Single(i => i.Id == jobDataId);
			this.JobManager.DeleteJobData(item);
			return this.RedirectToAction("QueueStatusList", new { jobDefinitionId, status = item.Status });
		}

		public ActionResult Export()
		{
			var jobDefinitionList = this.JobManager.LoadJobList();
			return View(jobDefinitionList);
		}

		public ActionResult ExportComplete(string SelectedJobIDs)
		{
			var idList = SelectedJobIDs.Split(',').Select(i=>int.Parse(i));
			List<JobDefinition> jobList = new List<JobDefinition>();
			foreach(int id in idList)
			{
				var job = this.JobManager.GetJobDefinition(id);
				jobList.Add(job);
			}
			string json = JsonConvert.SerializeObject(jobList);
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
			return File(bytes, "text/json", "JobExport.txt");
		}
	}
}
