using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Jobs
{
	public class FtpUploadJob : IDataServiceJob
	{
		private FtpManager FtpManager { get; set; }
		
		public FtpUploadJob(FtpManager ftpManager)
		{
			this.FtpManager = ftpManager;
		}

		public void Run(Quartz.IJobExecutionContext context)
		{
			bool done = false;
			while(!done)
			{
				var jobItem = this.FtpManager.GetNextUploadItem();
				if(jobItem == null)
				{
					done = true;
				}
				else 
				{
					this.FtpManager.UploadFile(jobItem);
				}
			}
		}
	}
}
