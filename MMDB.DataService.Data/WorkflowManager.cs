using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Metadata;
using Raven.Client;

namespace MMDB.DataService.Data
{
	public class WorkflowManager
	{
		private IDocumentSession DocumentSession { get; set; }

		public WorkflowManager(IDocumentSession documentSession)
		{
			this.DocumentSession = documentSession;
		}

		public IEnumerable<JobDataWorkflow> GetList()
		{
			return this.DocumentSession.Query<JobDataWorkflow>().ToList();
		}

		public JobDataWorkflow CreateWorkflow(string workflowName)
		{
			var newItem = new JobDataWorkflow
			{
				WorkflowName = workflowName
			};
			this.DocumentSession.Store(newItem);
			this.DocumentSession.SaveChanges();
			return newItem;
		}

		public JobDataWorkflow GetWorkflow(int id)
		{
			return this.DocumentSession.Load<JobDataWorkflow>(id);
		}
	}
}
