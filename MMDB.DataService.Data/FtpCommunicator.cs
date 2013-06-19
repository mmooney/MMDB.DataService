using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using MMDB.DataService.Data.Dto.Ftp;
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
			string targetFilePath;
			targetFileName = targetFileName.Replace("/", "_");
			if (!string.IsNullOrEmpty(targetDirectory))
			{
				if(targetDirectory.EndsWith("/"))
				{
					targetFilePath = targetDirectory + targetFileName;
				}
				else 
				{
					targetFilePath = targetDirectory + "/" + targetFileName;
				}
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
				File.WriteAllBytes(tempPath, fileData);

				if(settings.SecureFTP)
				{
					this.EventReporter.Info("Uploading file " + tempPath + " to " + targetFilePath + "...");
					Tamir.SharpSsh.Sftp ftp = new Tamir.SharpSsh.Sftp(settings.FtpHost, settings.FtpUserName, settings.FtpPassword);
					ftp.Connect();
					ftp.Put(tempPath, targetFilePath);
					this.EventReporter.Info("Uploading file " + tempPath + " to " + targetFilePath + " complete");
				}
				else 
				{
					string targetUrl = new Uri(new Uri(Uri.UriSchemeFtp + "://" + settings.FtpHost),targetFilePath).ToString();
					this.EventReporter.Info("Uploading file " + tempPath + " to " + targetUrl + "...");
					FtpWebRequest request = (FtpWebRequest)WebRequest.Create(targetUrl);
					request.Method = WebRequestMethods.Ftp.UploadFile;

					request.Credentials = new NetworkCredential (settings.FtpUserName,settings.FtpPassword);

					using(var requestStream = request.GetRequestStream())
					{
						requestStream.Write(fileData, 0, fileData.Length);
					}
					using(var response = (FtpWebResponse)request.GetResponse())
					{
						this.EventReporter.Info(string.Format("Upload File {0} Complete, status {1}", targetUrl, response.StatusDescription));
						response.Close();
					}
				}

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


		public List<string> GetFileList(EnumSettingSource settingSource, string settingKey, string directory, string filePattern)
		{
			var settings = this.ConnectionSettingsManager.Load<FtpConnectionSettings>(settingSource, settingKey); 
			var returnList = new List<string>();
			if(string.IsNullOrEmpty(filePattern))
			{
				filePattern = "*.*";
			}
			if(settings.SecureFTP)
			{
				string path;
				if (string.IsNullOrEmpty(directory))
				{
					path = filePattern;
				}
				else if (directory.EndsWith("/"))
				{
					path = directory + filePattern;
				}
				else
				{
					path = directory + "/" + filePattern;
				}
				this.EventReporter.Trace("Searching on " + settings.FtpHost + " for: " + path);
				Tamir.SharpSsh.Sftp ftp = new Tamir.SharpSsh.Sftp(settings.FtpHost, settings.FtpUserName, settings.FtpPassword);
				ftp.Connect();
				var list = ftp.GetFileList(path);
				this.EventReporter.Trace((list ?? new ArrayList()).Count.ToString() + " records found for " + path + " on " + settings.FtpHost);
				foreach (string fileName in list)
				{
					returnList.Add(fileName);
				}
			}
			else 
			{
				string path;
				if (string.IsNullOrEmpty(directory))
				{
					path = filePattern;
				}
				else if (directory.EndsWith("/"))
				{
					path = directory + filePattern;
				}
				else
				{
					path = directory + "/" + filePattern;
				}
				string targetUrl = new Uri(new Uri(Uri.UriSchemeFtp + "://" + settings.FtpHost), path).ToString();
				FtpWebRequest request = (FtpWebRequest)WebRequest.Create(targetUrl);
				request.Method = WebRequestMethods.Ftp.ListDirectory;

				request.Credentials = new NetworkCredential(settings.FtpUserName, settings.FtpPassword);

				this.EventReporter.Trace(string.Format("Searching on {0} for: {1}", targetUrl, path));

				string responseData;
				using(var response = (FtpWebResponse)request.GetResponse())
				using(var responseStream = response.GetResponseStream())
				using(var reader = new StreamReader(responseStream))
				{
					responseData = reader.ReadToEnd();
				}
				using(var reader = new StringReader(responseData))
				{
					string item;
					while((item = reader.ReadLine()) != null)
					{
						string filePath;
						if (string.IsNullOrEmpty(directory))
						{
							filePath = item;
						}
						else if (directory.EndsWith("/"))
						{
							filePath = directory + item;
						}
						else
						{
							filePath = directory + "/" + item;
						}
						if (!returnList.Contains(filePath, StringComparer.CurrentCultureIgnoreCase))
						{
							returnList.Add(filePath);
						}
					}
				}

				this.EventReporter.Trace(string.Format("{0} records found for {1} on {2}, found {3}", returnList.Count, path, targetUrl, responseData));

				//http://stackoverflow.com/questions/652037/how-do-i-check-if-a-filename-matches-a-wildcard-pattern
			}
			return returnList;
		}


		public void DownloadFile(EnumSettingSource settingSource, string settingKey, string ftpSourcePath, string localDestinationPath)
		{
			var settings = this.ConnectionSettingsManager.Load<FtpConnectionSettings>(settingSource, settingKey);
			if(settings.SecureFTP)
			{
				Tamir.SharpSsh.Sftp ftp = new Tamir.SharpSsh.Sftp(settings.FtpHost, settings.FtpUserName, settings.FtpPassword);
				ftp.Connect();
				ftp.Get(ftpSourcePath, localDestinationPath);
			}
			else
			{
				string targetUrl = new Uri(new Uri(Uri.UriSchemeFtp + "://" + settings.FtpHost), ftpSourcePath).ToString();
				var request = (FtpWebRequest)WebRequest.Create(targetUrl);
				request.Method = WebRequestMethods.Ftp.DownloadFile;
				request.Credentials = new NetworkCredential(settings.FtpUserName, settings.FtpPassword);

				this.EventReporter.Info(string.Format("Downloading {0} to {1}",  ftpSourcePath, localDestinationPath));

				int totalBytesRead = 0;
				using(var response = (FtpWebResponse)request.GetResponse())
				using(var responseStream = response.GetResponseStream())
				using(var fileStream = new FileStream(localDestinationPath, FileMode.Create))
				{
					var buffer = new byte[1024];
					while (true)
					{
						int bytesRead = responseStream.Read(buffer, 0, buffer.Length);
						totalBytesRead += bytesRead;
						if (bytesRead == 0)
							break;
						fileStream.Write(buffer, 0, bytesRead);
					}

					this.EventReporter.Info(string.Format("Download Complete from {0} to {1}, {2} bytes, status: {3}", ftpSourcePath, localDestinationPath, totalBytesRead, response.StatusDescription));
				}
			}
		}
	}
}
