using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using MMDB.DataService.Data.Settings;
using MMDB.DataService.Web.Models;

namespace MMDB.DataService.Web.Controllers
{
    public class SettingsController : Controller
    {
		private SettingsManager SettingsManager { get; set; }

		public SettingsController(SettingsManager settingsManager)
		{
			this.SettingsManager = settingsManager;
		}
        //
        // GET: /Settings/

        public ActionResult Index()
        {
			var settingsList = this.SettingsManager.GetList();
            return View(settingsList);
        }

		public ActionResult Create(string className)
		{
			var types = new List<AssemblyClassItem>();
			var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
			Type selectedType = null;
			foreach(var assembly in loadedAssemblies)
			{
				var allTypes = (from i in assembly.GetTypes()
								where typeof(SettingsBase).IsAssignableFrom(i)
									&& !i.IsAbstract
								select new AssemblyClassItem 
									{ 
										AssemblyName=assembly.FullName, 
										ClassName=i.FullName 
								});
				if(selectedType == null && !string.IsNullOrEmpty(className))
				{
					selectedType = assembly.GetTypes().SingleOrDefault(i=>i.FullName == className);
				}
				types.AddRange(allTypes);
			}
			this.ViewBag.AvailableClassList = types;
			this.ViewBag.SelectedClassName = className;
			SettingsBase model = null;
			if(selectedType != null)
			{
				model = (SettingsBase)Activator.CreateInstance(selectedType);
			}
			return View(model);
		}
    }
}
