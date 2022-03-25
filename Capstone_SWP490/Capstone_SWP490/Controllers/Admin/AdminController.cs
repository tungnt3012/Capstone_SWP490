﻿using Capstone_SWP490.Models;
using Capstone_SWP490.Services;
using Capstone_SWP490.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Capstone_SWP490.Controllers.Admin
{
    public class AdminController : Controller
    {
        private readonly Iapp_userService _iapp_UserService = new app_userService();
        private readonly Ipage_contentService _ipage_contentService = new page_contentService();

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

        public ActionResult ManagermentAccount()
        {
            if (HttpContext.Session["username"] != null)
            {
                var u = _iapp_UserService.GetUserByUsername(HttpContext.Session["username"].ToString());
                if (u.user_role.Equals("ADMIN"))
                {
                    var data = _iapp_UserService.GetListUsersManager(0, 1000);
                    ViewData["Users"] = data;
                    return View();
                }
                //ViewData["LoginError"] = "You NOT permission in this Function";
                //return RedirectToAction("Login", "Login");
            }
            ViewData["LoginError"] = "You NOT permission in this Function";
            return RedirectToAction("Login", "Login");
        }

        public ActionResult DisableUsers(int user_id)
        {
            ViewData["Users"] = _iapp_UserService.SwitchableUsers(user_id, false);
            return RedirectToAction("ManagermentAccount", "Admin");
        }

        public ActionResult EnableUsers(int user_id)
        {
            ViewData["Users"] = _iapp_UserService.SwitchableUsers(user_id, true);
            return RedirectToAction("ManagermentAccount", "Admin");
        }

        public ActionResult NextPage(int pageIndex)
        {
            var lstPosts = _iapp_UserService.GetListUsersManager(pageIndex, 10);
            ViewData["Users"] = lstPosts;
            return View("ManagermentAccount");
        }

        public ActionResult ManagermentRole()
        {
            return View();
        }

        public ActionResult GetMenuContentByRole(string roleName)
        {
            return Json(_ipage_contentService.GetMenuContentByRole(roleName), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> HideMenuContent(int content_id)
        {
            AjaxResponseViewModel<bool> ajaxResponse = await _ipage_contentService.UpdateStatusMenuContent(content_id, 0);
            return Json(ajaxResponse);
        }

        [HttpPost]
        public async Task<ActionResult> ShowMenuContent(int content_id)
        {
            AjaxResponseViewModel<bool> ajaxResponse = await _ipage_contentService.UpdateStatusMenuContent(content_id, 1);
            return Json(ajaxResponse);
        }

    }
}