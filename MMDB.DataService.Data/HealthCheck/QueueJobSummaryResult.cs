using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.HealthCheck
{
    public class QueueJobSummaryResult
    {
        public List<QueueJobSummaryResultItem> Items { get; set; }

        public QueueJobSummaryResult()
        {
            this.Items = new List<QueueJobSummaryResultItem>();
        }
    }
}
