using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data.Tests.X12
{
	public abstract class X12TestBase
	{
		public X12Verifier Verifier { get; set; }

		public X12TestBase(params char[] lineSeperators)
		{
			this.Verifier = new X12Verifier(lineSeperators);
		}
	}
}
