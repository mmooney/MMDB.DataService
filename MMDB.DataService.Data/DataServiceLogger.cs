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

		public ServiceMessage Trace(string message, object[] args)
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
			var traceMessage = new ServiceMessage
			{
				Level = EnumServiceMessageLevel.Trace,
				Message = message,
				Detail = message,
				MessageDateTimeUtc = DateTime.UtcNow
			};
			this.DocumentSession.Store(traceMessage);
			this.DocumentSession.SaveChanges();
			return traceMessage;
		}

		public ServiceMessage InfoForObject(string message, object dataObject)
		{
			var infoMessage = new ServiceMessage
			{
				Level = EnumServiceMessageLevel.Info,
				Message = message,
				Detail = message,
				MessageDateTimeUtc = DateTime.UtcNow,
				DataObjectJson = (dataObject!=null) ? dataObject.ToJson() : null
			};
			this.DocumentSession.Store(infoMessage);
			this.DocumentSession.SaveChanges();
			return infoMessage;
		}

		public ServiceMessage WarningForObject(string message, object dataObject)
		{
			var warningMessage = new ServiceMessage
			{
				Level = EnumServiceMessageLevel.Warning,
				Message = message,
				Detail = message,
				MessageDateTimeUtc = DateTime.UtcNow,
				DataObjectJson = (dataObject != null) ? dataObject.ToJson() : null
			};
			this.DocumentSession.Store(warningMessage);
			this.DocumentSession.SaveChanges();
			return warningMessage;
		}

		public ServiceMessage Exception(Exception err, object dataObject = null)
		{
			var exceptionMessage = new ServiceMessage
			{
				Level = EnumServiceMessageLevel.Error,
				Message = err.Message,
				Detail = err.ToString(),
				MessageDateTimeUtc = DateTime.UtcNow,
				DataObjectJson = (dataObject != null) ? dataObject.ToJson(true) : null
			};
			this.DocumentSession.Store(exceptionMessage);
			this.DocumentSession.SaveChanges();
			return exceptionMessage;
		}

		public int GetEventCount(EnumServiceMessageLevel? level)
		{
			if (level.HasValue)
			{
				return this.DocumentSession.Query<ServiceMessage>().Where(i => i.Level == level.Value).Count();
			}
			else
			{
				return this.DocumentSession.Query<ServiceMessage>().Count();
			}
		}

		public IQueryable<ServiceMessage> GetEventList(int pageIndex, int pageSize, EnumServiceMessageLevel? level)
		{
			if(level.HasValue)
			{
				return this.DocumentSession.Query<ServiceMessage>().Where(i=>i.Level == level.Value).OrderByDescending(i => i.MessageDateTimeUtc).Skip((pageIndex) * pageSize).Take(pageSize);
			}
			else 
			{
				return this.DocumentSession.Query<ServiceMessage>().OrderByDescending(i => i.MessageDateTimeUtc).Skip((pageIndex) * pageSize).Take(pageSize);
			}
		}
	}
}
