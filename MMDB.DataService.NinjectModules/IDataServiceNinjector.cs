using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;

namespace MMDB.DataService.NinjectModules
{
	public interface IDataServiceNinjector
	{
		void Setup(IKernel kernel);
	}
}
