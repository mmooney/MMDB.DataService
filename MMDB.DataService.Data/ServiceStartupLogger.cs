using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MMDB.Shared;

namespace MMDB.DataService.Data
{
	public static class ServiceStartupLogger
	{
		public static void LogMessage(string message)
		{
			try 
			{
				if(AppSettingsHelper.GetBoolSetting("ServiceStartupLogger", true))
				{
					var now = DateTime.Now;
					string exePath = Assembly.GetExecutingAssembly().Location;
					string directory = Path.GetDirectoryName(exePath);
					string logFileName = string.Format("StartupLog_{0:d4}_{1:d2}_{2:d2}.log", now.Year, now.Month, now.Day);
					string logFilePath = Path.Combine(directory, logFileName);
					string formattedMessage = string.Format("{0}: {1}\r\n", now, message);
					File.AppendAllText(logFilePath, formattedMessage);
				}
			}
			catch {}
		}
	}
}
