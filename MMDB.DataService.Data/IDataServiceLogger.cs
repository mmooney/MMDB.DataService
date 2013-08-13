using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Logging;

namespace MMDB.DataService.Data
{
	public interface IDataServiceLogger
	{
		ServiceMessage Trace(string message);
		ServiceMessage TraceForObject(string message, object dataObject);

		ServiceMessage InfoForObject(string message, object dataObject);
		ServiceMessage Info(string message);

		ServiceMessage WarningForObject(string message, object dataObject);

		ServiceMessage Exception(Exception err, object dataObject = null);

		int GetEventCount(EnumServiceMessageLevel? level);
		IQueryable<ServiceMessage> GetEventList(int pageIndex, int pageSize, EnumServiceMessageLevel? level);
		ServiceMessage GetEventItem(int id);
	}
}
