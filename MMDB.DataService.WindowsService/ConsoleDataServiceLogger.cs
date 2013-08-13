using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data;
using MMDB.DataService.Data.Dto.Logging;
using MMDB.DataService.Data.Impl;

namespace MMDB.DataService.WindowsService
{
	public class ConsoleDataServiceLogger : DataServiceLogger
	{
		public ConsoleDataServiceLogger() : base(null)
		{
		}

		public override ServiceMessage TraceForObject(string message, object dataObject)
		{
			var traceMessage = new ServiceMessage
			{
				Level = EnumServiceMessageLevel.Trace,
				Message = message,
				Detail = message,
				MessageDateTimeUtc = DateTime.UtcNow,
				DataObjectJson = (dataObject != null) ? dataObject.ToJson() : null
			};
			this.WriteServiceMessage(traceMessage);
			return traceMessage;
		}

		public override ServiceMessage InfoForObject(string message, object dataObject)
		{
			var infoMessage = new ServiceMessage
			{
				Level = EnumServiceMessageLevel.Info,
				Message = message,
				Detail = message,
				MessageDateTimeUtc = DateTime.UtcNow,
				DataObjectJson = (dataObject != null) ? dataObject.ToJson() : null
			};
			this.WriteServiceMessage(infoMessage);
			return infoMessage;
		}

		public override ServiceMessage Info(string message)
		{
			return this.InfoForObject(message, null);
		}

		public override ServiceMessage Trace(string message)
		{
			var traceMessage = new ServiceMessage
			{
				Level = EnumServiceMessageLevel.Trace,
				Message = message,
				Detail = message,
				MessageDateTimeUtc = DateTime.UtcNow
			};
			this.WriteServiceMessage(traceMessage);
			return traceMessage;
		}

		public override ServiceMessage WarningForObject(string message, object dataObject)
		{
			var warningMessage = new ServiceMessage
			{
				Level = EnumServiceMessageLevel.Warning,
				Message = message,
				Detail = message,
				MessageDateTimeUtc = DateTime.UtcNow,
				DataObjectJson = (dataObject != null) ? dataObject.ToJson() : null
			};
			this.WriteServiceMessage(warningMessage);
			return warningMessage;
		}

		public override ServiceMessage Exception(Exception err, object dataObject = null)
		{
			var exceptionMessage = new ServiceMessage
			{
				Level = EnumServiceMessageLevel.Error,
				Message = err.Message,
				Detail = err.ToString(),
				MessageDateTimeUtc = DateTime.UtcNow,
				DataObjectJson = (dataObject != null) ? dataObject.ToJson(true) : null
			};
			this.WriteServiceMessage(exceptionMessage);
			return exceptionMessage;
		}

		public override int GetEventCount(EnumServiceMessageLevel? level)
		{
			throw new NotImplementedException();
		}

		public override IQueryable<ServiceMessage> GetEventList(int pageIndex, int pageSize, EnumServiceMessageLevel? level)
		{
			throw new NotImplementedException();
		}

		public override ServiceMessage GetEventItem(int id)
		{
			throw new NotImplementedException();
		}

		private void WriteServiceMessage(ServiceMessage message)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("[{0}: {1}] {2} - {3}",message.Level, message.MessageDateTimeUtc, message.Message, message.Detail);
			if(!string.IsNullOrEmpty(message.DataObjectJson))
			{
				sb.AppendLine();
				sb.AppendLine("JSON:");
				sb.AppendLine(message.DataObjectJson);
				sb.AppendLine();
			}
			sb.AppendLine();

			Console.WriteLine(sb.ToString());
		}
	}
}
