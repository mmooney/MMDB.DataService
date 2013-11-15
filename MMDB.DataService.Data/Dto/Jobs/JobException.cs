using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Dto.Jobs
{
    public class JobException
    {
        public DateTime ExceptionDateTimeUtc { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionDetail { get; set; }
    }
}
