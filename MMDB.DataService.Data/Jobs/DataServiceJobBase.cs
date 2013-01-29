using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;

namespace MMDB.DataService.Data.Jobs
{
	public abstract class DataServiceJobBase<ConfigType> where ConfigType: JobConfigurationBase
	{
		protected IDocumentSession DocumentSession { get; set; }

		public DataServiceJobBase(IDocumentSession documentSession)
		{
			this.DocumentSession = documentSession;	
		}

		public abstract void Run(ConfigType configuration);
	}
}
