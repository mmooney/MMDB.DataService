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

		internal void Trace(string message, params object[] args)
		{
			this.Logger.Trace(message, args);
		}

		internal void Exception(Exception err)
		{
			this.Logger.Exception(err);
			this.ExceptionReporter.Exception(err);
		}
	}
}
