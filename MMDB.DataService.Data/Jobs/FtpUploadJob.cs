using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Ftp;
using Raven.Client;

namespace MMDB.DataService.Data.Jobs
{
	public class FtpUploadJob : ItemProcessingJob<FtpOutboundData>
	{
		private FtpManager FtpManager { get; set; }
		
		public FtpUploadJob(FtpManager ftpManager, IDocumentSession documentSession, EventReporter eventReporter) : base(documentSession, eventReporter)
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
	}
}
