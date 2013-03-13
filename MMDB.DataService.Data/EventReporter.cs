using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Logging;

namespace MMDB.DataService.Data
{
	public class EventReporter : IEventReporter
	{
		private DataServiceLogger Logger { get; set; }
		private ExceptionReporter ExceptionReporter { get; set; }

		public EventReporter(DataServiceLogger logger, ExceptionReporter exceptionReporter)
		{
			this.Logger = logger;
			this.ExceptionReporter = exceptionReporter;
		}

		public ServiceMessage Trace(string message)
		{
			return this.Logger.Trace(message);
		}

		public ServiceMessage Exception(Exception err)
		{
			var returnValue = this.Logger.Exception(err);
			this.ExceptionReporter.Exception(returnValue);
			return returnValue;
		}

		public ServiceMessage ExceptionForObject(Exception err, object dataObject)
		{
			var returnValue = this.Logger.Exception(err, dataObject);
			this.ExceptionReporter.Exception(returnValue);
			return returnValue;
		}

		public ServiceMessage ExceptionForObject(string errorMessage, object dataObject)
		{
			var err = new Exception(errorMessage);
			var returnValue = this.Logger.Exception(err, dataObject);
			this.ExceptionReporter.Exception(returnValue);
			return returnValue;
		}

		public ServiceMessage Info(string message)
		{
			return this.Logger.Info(message);
		}

		public ServiceMessage InfoForObject(string message, object dataObject)
		{
			return this.Logger.InfoForObject(message, dataObject);
		}

		public ServiceMessage WarningForObject(string message, object dataObject)
		{
			return this.Logger.WarningForObject(message, dataObject);
		}
	}
}
