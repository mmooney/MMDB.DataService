using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using MMDB.DataService.Data.Dto.Logging;

namespace MMDB.DataService.Data
{
	public class DataServiceLogger
	{
		private IDocumentSession DocumentSession { get; set; }

		public DataServiceLogger(IDocumentSession documentSession)
		{
			this.DocumentSession = documentSession;
		}

		public void Trace(string message, object[] args)
		{
			string formattedMessage;
			if (args == null || args.Length == 0)
			{
				formattedMessage = message;
			}
			else
			{
				formattedMessage = string.Format(message, args);
			}
			var traceMessage = new TraceMessage
			{
				Message = message,
				Detail = message,
				MessageDateTimeUtc = DateTime.UtcNow
			};
			this.DocumentSession.Store(traceMessage);
			this.DocumentSession.SaveChanges();
		}

		public void Exception(Exception err)
		{
			this.DocumentSession.Store(err);
			this.DocumentSession.SaveChanges();
		}
	}
}
