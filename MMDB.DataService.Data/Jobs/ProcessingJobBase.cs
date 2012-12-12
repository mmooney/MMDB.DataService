using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Jobs;

namespace MMDB.DataService.Data.Jobs
{
	public abstract class ProcessingJob<T>: IDataServiceJob where T:JobData
	{
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
		protected abstract void MarkItemSuccessful(T jobItem);
		protected abstract void MarkItemFailed(T jobData, Exception err);
	}
}
