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
		private IJobScheduler JobScheduler { get; set; }
		private Thread ProcessingThread { get; set; }
		private volatile bool _stopRequested;

		public WinService(IJobScheduler jobScheduler)
		{
			ServiceStartupLogger.LogMessage("Started WinService.ctor");
			InitializeComponent();
			this.JobScheduler = jobScheduler;
			ServiceStartupLogger.LogMessage("Done WinService.ctor");
		}

		protected override void OnStart(string[] args)
		{
			ServiceStartupLogger.LogMessage("Started WinService.OnStart");
			this._stopRequested = false;
			this.ProcessingThread = new Thread(this.ThreadProc);
			this.ProcessingThread.Start();
			ServiceStartupLogger.LogMessage("Done WinService.OnStart");
		}

		protected override void OnStop()
		{
			ServiceStartupLogger.LogMessage("Started WinService.OnStop");
			// TODO: Add code here to perform any tear-down necessary to stop your service.
			this._stopRequested = true;
			for(int i = 0; i < 600; i++)
			{
				if(this.ProcessingThread.IsAlive)
				{
					Thread.Sleep(100);
				}
				else
				{
					break;
				}
			}
			if(this.ProcessingThread.IsAlive)
			{
				this.ProcessingThread.Abort();
				this.ProcessingThread.Join();
			}
			System.Environment.Exit(0);
			ServiceStartupLogger.LogMessage("Done WinService.OnStop");
		}

		public void DebugStart()
		{
			ServiceStartupLogger.LogMessage("Started WinService.DebugStart");
			this._stopRequested = false;
			this.ThreadProc();
			ServiceStartupLogger.LogMessage("Done WinService.DebugStart");
		}

		private void ThreadProc()
		{
			ServiceStartupLogger.LogMessage("Started WinService.ThreadProc");
			this.JobScheduler.StartJobs();
			while(!this._stopRequested)
			{
				Thread.Sleep(1000);
			}
			this.JobScheduler.StopJobs();
			ServiceStartupLogger.LogMessage("Done WinService.ThreadProc");
		}
	}
}
