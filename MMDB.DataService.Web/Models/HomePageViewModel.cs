using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MMDB.DataService.Data.Dto.Jobs;

namespace MMDB.DataService.Web.Models
{
	public class HomePageViewModel
	{
		public List<JobStatus> JobStatusList { get; set; }
	}
}