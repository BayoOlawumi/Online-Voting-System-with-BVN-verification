using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SeunEvote.Models;

namespace SeunEvote.Controllers
{
    public class SessionExpireFilterAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["AdminId"] == null)
            {
                filterContext.Result = new RedirectResult("~/Home/Index");
                return;
            }
            

            base.OnActionExecuting(filterContext);
            
        }
    }
    public class AdminsController : Controller 
    {
        // GET: Admin 
        private IVotingModelEntities db = new IVotingModelEntities();

        
        public ActionResult Index(string report)
        {
            ViewBag.Report = report;
            return View();
        }
   
      [HttpPost]
      
        public ActionResult Login([Bind(Include = "Id,Surname,Password")] Adminee admin)
        {
            if (ModelState.IsValid)
            {

                var report = db.Adminees.Where(a => a.Password.Equals(admin.Password) && a.Surname.Equals(admin.Surname)).FirstOrDefault();

                if (report != null)
                {
                    Session["AdminId"] = report.Id.ToString();
                    Session["AdminName"] = report.Surname.ToString();
                    return RedirectToAction("Index","elections", new { id = report.Id });
                }

            }
            return RedirectToAction("Index", new { report = "Invalid Login Details" });
        }
        
    }
}