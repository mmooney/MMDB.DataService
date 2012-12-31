﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;

namespace MMDB.DataService.WindowsService
{
	public static class NinjectBootstrapper
	{
		public static IKernel Kernel { get; set; }

		public static void Initialize()
		{
			var kernel = NinjectBootstrapper.Kernel = new StandardKernel();
		}

		public static object Get(Type type)
		{
			return NinjectBootstrapper.Kernel.Get(type);
		}

		public static T Get<T>()
		{
			return NinjectBootstrapper.Kernel.Get<T>();
		}
	}
}
