using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMDB.DataService.Data;
using MMDB.DataService.Data.Settings;
using MMDB.DataService.Web.Models;
using Raven.Imports.Newtonsoft.Json;

namespace MMDB.DataService.Web.Controllers
{
    public class AdminController : Controller
    {
		private JobManager JobManager { get; set; }
		private SettingsManager SettingsManager { get; set; }

		public AdminController(JobManager jobManager, SettingsManager settingsManager)
		{
			this.JobManager = jobManager;
			this.SettingsManager = settingsManager;
		}
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult Export()
		{
			var viewModel = new ExportSettingsViewModel()
			{
				SettingsContainerList = this.SettingsManager.GetList(),
				JobDefinitionList = this.JobManager.LoadJobList()
			};
			return View(viewModel);
		}

		public ActionResult ExportComplete(string selectedJobIDs, string selectedSettingsIDs)
		{
			var viewModel = new ImportExportFile();

			if(!string.IsNullOrEmpty(selectedJobIDs))
			{
				var jobIDList = selectedJobIDs.Split(',').Select(i => int.Parse(i));
				foreach (int id in jobIDList)
				{
					var job = this.JobManager.GetJobDefinition(id);
					viewModel.JobDefinitionList.Add(job);
				}
			}

			if(!string.IsNullOrEmpty(selectedSettingsIDs))
			{
				var settingsIDList = selectedSettingsIDs.Split(',').Select(i=>int.Parse(i));
				foreach(int id in settingsIDList)
				{
					var settings = this.SettingsManager.GetSettings(id);
					viewModel.SettingsContainerList.Add(settings);
				}
			}
			string json = JsonConvert.SerializeObject(viewModel);
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
			return File(bytes, "text/json", "JobExport.txt");
		}
	}
}
