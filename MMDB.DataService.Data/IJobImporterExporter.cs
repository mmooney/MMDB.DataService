using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data
{
	public interface IJobImporterExporter
	{
		string ExportJobs(List<int> selectedJobIds = null);
		void ExportJobsToFile(string path, List<int> selectedJobIds = null);

		void ImportJobs(string jobsJson);
		void ImportJobsFromFile(string path);
	}
}
