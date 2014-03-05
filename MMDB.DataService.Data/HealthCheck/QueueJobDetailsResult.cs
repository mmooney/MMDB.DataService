using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.HealthCheck
{
    public class QueueJobDetailsResult
    {
        public Type QueueDataType { get; set; }
        public List<QueueJobDetailsResultItem> ErrorItems { get; set; }
        public List<QueueJobDetailsResultItem> InProcessItems { get; set; }
        public List<QueueJobDetailsResultItem> SuspectInProcessItem { get; set; }

        public QueueJobDetailsResult()
        {
            this.ErrorItems = new List<QueueJobDetailsResultItem>();
            this.InProcessItems = new List<QueueJobDetailsResultItem>();
            this.SuspectInProcessItem = new List<QueueJobDetailsResultItem>();
        }
    }
}
