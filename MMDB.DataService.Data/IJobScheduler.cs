using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data
{
	public interface IJobScheduler
	{
		void StartJobs();
		void StopJobs();
		void RunJobNow(int jobID);
	}
}
