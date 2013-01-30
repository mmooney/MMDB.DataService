using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Jobs;

namespace MMDB.DataService.Data.Jobs
{
	[Obsolete]
	public abstract class ListImportJobBaseOld<ImportDataType, JobDataType> : IQueueJob<JobDataType> where JobDataType : JobData
	{
		protected EventReporter EventReporter { get; private set; }

		public ListImportJobBaseOld(EventReporter eventReporter)
		{
			this.EventReporter = eventReporter;
		}

		public Type GetQueueDataType()
		{
			return typeof(JobDataType);
		}

		public void Run()
		{
			this.EventReporter.Trace(this.GetType().Name + ": job run started");
			var list = this.GetListToProcess();
			this.EventReporter.Trace(this.GetType().Name + ": " + list.Count() + " records to process");
			foreach (var item in list)
			{
				try
				{
					bool jobAlreadyExisted;
					this.EventReporter.InfoForObject(this.GetType().Name + ": processing item", item);
					var jobData = this.TryCreateJobData(item, out jobAlreadyExisted);
					if (jobData == null)
					{
						string message = string.Format("TryCreateJobData<{0}> returned null", typeof(ImportDataType).Name);
						this.EventReporter.ExceptionForObject(message, item);
					}
					if (jobAlreadyExisted)
					{
						this.EventReporter.WarningForObject(typeof(ImportDataType).Name + " job data already existed", item);
					}
					else
					{
						this.EventReporter.InfoForObject(this.GetType().Name + ": item done", jobData);
					}
				}
				catch (Exception err)
				{
					this.EventReporter.ExceptionForObject(err, item);
				}
			}
			this.EventReporter.Trace(this.GetType().Name + ": job run ended");
		}

		protected abstract JobDataType TryCreateJobData(ImportDataType item, out bool jobAlreadyExisted);
		protected abstract List<ImportDataType> GetListToProcess();
	}
}
