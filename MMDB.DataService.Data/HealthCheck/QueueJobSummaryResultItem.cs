using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.HealthCheck
{
    public class QueueJobSummaryResultItem
    {
        public Type QueueDataType { get; set; }
        public int ErrorCount { get; set; }
        public int InProcessCount { get; set; }
        public int SuspectInProcessCount { get; set; }
    }
}
