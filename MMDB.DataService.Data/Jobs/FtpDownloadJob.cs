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
		private IFtpJobManager FtpJobManager { get; set; }

		public FtpDownloadJob(IEventReporter eventReporter, IFtpJobManager ftpJobManager) : base(eventReporter)
		{
			this.FtpJobManager = ftpJobManager;
		}

		protected override FtpInboundData TryCreateJobData(FtpDownloadJobConfiguration configuration, FtpDownloadMetadata item, out bool jobAlreadyExisted)
		{
			return this.FtpJobManager.TryCreateJobData(item, out jobAlreadyExisted);
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
				var tempList = this.FtpJobManager.GetAvailableDownloadList(item);
				returnList.AddRange(tempList);
			}
			return returnList;
		}
	}
}
