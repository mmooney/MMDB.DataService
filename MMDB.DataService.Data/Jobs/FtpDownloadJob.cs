using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using MMDB.DataService.Data.Dto.Ftp;
using MMDB.DataService.Data.Settings;
using Raven.Client;

namespace MMDB.DataService.Data.Jobs
{
	public class FtpDownloadJob : ListImportJobBase<FtpDownloadJobConfiguration, FtpDownloadMetadata, FtpInboundData>
	{
		private FtpManager FtpManager { get; set; }
		
		public FtpDownloadJob(IDocumentSession documentSession, EventReporter eventReporter, FtpManager ftpManager) : base(eventReporter, documentSession)
		{
			this.FtpManager = ftpManager;
		}

		protected override FtpInboundData TryCreateJobData(FtpDownloadJobConfiguration configuration, FtpDownloadMetadata item, out bool jobAlreadyExisted)
		{
			FtpInboundData returnValue;
			//using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
			//{
				//returnValue = this.DocumentSession.Query<FtpInboundData>()
				//									.Customize(i => i.WaitForNonStaleResultsAsOfNow())
				//									.SingleOrDefault(i => i.Directory == item.Directory && i.FileName == item.FileName);
				//if (returnValue == null)
				//{
					returnValue = new FtpInboundData
					{
						Directory = item.Directory,
						FileName = item.FileName,
						QueuedDateTimeUtc = DateTime.UtcNow,
						AttachmentId = this.FtpManager.DownloadFile(item)
					};
					jobAlreadyExisted = false;

					this.DocumentSession.Store(returnValue);
					this.DocumentSession.SaveChanges();
					//transaction.Complete();
				//}
				//else
				//{
				//	jobAlreadyExisted = true;
				//}
			//}
			return returnValue;
		}

		protected override List<FtpDownloadMetadata> GetListToProcess(FtpDownloadJobConfiguration configuration)
		{
			if(configuration.FtpDownloadSettings == null || configuration.FtpDownloadSettings.Count == 0)
			{
				throw new Exception("FtpDownloadServiceSettings.FtpDownloadSettings is empty");
			}
			List<FtpDownloadMetadata> returnList = new List<FtpDownloadMetadata>();
			foreach (var item in configuration.FtpDownloadSettings)
			{
				var tempList = this.FtpManager.GetAvailableDownloadList(item);
				returnList.AddRange(tempList);
			}
			return returnList;
		}
	}
}
