using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;

namespace MMDB.DataService.Data.DataProvider
{
	public class RavenServerProvider : IRavenProvider
	{
		public RavenServerProvider()
		{
		}

		public IDocumentSession CreateSession()
		{
			throw new NotImplementedException();
		}
	}
}
