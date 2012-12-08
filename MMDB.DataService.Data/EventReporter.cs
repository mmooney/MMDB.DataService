using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data
{
	public class EventReporter
	{
		private DataServiceLogger Logger { get; set; }
		private ExceptionReporter ExceptionReporter { get; set; }

		public EventReporter(DataServiceLogger logger, ExceptionReporter exceptionReporter)
		{
			this.Logger = logger;
		}

		public void Trace(string message, params object[] args)
		{
			this.Logger.Trace(message, args);
		}

		public void Exception(Exception err)
		{
			this.Logger.Exception(err);
			this.ExceptionReporter.Exception(err);
		}

		public void ExceptionForObject(Exception err, object dataObject)
		{
			this.Logger.Exception(err, dataObject);
			this.ExceptionReporter.Exception(err, dataObject);
		}

		public void ExceptionForObject(string errorMessage, object dataObject)
		{
			var err = new Exception(errorMessage);
			this.Logger.Exception(err, dataObject);
			this.ExceptionReporter.Exception(err, dataObject);
		}

		public void InfoForObject(string message, object dataObject)
		{
			this.Logger.InfoForObject(message, dataObject);
		}

		public void WarningForObject(string message, object dataObject)
		{
			this.Logger.WarningForObject(message, dataObject);
		}
	}
}
