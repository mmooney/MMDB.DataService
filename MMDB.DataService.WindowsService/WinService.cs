using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using MMDB.DataService.Data;

namespace MMDB.DataService.WindowsService
{
	partial class WinService : ServiceBase
	{
		private JobManager JobManager { get; set; }

		public WinService(JobManager jobManager)
		{
			InitializeComponent();
			this.JobManager = jobManager;
		}

		protected override void OnStart(string[] args)
		{
			this.JobManager.StartJobs();
		}

		protected override void OnStop()
		{
			// TODO: Add code here to perform any tear-down necessary to stop your service.
		}

		public void DebugStart()
		{
			this.OnStart(null);
			while(true)
			{
				Thread.Sleep(1000);
			}
		}
	}
}
