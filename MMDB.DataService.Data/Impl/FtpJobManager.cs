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

namespace MMDB.DataService.Data.Impl
{
	public class FtpJobManager : IFtpJobManager
	{
		private IDocumentSession DocumentSession { get; set; }
		private IConnectionSettingsManager SettingsManager { get; set; }
		private IEventReporter EventReporter { get; set; }
		private IFtpCommunicator FtpCommunicator { get; set; }
		private IRavenManager RavenManager { get; set; }

		public FtpJobManager(IDocumentSession documentSession, IConnectionSettingsManager settingsManager, IEventReporter eventReporter, IFtpCommunicator ftpCommunicator, IRavenManager ravenManager)
		{
			this.DocumentSession = documentSession;
			this.SettingsManager = settingsManager;
			this.EventReporter = eventReporter;
			this.FtpCommunicator = ftpCommunicator;
			this.RavenManager = ravenManager;
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
				this.RavenManager.SetAttachment(attachmentID, fileData);
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
			var returnList = new List<FtpDownloadMetadata>();
			List<string> patternList = ftpDownloadSettings.FilePatternList;
			if(patternList == null || patternList.Count == 0)
			{
				patternList.Add("*.*");
			}
			foreach(string pattern in patternList)
			{
				var list = this.FtpCommunicator.GetFileList(ftpDownloadSettings.SettingSource, ftpDownloadSettings.SettingKey, ftpDownloadSettings.DownloadDirectory, pattern);
				if(list != null && list.Count > 0)
				{
					foreach(string fileName in list)
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
			return returnList;
		}

		public string DownloadFile(FtpDownloadMetadata item)
		{
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
				this.FtpCommunicator.DownloadFile(item.Settings.SettingSource, item.Settings.SettingKey, ftpFilePath, tempPath);
				string attachmentID = Guid.NewGuid().ToString();
				using(FileStream stream = new FileStream(tempPath, FileMode.Open))
				{
					this.RavenManager.SetAttachment(attachmentID, stream);
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

		public void UploadFile(FtpOutboundData jobItem)
		{
			var settings = this.SettingsManager.Load<FtpConnectionSettings>(jobItem.SettingSource, jobItem.SettingKey);
			var attachmentData = this.RavenManager.GetAttachment(jobItem.AttachmentId);
			this.FtpCommunicator.UploadFile(jobItem.SettingSource, jobItem.SettingKey, jobItem.TargetFileName, jobItem.TargetDirectory, attachmentData);
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
			if (item.Settings.IgnoreDuplicates)
			{
				using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
				{
					returnValue = this.DocumentSession.Query<FtpInboundData>()
														.Customize(i => i.WaitForNonStaleResultsAsOfLastWrite(TimeSpan.FromSeconds(120)))
														.SingleOrDefault(i => i.Directory == item.Directory && i.FileName == item.FileName);
					if (returnValue == null)
					{
						returnValue = new FtpInboundData
						{
							Directory = item.Directory,
							FileName = item.FileName,
							QueuedDateTimeUtc = DateTime.UtcNow,
							InboundQueueIdentifier = item.Settings.InboundQueueIdentifier,
							AttachmentId = this.DownloadFile(item)
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
			}
			else 
			{
				returnValue = new FtpInboundData
				{
					Directory = item.Directory,
					FileName = item.FileName,
					QueuedDateTimeUtc = DateTime.UtcNow,
					InboundQueueIdentifier = item.Settings.InboundQueueIdentifier,
					AttachmentId = this.DownloadFile(item)
				};
				jobAlreadyExisted = false;

				this.DocumentSession.Store(returnValue);
				this.DocumentSession.SaveChanges();
			}
			return returnValue;
		}
	}
}
