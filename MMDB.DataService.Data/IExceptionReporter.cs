using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Logging;

namespace MMDB.DataService.Data
{
	public interface IExceptionReporter
	{
		void Exception(ServiceMessage exception);
	}
}
