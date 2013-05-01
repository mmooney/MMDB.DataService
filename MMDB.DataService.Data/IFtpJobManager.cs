using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Ftp;
using MMDB.DataService.Data.Jobs;

namespace MMDB.DataService.Data
{
	public interface IFtpJobManager
	{
		FtpInboundData TryCreateJobData(FtpDownloadMetadata item, out bool jobAlreadyExisted);
		FtpOutboundData GetNextUploadItem();
		void UploadFile(FtpOutboundData jobItem);
		void MarkItemSuccessful(FtpOutboundData jobData);
		void MarkItemFailed(FtpOutboundData jobData, Exception err);
		FtpOutboundData QueueUploadItem(string fileData, string targetDirectory, string targetFileName, string ftpSettingsKey, EnumSettingSource ftpSettingsSource);
		List<FtpDownloadMetadata> GetAvailableDownloadList(FtpDownloadSettings ftpDownloadSettings);
	}
}
