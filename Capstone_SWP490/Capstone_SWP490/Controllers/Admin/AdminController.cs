using Capstone_SWP490.Services;
using Capstone_SWP490.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Capstone_SWP490.Controllers.Admin
{
    public class AdminController : Controller
    {
        private readonly Iapp_userService _iapp_UserService = new app_userService();

        // GET: Admin
        public ActionResult Index()
        {
            if (HttpContext.Session["username"] != null)
            {
                var u = _iapp_UserService.GetUserByUsername(HttpContext.Session["username"].ToString());
                if (u.user_role.Equals("ORGANIZER"))
                {
                    return View();
                }
                //ViewData["LoginError"] = "You NOT permission in this Function";
                //return RedirectToAction("Login", "Login");
            }
            ViewData["LoginError"] = "You NOT permission in this Function";
            return RedirectToAction("Login", "Login");
        }
        public ActionResult Admin()
        {
            return View();
        }
    }
}