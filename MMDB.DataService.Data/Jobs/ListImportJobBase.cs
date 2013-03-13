using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Jobs;
using Raven.Client;

namespace MMDB.DataService.Data.Jobs
{
	public abstract class ListImportJobBase<ConfigType, ImportDataType, JobDataType> : QueueJobBase<ConfigType, JobDataType> where ConfigType : JobConfigurationBase where JobDataType : JobData
	{
		protected IEventReporter EventReporter { get; private set; }

		public ListImportJobBase(IEventReporter eventReporter, IDocumentSession documentSession) : base(documentSession)
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
			var list = this.GetListToProcess(configuration);
			this.EventReporter.Trace(this.GetType().Name + ": " + list.Count() + " records to process");
			foreach (var item in list)
			{
				try
				{
					bool jobAlreadyExisted;
					this.EventReporter.InfoForObject(this.GetType().Name + ": processing item", item);
					var jobData = this.TryCreateJobData(configuration, item, out jobAlreadyExisted);
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

		protected abstract JobDataType TryCreateJobData(ConfigType configuration, ImportDataType item, out bool jobAlreadyExisted);
		protected abstract List<ImportDataType> GetListToProcess(ConfigType configuration);
	}
}
