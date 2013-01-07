using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto;
using MMDB.DataService.Data.Dto.Jobs;
using Raven.Client;

namespace MMDB.DataService.Data.Jobs
{
	public abstract class ItemProcessingJob<T> : IQueueJob<T> where T : JobData
	{
		protected IDocumentSession DocumentSession { get; private set; }
		protected EventReporter EventReporter { get; private set; }


		public ItemProcessingJob(IDocumentSession documentSession, EventReporter eventReporter)
		{
			this.DocumentSession = documentSession;
			this.EventReporter = eventReporter;
		}

		public Type GetQueueDataType()
		{
			return typeof(T);
		}

		public void Run()
		{
			bool done = false;
			while (!done)
			{
				T jobData = this.GetNextItemToProcess();
				if (jobData == null)
				{
					done = true;
				}
				else
				{
					try
					{
						this.ProcessItem(jobData);
						this.MarkItemSuccessful(jobData);
					}
					catch (Exception err)
					{
						this.MarkItemFailed(jobData, err);
					}
				}
			}
		}

		protected abstract T GetNextItemToProcess();
		protected abstract void ProcessItem(T jobItem);

		protected virtual void MarkItemSuccessful(T jobData)
		{
			jobData.Status = EnumJobStatus.Complete;
			this.DocumentSession.SaveChanges();
		}

		protected virtual void MarkItemFailed(T jobData, Exception err)
		{
			var errorObject = this.EventReporter.ExceptionForObject(err, jobData);
			jobData.Status = EnumJobStatus.Error;
			jobData.ExceptionIdList.Add(errorObject.Id);
		}

	}
}
