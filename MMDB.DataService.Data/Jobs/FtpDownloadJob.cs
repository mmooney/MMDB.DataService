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

		public FtpDownloadJob(IEventReporter eventReporter, FtpManager ftpManager) : base(eventReporter)
		{
			this.FtpManager = ftpManager;
		}

		protected override FtpInboundData TryCreateJobData(FtpDownloadJobConfiguration configuration, FtpDownloadMetadata item, out bool jobAlreadyExisted)
		{
			return this.FtpManager.TryCreateJobData(item, out jobAlreadyExisted);
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
