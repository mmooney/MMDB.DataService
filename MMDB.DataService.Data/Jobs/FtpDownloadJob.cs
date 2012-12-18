using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using MMDB.DataService.Data.Dto.Ftp;
using Raven.Client;

namespace MMDB.DataService.Data.Jobs
{
	public class FtpDownloadJob : ListImportJobBase<FtpDownloadMetadata, FtpInboundData>
	{
		private IDocumentSession DocumentSession { get; set; }
		private FtpManager FtpManager { get; set; }
		private FtpDownloadSettings FtpDownloadSettings { get; set; }

		public FtpDownloadJob(IDocumentSession documentSession, EventReporter eventReporter, FtpManager ftpManager, FtpDownloadSettings ftpDownloadSettings) : base(eventReporter)
		{
			this.DocumentSession = documentSession;
			this.FtpManager = ftpManager;
			this.FtpDownloadSettings = ftpDownloadSettings;
		}

		protected override FtpInboundData TryCreateJobData(FtpDownloadMetadata item, out bool jobAlreadyExisted)
		{
			FtpInboundData returnValue;
			using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
			{
				returnValue = this.DocumentSession.Query<FtpInboundData>()
													.Customize(i => i.WaitForNonStaleResultsAsOfNow())
													.SingleOrDefault(i => i.Directory == item.Directory && i.FileName == item.FileName);
				if (returnValue == null)
				{
					returnValue = new FtpInboundData
					{
						Directory = item.Directory,
						FileName = item.FileName,
						QueuedDateTimeUtc = DateTime.UtcNow,
						AttachmentId = this.FtpManager.DownloadFile(item, this.FtpDownloadSettings)
					};
					jobAlreadyExisted = false;

					this.DocumentSession.Store(returnValue);
					this.DocumentSession.SaveChanges();
					transaction.Complete();
				}
				else
				{
					jobAlreadyExisted = true;
				}
			}
			return returnValue;
		}

		protected override List<FtpDownloadMetadata> GetListToProcess()
		{
			return this.FtpManager.GetAvailableDownloadList(this.FtpDownloadSettings);
		}
	}
}
