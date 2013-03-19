using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MMDB.DataService.Web.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult Login(string returnUrl)
        {
            if(string.IsNullOrEmpty(returnUrl))
			{
				return RedirectToAction("Index","Home");
			}
			else 
			{
				return Redirect(returnUrl);
			}
        }

    }
}
