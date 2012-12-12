using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Ftp;

namespace MMDB.DataService.Data.Jobs
{
	public class FtpUploadJob : ProcessingJob<FtpOutboundData>
	{
		private FtpManager FtpManager { get; set; }
		
		public FtpUploadJob(FtpManager ftpManager)
		{
			this.FtpManager = ftpManager;
		}

		protected override FtpOutboundData GetNextItemToProcess()
		{
			return this.FtpManager.GetNextUploadItem();
		}

		protected override void ProcessItem(FtpOutboundData jobItem)
		{
			this.FtpManager.UploadFile(jobItem);
		}

		protected override void MarkItemSuccessful(FtpOutboundData jobItem)
		{
			this.FtpManager.MarkJobSuccessful(jobItem);
		}

		protected override void MarkItemFailed(FtpOutboundData jobData, Exception err)
		{
			this.FtpManager.MarkJobFailed(jobData, err);
		}
	}
}
