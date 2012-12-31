using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;

namespace MMDB.DataService.Data.Tests
{
	public static class DataServiceTestHelper
	{
		public static Mock<EventReporter> GetEventReporter()
		{
			var logger = new Mock<DataServiceLogger>();
			var exceptionReporter = new Mock<ExceptionReporter>();
			return new Mock<EventReporter>(logger.Object, exceptionReporter.Object);
		}
	}
}
