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
		private FtpManager FtpManager { get; set; }
		private List<string> ProcessedFileNames { get; set; }
		
		public FtpUploadJob(FtpManager ftpManager, IDocumentSession documentSession, IEventReporter eventReporter) : base(documentSession, eventReporter)
		{
			this.FtpManager = ftpManager;
			this.ProcessedFileNames = new List<string>();
		}

		protected override int GetMaxItemsToProcess(NullJobConfiguration configuration)
		{
			return 10;
		}

		protected override FtpOutboundData GetNextItemToProcess(NullJobConfiguration configuration)
		{
			var item = this.FtpManager.GetNextUploadItem();
			if(item != null)
			{
				if(this.ProcessedFileNames.Contains(item.TargetFileName, StringComparer.CurrentCultureIgnoreCase))
				{
					var err = new Exception("Already processed " + item.TargetFileName);
					this.FtpManager.MarkItemFailed(item, err);
					throw err;
				}
				else 
				{
					this.ProcessedFileNames.Add(item.TargetFileName);
				}
			}
		}

		protected override void ProcessItem(NullJobConfiguration configuration, FtpOutboundData jobItem)
		{
			this.FtpManager.UploadFile(jobItem);
		}

		protected override void MarkItemSuccessful(FtpOutboundData jobData)
		{
			this.FtpManager.MarkItemSuccessful(jobData);
		}

		protected override void MarkItemFailed(FtpOutboundData jobData, Exception err)
		{
			this.FtpManager.MarkItemFailed(jobData, err);
		}
	}
}
