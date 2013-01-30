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
		
		public FtpUploadJob(FtpManager ftpManager, IDocumentSession documentSession, EventReporter eventReporter) : base(documentSession, eventReporter)
		{
			this.FtpManager = ftpManager;
		}

		protected override FtpOutboundData GetNextItemToProcess(NullJobConfiguration configuration)
		{
			return this.FtpManager.GetNextUploadItem();
		}

		protected override void ProcessItem(NullJobConfiguration configuration, FtpOutboundData jobItem)
		{
			this.FtpManager.UploadFile(jobItem);
		}

	}
}
