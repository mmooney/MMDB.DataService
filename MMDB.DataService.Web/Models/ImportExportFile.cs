using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MMDB.DataService.Data.Dto.Jobs;
using MMDB.DataService.Data.Settings;

namespace MMDB.DataService.Web.Models
{
	public class ImportExportFile
	{
		public List<JobDefinition> JobDefinitionList { get; set; }
		public List<SettingsContainer> SettingsContainerList { get; set; }

		public ImportExportFile()
		{
			this.JobDefinitionList = new List<JobDefinition>();
			this.SettingsContainerList = new List<SettingsContainer>();
		}
	}
}