using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MMDB.DataService.Data.Dto;
using MMDB.DataService.Data.Dto.Jobs;

namespace MMDB.DataService.Web.Models
{
	public class JobQueueDataListViewModel
	{
		public EnumJobStatus Status { get; set; }
		public JobDefinition JobDefinition { get; set; }
		public List<JobData> JobDataList { get; set; }

	}
}