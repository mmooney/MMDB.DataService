using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Dto.Jobs
{
	public static class JobDataExtensions 
	{
		public static IQueryable<JobData> ByStatus(this IQueryable<JobData> query, EnumJobStatus status)
		{
			return query.Where(i=>i.Status == status);
		}

	}
}
