using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Ftp;
using Raven.Client;

namespace MMDB.DataService.Data.Jobs
{
	public class FtpUploadJob : ItemProcessingJob<NullJobConfiguration, FtpOutboundData>
	{
		private IFtpJobManager FtpJobManager { get; set; }
		private List<string> ProcessedFileNames { get; set; }
		
		public FtpUploadJob(IFtpJobManager ftpJobManager, IEventReporter eventReporter) : base(eventReporter)
		{
			this.FtpJobManager = ftpJobManager;
			this.ProcessedFileNames = new List<string>();
		}

		protected override int GetMaxItemsToProcess(NullJobConfiguration configuration)
		{
			return 10;
		}

		protected override FtpOutboundData GetNextItemToProcess(NullJobConfiguration configuration)
		{
			var item = this.FtpJobManager.GetNextUploadItem();
			if(item != null)
			{
				if(this.ProcessedFileNames.Contains(item.TargetFileName, StringComparer.CurrentCultureIgnoreCase))
				{
					var err = new Exception("Already processed " + item.TargetFileName);
					this.FtpJobManager.MarkItemFailed(item, err);
					throw err;
				}
				else 
				{
					this.ProcessedFileNames.Add(item.TargetFileName);
				}
			}
			return item;
		}

		protected override void ProcessItem(NullJobConfiguration configuration, FtpOutboundData jobItem)
		{
			this.FtpJobManager.UploadFile(jobItem);
		}

		protected override void MarkItemSuccessful(FtpOutboundData jobData)
		{
			this.FtpJobManager.MarkItemSuccessful(jobData);
		}

		protected override void MarkItemFailed(FtpOutboundData jobData, Exception err)
		{
			this.FtpJobManager.MarkItemFailed(jobData, err);
		}
	}
}
