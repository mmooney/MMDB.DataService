using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MMDB.DataService.Data.Dto.Assemblies;

namespace MMDB.DataService.Web.Models
{
	public class CreateJobDefinitionViewModel
	{
		public List<JobAssembly> AssemblyList { get; set; }
	}
}