using MMDB.DataService.Data.Dto.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.HealthCheck
{
    public class QueueJobDetailsResultItem
    {
        public int Id { get; set; }
        public EnumJobStatus Status { get; set; }
        public DateTime QueueDateTimeUtc { get; set; }
        public List<JobException> ExceptionList { get; set; }
        public string JsonData { get; set; }
    }
}
