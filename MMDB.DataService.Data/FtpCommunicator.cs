﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Settings;

namespace MMDB.DataService.Data
{
	public class FtpCommunicator : IFtpCommunicator
	{
		private IEventReporter EventReporter { get; set; }
		private ConnectionSettingsManager ConnectionSettingsManager { get; set; }

		public FtpCommunicator(ConnectionSettingsManager connectionSettingsManager, IEventReporter eventReporter)
		{
			this.ConnectionSettingsManager = connectionSettingsManager;
			this.EventReporter = eventReporter;
		}

		public void UploadFile(EnumSettingSource settingSource, string settingKey, string targetFileName, string targetDirectory, byte[] fileData)
		{
			var settings = this.ConnectionSettingsManager.Load<FtpConnectionSettings>(settingSource, settingKey);
			Tamir.SharpSsh.Sftp ftp = new Tamir.SharpSsh.Sftp(settings.FtpHost, settings.FtpUserName, settings.FtpPassword);
			ftp.Connect();

			string targetFilePath;
			targetFileName = targetFileName.Replace("/", "_");
			if (!string.IsNullOrEmpty(targetDirectory))
			{
				targetFilePath = targetDirectory + targetFileName;
			}
			else
			{
				targetFilePath = targetFileName;
			}

			string tempDirectory = Path.Combine(Path.GetTempPath(), "MMDB.DataService");
			if (!Directory.Exists(tempDirectory))
			{
				Directory.CreateDirectory(tempDirectory);
			}
			string tempPath = Path.Combine(tempDirectory, Guid.NewGuid().ToString() + "." + targetFileName);
			try
			{
				this.EventReporter.Info("Uploading file " + tempPath + " to " + targetFilePath + "...");
				File.WriteAllBytes(tempPath, fileData);
				ftp.Put(tempPath, targetFilePath);
				this.EventReporter.Info("Uploading file " + tempPath + " to " + targetFilePath + " complete");
			}
			finally
			{
				try
				{
					if (File.Exists(tempPath))
					{
						File.Delete(tempPath);
						tempPath = null;
					}
				}
				catch { }
			}
		}
	}
}
