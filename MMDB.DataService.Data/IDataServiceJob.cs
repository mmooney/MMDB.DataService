using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;

namespace MMDB.DataService.Data
{
	public interface IDataServiceJob
	{
		void Run();
	}
}
