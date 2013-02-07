using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MMDB.DataService.Data.Dto.Jobs;
using MMDB.DataService.Data.Settings;

namespace MMDB.DataService.Web.Models
{
	public class ExportSettingsViewModel
	{
		public List<SettingsContainer> SettingsContainerList { get; set; }
		public List<JobDefinition> JobDefinitionList { get; set; }
	}
}