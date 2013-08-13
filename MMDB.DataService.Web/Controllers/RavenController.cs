using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMDB.DataService.Data;

namespace MMDB.DataService.Web.Controllers
{
    public class RavenController : Controller
    {
        private IRavenManager RavenManager { get; set; }

		public RavenController(IRavenManager ravenManager)
		{
			this.RavenManager = ravenManager;
		}
		//public ActionResult Index()
		//{
		//	return View();
		//}

		public ActionResult AttachmentList()
		{
			var attachmentList = this.RavenManager.GetAttachmentList();
			return View(attachmentList);
		}

		public ActionResult Attachment(string attachmentID, string contentType="text/plain")
		{
			var data = RavenManager.GetAttachment(attachmentID);
			return File(data, contentType);
		}

    }
}
