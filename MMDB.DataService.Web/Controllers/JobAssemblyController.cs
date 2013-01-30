using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMDB.DataService.Data;
using MMDB.DataService.Data.Dto.Assemblies;

namespace MMDB.DataService.Web.Controllers
{
    public class JobAssemblyController : Controller
    {
		private AssemblyManager AssemblyManager { get; set; }

		public JobAssemblyController(AssemblyManager assemblyManager)
		{
			this.AssemblyManager = assemblyManager;
		}

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /JobAssembly/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /JobAssembly/Create

		//public ActionResult Create()
		//{
		//	return View();
		//} 

        //
        // POST: /JobAssembly/Create

		//[HttpPost]
		//public ActionResult Create(HttpPostedFileBase file)
		//{
		//	try
		//	{
		//		var result = this.AssemblyManager.ImportAssembly(file.FileName, file.InputStream);
		//		return View("CreateComplete", result);
		//	}
		//	catch(Exception err)
		//	{
		//		var result = new AssemblyImportResult
		//		{
		//			Success = false,
		//			ErrorMessage = err.ToString()
		//		};
		//		return View("CreateComplete", result);
		//	}
		//}
        
        //
        // GET: /JobAssembly/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /JobAssembly/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /JobAssembly/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /JobAssembly/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
