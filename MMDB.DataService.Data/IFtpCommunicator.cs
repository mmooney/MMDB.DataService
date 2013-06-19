using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Ftp;

namespace MMDB.DataService.Data
{
	public interface IFtpCommunicator
	{
		void UploadFile(EnumSettingSource settingSource, string settingKey, string targetFileName, string targetDirectory, byte[] fileData);
		List<string> GetFileList(EnumSettingSource settingSource, string settingKey, string directory, string filePattern);
		void DownloadFile(EnumSettingSource settingSource, string settingKey, string ftpSourcePath, string localDestinationPath);
	}
}
