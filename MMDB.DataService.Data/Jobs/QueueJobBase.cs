using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Jobs;
using Raven.Client;

namespace MMDB.DataService.Data.Jobs
{
	public abstract class QueueJobBase<ConfigType, JobDataType> : DataServiceJobBase<ConfigType> where ConfigType:JobConfigurationBase where JobDataType:JobData
	{
		public QueueJobBase(IDocumentSession documentSession) : base(documentSession)
		{
				
		}
	}
}
