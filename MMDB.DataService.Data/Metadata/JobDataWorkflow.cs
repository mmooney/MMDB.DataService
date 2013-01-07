using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Metadata
{
	public class JobDataWorkflow
	{
		public int Id { get; set; }
		public string WorkflowName { get; set; }
		public List<JobDataQueue> WorkflowQueueList { get; set; }

		public JobDataWorkflow()
		{
			this.WorkflowQueueList = new List<JobDataQueue>();
		}
	}
}
