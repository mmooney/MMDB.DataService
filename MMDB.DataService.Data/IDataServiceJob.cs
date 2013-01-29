using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Jobs;
using Quartz;

namespace MMDB.DataService.Data
{
	public interface IDataServiceJob
	{
		void Run();
	}
}
