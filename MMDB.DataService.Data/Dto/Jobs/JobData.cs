using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Dto.Jobs
{
	public abstract class JobData
	{
		public int Id { get; set; }
		public List<int> ExceptionIdList { get; set; }
        public List<JobException> ExceptionList { get; set; }
		public EnumJobStatus Status { get; set; }

		public JobData()
		{
			this.ExceptionIdList = new List<int>();
            this.ExceptionList = new List<JobException>();
		}

        public JobException AddException(Exception err)
        {
            var item = new JobException
            {
                ExceptionDateTimeUtc = DateTime.UtcNow,
                ExceptionMessage = err.Message,
                ExceptionDetail = err.ToString()
            };
            this.ExceptionList.Add(item);
            return item;
        }
	}
}
