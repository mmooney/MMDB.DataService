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

namespace MMDB.DataService.Data
{
	public class FtpManager
	{
		private IDocumentSession DocumentSession { get; set; }
		private SettingsManager SettingsManager { get; set; }
		private EventReporter EventReporter { get; set; }

		public FtpManager(IDocumentSession documentSession, SettingsManager settingsManager, EventReporter eventReporter)
		{
			this.DocumentSession = documentSession;
			this.SettingsManager = settingsManager;
			this.EventReporter = eventReporter;
		}

		public FtpOutboundData GetNextUploadItem()
		{
			FtpOutboundData returnValue;
			using(var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel=IsolationLevel.Serializable }))
			{
				returnValue = this.DocumentSession.Query<FtpOutboundData>()
													.Customize(i => i.WaitForNonStaleResultsAsOfNow())
													.OrderByDescending(i => i.QueuedDateTimeUtc)
													.SingleOrDefault(i => i.Status == EnumJobStatus.New);
				if(returnValue != null)
				{
					returnValue.Status = EnumJobStatus.InProcess;
					this.DocumentSession.SaveChanges();
					transaction.Complete();
				}
			}
			return returnValue;
		}

		public FtpOutboundData QueueUploadItem(string fileData, string targetDirectory, string targetFileName, string ftpSettingsKey, EnumSettingSource ftpSettingsSource)
		{
			string attachmentID = Guid.NewGuid().ToString();
			using(var stream = new MemoryStream())
			{
				using(var writer = new StreamWriter(stream))
				{
					writer.Write(fileData);
					writer.Flush();
					stream.Position = 0;
					this.DocumentSession.Advanced.DatabaseCommands.PutAttachment(attachmentID, null, stream, new Raven.Json.Linq.RavenJObject());
				}
			}
			var newItem = new FtpOutboundData
			{
				AttachementId = attachmentID,
				QueuedDateTimeUtc = DateTime.UtcNow,
				SettingKey = ftpSettingsKey,
				SettingSource = ftpSettingsSource,
				Status = EnumJobStatus.New,
				TargetDirectory = targetDirectory,
				TargetFileName = targetFileName
			};
			this.DocumentSession.Store(newItem);
			this.DocumentSession.SaveChanges();
			return newItem;
		}

		public void UploadFile(FtpOutboundData jobItem)
		{
			var settings = this.SettingsManager.Load<FtpSettings>(jobItem.SettingSource, jobItem.SettingKey);
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
				var attachment = this.DocumentSession.Advanced.DatabaseCommands.GetAttachment(jobItem.AttachementId);
				using (var fileStream = File.Create(tempPath))
				{
					attachment.Data().CopyTo(fileStream);
				}
				ftp.Put(tempPath, targetFilePath);
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

		public void MarkJobSuccessful(FtpOutboundData jobItem)
		{
			jobItem.Status = EnumJobStatus.Complete;
			this.DocumentSession.SaveChanges();
		}

		public void MarkJobFailed(FtpOutboundData jobItem, Exception err)
		{
			var errorObject = this.EventReporter.ExceptionForObject(err, jobItem);
			jobItem.Status = EnumJobStatus.Error;
			jobItem.ExceptionIdList.Add(errorObject.Id);
		}
	}
}
