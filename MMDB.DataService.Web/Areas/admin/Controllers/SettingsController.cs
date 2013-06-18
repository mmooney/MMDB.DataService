using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMDB.DataService.Data;
using MMDB.DataService.Data.Settings;
using MMDB.DataService.Web.Models;
using MMDB.Shared;
using Raven.Client;
using Raven.Imports.Newtonsoft.Json;

namespace MMDB.DataService.Web.Areas.admin.Controllers
{
    public class SettingsController : Controller
    {
		private IJobManager JobManager { get; set; }
		private SettingsManager SettingsManager { get; set; }
		private IDocumentSession DocumentSession { get; set; }

		public SettingsController(IJobManager jobManager, SettingsManager settingsManager, IDocumentSession documentSession)
		{
			this.JobManager = jobManager;
			this.SettingsManager = settingsManager;
			this.DocumentSession = documentSession;
		}
		public ActionResult Index()
        {
            return View();
        }

		public ActionResult Import()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Import(HttpPostedFileBase fileData)
		{
			string fileString = StreamHelper.ReadAll(fileData.InputStream);
			var serializer = this.DocumentSession.Advanced.DocumentStore.Conventions.CreateSerializer();
			using (var streamReader = new StreamReader(fileData.InputStream))
			{
				using (var jsonReader = new JsonTextReader(streamReader))
				{
					var data = serializer.Deserialize<ImportExportFile>(jsonReader);
					this.JobManager.ImportJobs(data.JobDefinitionList);
					this.SettingsManager.ImportSettings(data.SettingsContainerList);
					return RedirectToAction("ImportComplete");
				}
			}
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

			if (!string.IsNullOrEmpty(selectedJobIDs))
			{
				var jobIDList = selectedJobIDs.Split(',').Select(i => int.Parse(i));
				foreach (int id in jobIDList)
				{
					var job = this.JobManager.GetJobDefinition(id);
					viewModel.JobDefinitionList.Add(job);
				}
			}

			if (!string.IsNullOrEmpty(selectedSettingsIDs))
			{
				var settingsIDList = selectedSettingsIDs.Split(',').Select(i => int.Parse(i));
				foreach (int id in settingsIDList)
				{
					var settings = this.SettingsManager.GetSettings(id);
					viewModel.SettingsContainerList.Add(settings);
				}
			}

			var serializer = this.DocumentSession.Advanced.DocumentStore.Conventions.CreateSerializer();
			using (var writer = new StringWriter())
			{
				serializer.Serialize(writer, viewModel);
				string json = writer.ToString();
				//string json = JsonConvert.SerializeObject(viewModel);
				byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
				return File(bytes, "text/json", "JobExport.txt");
			}
		}
    }
}
