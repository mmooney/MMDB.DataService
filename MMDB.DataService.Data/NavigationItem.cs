using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data
{
	public class NavigationItem
	{
		public string DisplayValue { get; set; }
		public string ControllerName { get; set; }
		public string ActionName { get; set; }

		public NavigationItem()
		{
		}

		public NavigationItem(string controllerName)
		{
			this.DisplayValue = controllerName;
			this.ControllerName = controllerName;
		}

		public NavigationItem(string displayValue, string controllerName)
		{
			this.DisplayValue = displayValue;
			this.ControllerName = controllerName;
		}

		public NavigationItem(string displayValue, string controllerName, string actionName)
		{
			this.DisplayValue = displayValue;
			this.ControllerName = controllerName;
			this.ActionName = actionName;
		}
	}
}
