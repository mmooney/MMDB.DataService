using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Jobs;

namespace MMDB.DataService.Data.Jobs
{
	public abstract class ListImportJobBase<ImportDataType, JobDataType>:  IDataServiceJob where JobDataType : JobData
	{
		protected EventReporter EventReporter { get; private set; }

		public ListImportJobBase(EventReporter eventReporter)
		{
			this.EventReporter = eventReporter;
		}

		public void Run()
		{
			var list = this.GetListToProcess();
			foreach (var item in list)
			{
				try
				{
					bool jobAlreadyExisted;
					var jobData = this.TryCreateJobData(item, out jobAlreadyExisted);
					if (jobData == null)
					{
						this.EventReporter.ExceptionForObject("TryCreateJobData returned null", item);
					}
					if (jobAlreadyExisted)
					{
						this.EventReporter.WarningForObject("Order job already existed", item);
					}
					else
					{
						this.EventReporter.InfoForObject("Order job created", item);
					}
				}
				catch (Exception err)
				{
					this.EventReporter.ExceptionForObject(err, item);
				}
			}
		}

		protected abstract JobDataType TryCreateJobData(ImportDataType item, out bool jobAlreadyExisted);
		protected abstract List<ImportDataType> GetListToProcess();
	}
}
