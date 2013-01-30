using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto;
using MMDB.DataService.Data.Dto.Jobs;
using Raven.Client;

namespace MMDB.DataService.Data.Jobs
{
	public abstract class ItemProcessingJob<ConfigType, JobDataType> : QueueJobBase<ConfigType, JobDataType> where ConfigType:JobConfigurationBase where JobDataType:JobData
	{
		protected EventReporter EventReporter { get; private set; }

		public ItemProcessingJob(IDocumentSession documentSession, EventReporter eventReporter) : base(documentSession)
		{
			this.DocumentSession = documentSession;
			this.EventReporter = eventReporter;
		}

		public Type GetQueueDataType()
		{
			return typeof(JobDataType);
		}

		public override void Run(ConfigType configuration)
		{
			this.EventReporter.Trace(this.GetType().Name + ": job run started");
			bool done = false;
			while (!done)
			{
				JobDataType jobData = this.GetNextItemToProcess(configuration);
				if (jobData == null)
				{
					done = true;
					this.EventReporter.Trace(this.GetType().Name + ": job run done");
				}
				else
				{
					try
					{
						this.EventReporter.InfoForObject(this.GetType().Name + ": processing item", jobData);
						this.ProcessItem(configuration, jobData);
						this.MarkItemSuccessful(jobData);
						this.EventReporter.InfoForObject(this.GetType().Name + ": item done", jobData);
					}
					catch (Exception err)
					{
						this.MarkItemFailed(jobData, err);
					}
				}
			}
		}

		protected abstract JobDataType GetNextItemToProcess(ConfigType configuration);
		protected abstract void ProcessItem(ConfigType configuration, JobDataType jobItem);

		protected virtual void MarkItemSuccessful(JobDataType jobData)
		{
			jobData.Status = EnumJobStatus.Complete;
			this.DocumentSession.SaveChanges();
		}

		protected virtual void MarkItemFailed(JobDataType jobData, Exception err)
		{
			var errorObject = this.EventReporter.ExceptionForObject(err, jobData);
			jobData.Status = EnumJobStatus.Error;
			jobData.ExceptionIdList.Add(errorObject.Id);
			this.DocumentSession.SaveChanges();
		}
	}
}
