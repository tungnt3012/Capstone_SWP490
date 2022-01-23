using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Capstone_SWP490.Controllers
{
    public class HomeController : Controller
    {
        
            public ActionResult HomeLogin()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult RegistrationTitle()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult RegistrationFrom()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
        public ActionResult RegistrationImport()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
      
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}