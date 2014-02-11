using MMDB.DataService.Data.Dto.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data
{
    public interface IEmailManager
    {
        EmailJobData PopNextJobData();
        void ProcessItem(EmailJobData jobItem);
        void MarkItemSuccessful(EmailJobData jobData);
        void MarkItemFailed(EmailJobData jobData, Exception err);
    }
}
