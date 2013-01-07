using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Jobs;

namespace MMDB.DataService.Data.Jobs
{
	public interface IQueueJob<T> : IDataServiceJob where T:JobData
	{
		Type GetQueueDataType();
	}
}
