using MMDB.DataService.Data.Dto.Jobs;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.HealthCheck.HealthCheckImpl
{
    public class HealthChecker : IHealthChecker
    {
        private readonly IDocumentSession _documentSession;

        public HealthChecker(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }
        public QueueJobSummaryResult CheckQueueJobSummary()
        {
            var returnValue = new QueueJobSummaryResult();

            var baseType = typeof(JobData);
            var typeList = AppDomain.CurrentDomain.GetAssemblies().ToList()
                .SelectMany(s => s.GetTypes())
                .Where(p => baseType.IsAssignableFrom(p)
                        && p.IsClass).ToList();
            DateTime tenMinuetsAgo = DateTime.UtcNow.AddMinutes(-10);
            foreach (var type in typeList)
            {
                bool any = _documentSession.Advanced.LuceneQuery<object>()
                            .WhereEquals("@metadata.Raven-Entity-Name", _documentSession.Advanced.DocumentStore.Conventions.GetTypeTagName(type))
                            .Any();
                if (any)
                {
                    var item = new QueueJobSummaryResultItem
                    {
                        QueueDataType = type
                    };
                    item.ErrorCount = _documentSession.Advanced.LuceneQuery<object>()
                                .WhereEquals("@metadata.Raven-Entity-Name", _documentSession.Advanced.DocumentStore.Conventions.GetTypeTagName(type))
                                .AndAlso().WhereEquals("Status", EnumJobStatus.Error)
                                .WaitForNonStaleResultsAsOfNow(TimeSpan.FromSeconds(120))
                                .QueryResult.TotalResults;
                    item.InProcessCount = _documentSession.Advanced.LuceneQuery<object>()
                                .WhereEquals("@metadata.Raven-Entity-Name", _documentSession.Advanced.DocumentStore.Conventions.GetTypeTagName(type))
                                .AndAlso().WhereEquals("Status", EnumJobStatus.InProcess)
                                .WaitForNonStaleResultsAsOfNow(TimeSpan.FromSeconds(120))
                                .QueryResult.TotalResults;
                    item.SuspectInProcessCount = _documentSession.Advanced.LuceneQuery<object>()
                                .WhereEquals("@metadata.Raven-Entity-Name", _documentSession.Advanced.DocumentStore.Conventions.GetTypeTagName(type))
                                .AndAlso().WhereEquals("Status", EnumJobStatus.InProcess)
                                .AndAlso().WhereLessThan("QueuedDateTimeUtc", tenMinuetsAgo)
                                .WaitForNonStaleResultsAsOfNow(TimeSpan.FromSeconds(120))
                                .QueryResult.TotalResults;
                    returnValue.Items.Add(item);
                }
            }
            return returnValue;
        }


        public QueueJobDetailsResult CheckQueueJobDetails(string typeName)
        {
            var type = AppDomain.CurrentDomain.GetAssemblies().ToList()
                .SelectMany(s => s.GetTypes())
                .FirstOrDefault(i=>i.FullName == typeName);
            if(type == null)
            {
                throw new Exception("Unrecognized type " + typeName);
            }
            var returnValue = new QueueJobDetailsResult
            {
                QueueDataType = type
            };

            DateTime tenMinuetsAgo = DateTime.UtcNow.AddMinutes(-10);
            returnValue.ErrorItems = _documentSession.Advanced.LuceneQuery<object>()
                        .WhereEquals("@metadata.Raven-Entity-Name", _documentSession.Advanced.DocumentStore.Conventions.GetTypeTagName(type))
                        .AndAlso().WhereEquals("Status", EnumJobStatus.Error)
                        .WaitForNonStaleResultsAsOfNow(TimeSpan.FromSeconds(120))
                        .Select(i=>CreateQueueJobDetailsResultItem(i)).ToList();
            returnValue.InProcessItems = _documentSession.Advanced.LuceneQuery<object>()
                        .WhereEquals("@metadata.Raven-Entity-Name", _documentSession.Advanced.DocumentStore.Conventions.GetTypeTagName(type))
                        .AndAlso().WhereEquals("Status", EnumJobStatus.InProcess)
                        .WaitForNonStaleResultsAsOfNow(TimeSpan.FromSeconds(120))
                        .Select(i=>CreateQueueJobDetailsResultItem(i)).ToList();
            returnValue.SuspectInProcessItem = _documentSession.Advanced.LuceneQuery<object>()
                        .WhereEquals("@metadata.Raven-Entity-Name", _documentSession.Advanced.DocumentStore.Conventions.GetTypeTagName(type))
                        .AndAlso().WhereEquals("Status", EnumJobStatus.Error)
                        .AndAlso().WhereLessThan("QueuedDateTimeUtc", tenMinuetsAgo)
                        .WaitForNonStaleResultsAsOfNow(TimeSpan.FromSeconds(120))
                        .Select(i=>CreateQueueJobDetailsResultItem(i)).ToList();

            return returnValue;
        }

        private QueueJobDetailsResultItem CreateQueueJobDetailsResultItem(object i)
        {
            var jobData = (JobData)i;
            return new QueueJobDetailsResultItem
            {
                Id = jobData.Id,
                Status = jobData.Status,
                QueueDateTimeUtc = jobData.QueuedDateTimeUtc,
                ExceptionList = jobData.ExceptionList,
                JsonData = i.ToJson(true)
            };
        }


        public JobData RequeueQueueJob(int id, string typeName)
        {
            var type = AppDomain.CurrentDomain.GetAssemblies().ToList()
                .SelectMany(s => s.GetTypes())
                .FirstOrDefault(i=>i.FullName == typeName);
            if (type == null)
            {
                throw new Exception("Unrecognized type " + typeName);
            }
            var key = _documentSession.Advanced.DocumentStore.Conventions.GetTypeTagName(type) + "/" + id;
            var data = _documentSession.Load<JobData>(key);
            data.Status = EnumJobStatus.New;
            return _documentSession.SaveEvict(data);
        }


        public JobData CancelQueueJob(int id, string typeName)
        {
            var type = AppDomain.CurrentDomain.GetAssemblies().ToList()
                .SelectMany(s => s.GetTypes())
                .FirstOrDefault(i => i.FullName == typeName);
            if (type == null)
            {
                throw new Exception("Unrecognized type " + typeName);
            }
            var key = _documentSession.Advanced.DocumentStore.Conventions.GetTypeTagName(type) + "/" + id;
            var data = _documentSession.Load<JobData>(key);
            data.Status = EnumJobStatus.Cancelled;
            return _documentSession.SaveEvict(data);
        }
    }
}
