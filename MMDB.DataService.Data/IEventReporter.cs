using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Logging;

namespace MMDB.DataService.Data
{
	public interface IEventReporter
	{
		ServiceMessage Trace(string message);
		ServiceMessage Info(string message);
		ServiceMessage InfoForObject(string message, object dataObject);
		ServiceMessage WarningForObject(string message, object dataObject);
		ServiceMessage Exception(Exception err);
		ServiceMessage ExceptionForObject(Exception err, object dataObject);
		ServiceMessage ExceptionForObject(string message, object dataObject);
	}
}
