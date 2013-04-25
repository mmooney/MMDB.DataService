using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Ftp;
using MMDB.Shared;
using MMDB.DataService.Data.Dto;
using System.Transactions;
using Raven.Client;
using System.IO;
using MMDB.DataService.Data.Jobs;
using MMDB.DataService.Data.Settings;
using System.Net;
using FtpLib;
using System.Collections;

namespace MMDB.DataService.Data
{
	public class FtpManager
	{
		private IDocumentSession DocumentSession { get; set; }
		private ConnectionSettingsManager SettingsManager { get; set; }
		private IEventReporter EventReporter { get; set; }
		private IDocumentStore DocumentStore;

		public FtpManager(IDocumentSession documentSession, ConnectionSettingsManager settingsManager, IEventReporter eventReporter, IDocumentStore documentStore)
		{
			this.DocumentSession = documentSession;
			this.SettingsManager = settingsManager;
			this.EventReporter = eventReporter;
			this.DocumentStore = documentStore;
		}

		public FtpOutboundData GetNextUploadItem()
		{
			FtpOutboundData returnValue;
			using(var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel=IsolationLevel.Serializable }))
			{
				returnValue = this.DocumentSession.Query<FtpOutboundData>()
													.Customize(i => i.WaitForNonStaleResultsAsOfNow(TimeSpan.FromSeconds(120)))
													.OrderByDescending(i => i.QueuedDateTimeUtc)
													.FirstOrDefault(i => i.Status == EnumJobStatus.New);
				if(returnValue != null)
				{
					returnValue = this.DocumentSession.Load<FtpOutboundData>(returnValue.Id);
					if (returnValue.Status != EnumJobStatus.New)
					{
						this.EventReporter.WarningForObject("FtpOutboundData was returned as New from query but was really " + returnValue.Status.ToString(), returnValue);
						return null;
					}
					else 
					{
						returnValue.Status = EnumJobStatus.InProcess;
						this.DocumentSession.SaveChanges();
						transaction.Complete();
					}
				}
			}
			return returnValue;
		}

		public FtpOutboundData QueueUploadItem(string fileData, string targetDirectory, string targetFileName, string ftpSettingsKey, EnumSettingSource ftpSettingsSource)
		{
			string attachmentID = Guid.NewGuid().ToString();

			using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
			{
				bool anyExistingItems = this.DocumentSession.Query<FtpOutboundData>()
											.Customize(i => i.WaitForNonStaleResultsAsOfNow(TimeSpan.FromSeconds(120)))
											.Where(i=>i.TargetFileName == targetFileName)
											.Any();
				if (anyExistingItems)
				{
					throw new Exception(string.Format("FtpOutputData record for file name {0} already exists", targetFileName));
				}
				using (var stream = new MemoryStream())
				{
					using(var writer = new StreamWriter(stream))
					{
						writer.Write(fileData);
						writer.Flush();
						stream.Position = 0;
						this.DocumentStore.DatabaseCommands.PutAttachment(attachmentID, null, stream, new Raven.Json.Linq.RavenJObject());
					}
				}
				var newItem = new FtpOutboundData
				{
					AttachmentId = attachmentID,
					QueuedDateTimeUtc = DateTime.UtcNow,
					SettingKey = ftpSettingsKey,
					SettingSource = ftpSettingsSource,
					Status = EnumJobStatus.New,
					TargetDirectory = targetDirectory,
					TargetFileName = targetFileName
				};
				this.DocumentSession.Store(newItem);
				this.DocumentSession.SaveChanges();
				transaction.Complete();

				return newItem;
			}
		}

		public List<FtpDownloadMetadata> GetAvailableDownloadList(FtpDownloadSettings ftpDownloadSettings)
		{
			var settings = this.SettingsManager.Load<FtpConnectionSettings>(ftpDownloadSettings.SettingSource, ftpDownloadSettings.SettingKey);

			var returnList = new List<FtpDownloadMetadata>();
			if (settings.SecureFTP || true)
			{
				var patternList = new List<string>();
				if (ftpDownloadSettings.FilePatternList == null || ftpDownloadSettings.FilePatternList.Count == 0)
				{
					if (string.IsNullOrEmpty(ftpDownloadSettings.DownloadDirectory))
					{
						patternList.Add("*.*");
					}
					else if (ftpDownloadSettings.DownloadDirectory.EndsWith("/"))
					{
						patternList.Add(ftpDownloadSettings.DownloadDirectory + "*.*");
					}
					else
					{
						patternList.Add(ftpDownloadSettings.DownloadDirectory + "/*.*");
					}
				}
				foreach (var pattern in ftpDownloadSettings.FilePatternList)
				{
					if (string.IsNullOrEmpty(ftpDownloadSettings.DownloadDirectory))
					{
						patternList.Add("*." + pattern);
					}
					else if (ftpDownloadSettings.DownloadDirectory.EndsWith("/"))
					{
						patternList.Add(ftpDownloadSettings.DownloadDirectory + pattern);
					}
					else
					{
						patternList.Add(ftpDownloadSettings.DownloadDirectory + "/" + pattern);
					}
				}

				Tamir.SharpSsh.Sftp ftp = new Tamir.SharpSsh.Sftp(settings.FtpHost, settings.FtpUserName, settings.FtpPassword);
				ftp.Connect();
				foreach (var path in patternList)
				{
					this.EventReporter.Trace("Searching on " + settings.FtpHost + " for: " + path);
					var list = ftp.GetFileList(path);
					this.EventReporter.Trace((list??new ArrayList()).Count.ToString() + "{0} records found");
					foreach (string fileName in list)
					{
						var newItem = new FtpDownloadMetadata
						{
							Directory = ftpDownloadSettings.DownloadDirectory,
							FileName = fileName,
							Settings = ftpDownloadSettings
						};
						returnList.Add(newItem);
					}
				}
			}
			else 
			{
				using(var client = new FtpConnection(settings.FtpHost, settings.FtpUserName, settings.FtpPassword))
				{
					client.Open();
					client.Login();
					FtpDirectoryInfo directoryInfo;
					if(string.IsNullOrEmpty(ftpDownloadSettings.DownloadDirectory) || client.GetCurrentDirectory() == ftpDownloadSettings.DownloadDirectory)
					{
						directoryInfo = client.GetCurrentDirectoryInfo();
					}
					else
					{
						var list = client.GetDirectories(ftpDownloadSettings.DownloadDirectory);
						if(list.Length > 0)
						{
							directoryInfo = list[0];
						}
						else 
						{
							directoryInfo = null;
						}
					}
					if(directoryInfo != null)
					{
						foreach(var pattern in ftpDownloadSettings.FilePatternList)
						{
							var fileList = client.GetFiles(pattern);
							if(fileList != null)
							{
								foreach(var file in fileList)
								{
									var newItem = new FtpDownloadMetadata
									{
										Directory = ftpDownloadSettings.DownloadDirectory,
										FileName = file.Name,
										Settings = ftpDownloadSettings
									};
									returnList.Add(newItem);
								}
							}
						}
					}
				}
			}

			return returnList;
		}

		public string DownloadFile(FtpDownloadMetadata item)
		{
			var settings = this.SettingsManager.Load<FtpConnectionSettings>(item.Settings.SettingSource, item.Settings.SettingKey);
			if (settings.SecureFTP || true)
			{
				Tamir.SharpSsh.Sftp ftp = new Tamir.SharpSsh.Sftp(settings.FtpHost, settings.FtpUserName, settings.FtpPassword);
				ftp.Connect();

				string ftpFilePath;
				if (!string.IsNullOrEmpty(item.Directory))
				{
					if(item.Directory.EndsWith("/"))
					{
						ftpFilePath = item.Directory + item.FileName;
					}
					else 
					{
						ftpFilePath = item.Directory + "/" + item.FileName;
					}
				}
				else
				{
					ftpFilePath = item.FileName;
				}

				string tempDirectory = Path.Combine(Path.GetTempPath(), "MMDB.DataService");
				if (!Directory.Exists(tempDirectory))
				{
					Directory.CreateDirectory(tempDirectory);
				}
				string tempPath = Path.Combine(tempDirectory, Guid.NewGuid().ToString() + "." + item.FileName);
				try
				{
					ftp.Get(ftpFilePath, tempPath);
					string attachmentID = Guid.NewGuid().ToString();
					using(FileStream stream = new FileStream(tempPath, FileMode.Open))
					{
						this.DocumentStore.DatabaseCommands.PutAttachment(attachmentID, null, stream, new Raven.Json.Linq.RavenJObject());
					}
					return attachmentID;
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
			else 
			{
				using(var ftp = new FtpConnection(settings.FtpHost, settings.FtpUserName, settings.FtpPassword))
				{
					ftp.Open();
					ftp.Login();
					if (!string.IsNullOrEmpty(item.Directory))
					{
						ftp.SetCurrentDirectory(item.Directory);
					}

					string tempDirectory = Path.Combine(Path.GetTempPath(), "MMDB.DataService");
					if (!Directory.Exists(tempDirectory))
					{
						Directory.CreateDirectory(tempDirectory);
					}
					string tempPath = Path.Combine(tempDirectory, Guid.NewGuid().ToString() + "." + item.FileName);
					try
					{
						ftp.GetFile(item.FileName, tempPath, false);
						string attachmentID = Guid.NewGuid().ToString();
						using (FileStream stream = new FileStream(tempPath, FileMode.Open))
						{
							this.DocumentStore.DatabaseCommands.PutAttachment(attachmentID, null, stream, new Raven.Json.Linq.RavenJObject());
						}
						return attachmentID;
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

		public void UploadFile(FtpOutboundData jobItem)
		{
			var settings = this.SettingsManager.Load<FtpConnectionSettings>(jobItem.SettingSource, jobItem.SettingKey);
			Tamir.SharpSsh.Sftp ftp = new Tamir.SharpSsh.Sftp(settings.FtpHost, settings.FtpUserName, settings.FtpPassword);
			ftp.Connect();

			string targetFilePath;
			string targetFileName = jobItem.TargetFileName.Replace("/","_");
			if(!string.IsNullOrEmpty(jobItem.TargetDirectory))
			{
				targetFilePath = jobItem.TargetDirectory + targetFileName;
			}
			else 
			{
				targetFilePath = targetFileName;
			}

			string tempDirectory = Path.Combine(Path.GetTempPath(), "MMDB.DataService");
			if(!Directory.Exists(tempDirectory))
			{
				Directory.CreateDirectory(tempDirectory);
			}
			string tempPath = Path.Combine(tempDirectory, Guid.NewGuid().ToString() + "." + targetFileName);
			try 
			{
				this.EventReporter.Info("Uploading file " + tempPath + " to " + targetFilePath + "...");
				var attachment = this.DocumentStore.DatabaseCommands.GetAttachment(jobItem.AttachmentId);
				using (var fileStream = File.Create(tempPath))
				{
					attachment.Data().CopyTo(fileStream);
				}
				ftp.Put(tempPath, targetFilePath);
				this.EventReporter.Info("Uploading file " + tempPath + " to " + targetFilePath + " complete");
			}
			finally
			{
				try 
				{
					if(File.Exists(tempPath))
					{
						File.Delete(tempPath);
						tempPath = null;
					}
				}
				catch {}
			}
		}

		public void MarkItemSuccessful(FtpOutboundData jobData)
		{
			jobData.Status = EnumJobStatus.Complete;
			this.DocumentSession.SaveChanges();
		}

		public void MarkItemFailed(FtpOutboundData jobData, Exception err)
		{
			var errorObject = this.EventReporter.ExceptionForObject(err, jobData);
			jobData.Status = EnumJobStatus.Error;
			jobData.ExceptionIdList.Add(errorObject.Id);
			this.DocumentSession.SaveChanges();	
		}

		public FtpInboundData TryCreateJobData(FtpDownloadMetadata item, out bool jobAlreadyExisted)
		{
			FtpInboundData returnValue;
			//using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
			//{
			//	returnValue = this.DocumentSession.Query<FtpInboundData>()
			//										.Customize(i => i.WaitForNonStaleResultsAsOfNow())
			//										.SingleOrDefault(i => i.Directory == item.Directory && i.FileName == item.FileName);
			//	if (returnValue == null)
			//	{
					returnValue = new FtpInboundData
					{
						Directory = item.Directory,
						FileName = item.FileName,
						QueuedDateTimeUtc = DateTime.UtcNow,
						AttachmentId = this.DownloadFile(item)
					};
					jobAlreadyExisted = false;

					this.DocumentSession.Store(returnValue);
					this.DocumentSession.SaveChanges();
					//transaction.Complete();
			//	}
			//	else
			//	{
			//		jobAlreadyExisted = true;
			//	}
			//}
			return returnValue;
		}
	}
}
