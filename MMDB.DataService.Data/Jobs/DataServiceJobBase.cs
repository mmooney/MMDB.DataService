using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;

namespace MMDB.DataService.Data.Jobs
{
	public abstract class DataServiceJobBase<ConfigType> where ConfigType: JobConfigurationBase
	{
		public DataServiceJobBase()
		{
		}

		public abstract void Run(ConfigType configuration);
	}
}
