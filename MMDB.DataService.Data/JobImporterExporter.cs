using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Jobs;
using Raven.Client;
using Raven.Imports.Newtonsoft.Json;

namespace MMDB.DataService.Data
{
	public class JobImporterExporter : IJobImporterExporter
	{
		private IDocumentSession DocumentSession { get; set; }
		private IJobManager JobManager { get; set; }

		public JobImporterExporter(IDocumentSession documentSession, IJobManager jobManager)
		{
			this.DocumentSession = documentSession;
			this.JobManager = jobManager;
		}

		public string ExportJobs(List<int> selectedJobIds = null)
		{
			List<JobDefinition> jobDefinitionList;
			if (selectedJobIds != null && selectedJobIds.Count > 0)
			{
				jobDefinitionList = new List<JobDefinition>();
				foreach (int id in selectedJobIds)
				{
					var job = this.JobManager.GetJobDefinition(id);
					jobDefinitionList.Add(job);
				}
			}
			else 
			{
				jobDefinitionList = this.JobManager.LoadJobList();
			}

			var serializer = this.DocumentSession.Advanced.DocumentStore.Conventions.CreateSerializer();
			using (var writer = new StringWriter())
			{
				serializer.Serialize(writer, jobDefinitionList);
				string json = writer.ToString();
				return json;
			}
			throw new NotImplementedException();
		}

		public void ExportJobsToFile(string path, List<int> selectedJobIds = null)
		{
			string json = ExportJobs(selectedJobIds);
			File.WriteAllText(path, json);
		}

		public void ImportJobs(string jobsJson)
		{
			var serializer = this.DocumentSession.Advanced.DocumentStore.Conventions.CreateSerializer();
			using (var reader = new StringReader(jobsJson))
			using (var jsonReader = new JsonTextReader(reader))
			{
				var data = serializer.Deserialize<List<JobDefinition>>(jsonReader);
				this.JobManager.ImportJobs(data);
			}
		}

		public void ImportJobsFromFile(string path)
		{
			string jobsJson = File.ReadAllText(path);
			this.ImportJobs(jobsJson);
		}
	}
}
