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
		protected IEventReporter EventReporter { get; private set; }

		public ItemProcessingJob(IEventReporter eventReporter) : base()
		{
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
			int counter = 0;
			int maxItemsToProcess = this.GetMaxItemsToProcess(configuration);
			while (!done)
			{
				if(counter > maxItemsToProcess)
				{
					done = true;
				}
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
                        this.EventReporter.ExceptionForObject(err, jobData);
					}
				}
			}
		}

		protected virtual int GetMaxItemsToProcess(ConfigType configuration)
		{
			return int.MaxValue;
		}

		protected abstract JobDataType GetNextItemToProcess(ConfigType configuration);
		protected abstract void ProcessItem(ConfigType configuration, JobDataType jobItem);

		protected abstract void MarkItemSuccessful(JobDataType jobData);
		//protected virtual void MarkItemSuccessful(JobDataType jobData);
		//{
		//	var item = (JobDataType)this.DocumentSession.Load(jobData.GetType(), jobData.Id);
		//	item.Status = EnumJobStatus.Complete;
		//	this.DocumentSession.SaveChanges();
		//}

		protected abstract void MarkItemFailed(JobDataType jobData, Exception err);
		//protected virtual void MarkItemFailed(JobDataType jobData, Exception err)
		//{
		//	var item = (JobDataType)this.DocumentSession.Load(jobData.GetType(), jobData.Id);
		//	item.Status = EnumJobStatus.Error;

		//	var errorObject = this.EventReporter.ExceptionForObject(err, jobData);
		//	item.Status = EnumJobStatus.Error;
		//	item.ExceptionIdList.Add(errorObject.Id);
		//	this.DocumentSession.SaveChanges();
		//}
	}
}
