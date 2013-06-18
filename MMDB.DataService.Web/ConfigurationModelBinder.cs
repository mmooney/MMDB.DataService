using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMDB.DataService.Data;
using MMDB.DataService.Data.Jobs;

namespace MMDB.DataService.Web
{
	public class ConfigurationModelBinder : DefaultModelBinder
	{
		private TypeLoader TypeLoader { get; set; }
		private IJobManager JobManager { get; set; }

		public ConfigurationModelBinder(TypeLoader typeLoader, IJobManager jobManager)
		{
			this.TypeLoader = typeLoader;
			this.JobManager = jobManager;
		}

		protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
		{
			if(modelType == typeof(JobConfigurationBase))
			{
				string assemblyName = controllerContext.RequestContext.HttpContext.Request["AssemblyName"];
				string className = controllerContext.RequestContext.HttpContext.Request["ClassName"];
				var jobType = this.TypeLoader.LoadType(assemblyName, className);
				var configType = this.JobManager.GetJobConfigurationType(jobType);
				return base.CreateModel(controllerContext, bindingContext, configType);
			}
			else 
			{
				return base.CreateModel(controllerContext, bindingContext, modelType);
			}
		}
	}
}