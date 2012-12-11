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

		public FtpManager(IDocumentSession documentSession, SettingsManager settingsManager)
		{
			this.DocumentSession = documentSession;
			this.SettingsManager = settingsManager;
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

		public void UploadFile(FtpOutboundData jobItem)
		{
			var settings = this.SettingsManager.Load<FtpSettings>(jobItem.SettingSource, jobItem.SettingKey);
			Tamir.SharpSsh.Sftp ftp = new Tamir.SharpSsh.Sftp(settings.FtpHost, settings.FtpUserName, settings.FtpPassword);
			ftp.Connect();

			string targetFilePath;
			if(!string.IsNullOrEmpty(jobItem.TargetDirectory))
			{
				targetFilePath = jobItem.TargetDirectory + jobItem.TargetFileName;
			}
			else 
			{
				targetFilePath = jobItem.TargetFileName;
			}

			string tempDirectory = Path.Combine(Path.GetTempPath(), "MMDB.DataService");
			if(!Directory.Exists(tempDirectory))
			{
				Directory.CreateDirectory(tempDirectory);
			}
			string tempPath = Path.Combine(tempDirectory, Guid.NewGuid().ToString() + "." + jobItem.TargetFileName);
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
	}
}
