using MMDB.DataService.Data.Dto.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Jobs
{
    public class EmailSenderJob : ItemProcessingJob<NullJobConfiguration, EmailJobData>
    {
        private readonly IEmailManager _emailManager;

        public EmailSenderJob(IEventReporter eventReporter, IEmailManager emailManager) : base(eventReporter, singletonJob:true)
        {
            _emailManager = emailManager;
        }

        protected override EmailJobData GetNextItemToProcess(NullJobConfiguration configuration)
        {
            return _emailManager.PopNextJobData();
        }

        protected override void ProcessItem(NullJobConfiguration configuration, EmailJobData jobItem)
        {
            _emailManager.ProcessItem(jobItem);
        }

        protected override void MarkItemSuccessful(EmailJobData jobData)
        {
             _emailManager.MarkItemSuccessful(jobData);
        }

        protected override void MarkItemFailed(EmailJobData jobData, Exception err)
        {
            _emailManager.MarkItemFailed(jobData, err);
        }
    }
}
