using MMDB.DataService.Data.Dto.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.HealthCheck
{
    public interface IHealthChecker
    {
        QueueJobSummaryResult CheckQueueJobSummary();
        QueueJobDetailsResult CheckQueueJobDetails(string typeName);
        JobData RequeueQueueJob(int id, string typeName);
        JobData CancelQueueJob(int id, string typeName);
    }
}
