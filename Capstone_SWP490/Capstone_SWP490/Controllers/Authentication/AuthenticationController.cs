﻿using Capstone_SWP490.Constant.Const;
using Capstone_SWP490.Models.app_userViewModel;
using Capstone_SWP490.Models.memberViewModel;
using Capstone_SWP490.Services;
using Capstone_SWP490.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Capstone_SWP490.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly Iapp_userService _iapp_UserService = new app_userService();
        private readonly ImemberService _imemberService = new memberService();

        // GET: Login
        public ActionResult Index()
        {
            return View("Login");
        }

        // GET: Login/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Login/Create
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ForgotPassword(app_userViewModel usersIn)
        {
            var rs = await _iapp_UserService.ForgotPassword(usersIn.email);
            if (rs)
            {
                ViewData["SendSuccess"] = "Please check New Password in your Email!!!";
                return View(usersIn);
            }
            ViewData["SendError"] = "Email not exist, please check again!!!";
            return View(usersIn);
        }
        public ActionResult ResetPassword()
        {
            if (HttpContext.Session["username"] != null)
            {
                return View();
            }
            return RedirectToAction("Login", "Authentication");
        }
        [HttpPost]
        public async Task<ActionResult> ResetPassword(reset_password reset)
        {
            if (HttpContext.Session["username"] != null)
            {
                if (await _iapp_UserService.ResetPassword(HttpContext.Session["username"].ToString(), reset.old_password, reset.new_password))
                {
                    ViewData["ChangePasswordSuccess"] = "Change password Successfull!!!";
                    return View(reset);
                }
                ViewData["ChangePasswordError"] = "Change password Fail!!!";
                return View(reset);
            }
            return RedirectToAction("Login", "Authentication");
        }


        //public ActionResult ViewHome()
        //{
        //    string usernamess = HttpContext.Session["username"].ToString();
        //    var user = _iapp_UserService.GetUserByUsername(usernamess);
        //    if (user != null)
        //    {
        //        ViewData["userCurrent"] = user;
        //    }
        //    return View();
        //}

        public ActionResult Login()
        {
            return View(new app_user());
        }

        [HttpPost]
        public ActionResult Login(app_user app_User)
        {

            if (HttpContext.Session["username"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            var user = _iapp_UserService.GetUserByUsername(app_User.user_name);
         
            if (user != null)
            {
                if (user.active == false)
                {
                    string msg;
                    if (user.user_role.Equals(APP_CONST.APP_ROLE.getUserRole(1)))
                    {
                        msg = "Your account is not accepted by organization yet !";
                    }
                    else
                    {
                        msg = "Your account has been deactived";
                    }
                    ViewData["LoginError"] = msg;
                    return View();
                }
                if (_iapp_UserService.CheckLogin(app_User))
                {
                    //add session
                    Session.Add("username", user.user_name);
                    Session["profile"] = user;
                    if (user.confirm_password == 0)
                    {
                        return RedirectToAction("ResetPassword", "Authentication");
                    }
                    if (user.verified == false)
                    {
                        FormsAuthentication.SetAuthCookie(user.user_name, false);
                        return RedirectToAction("ChangePasswordFirst", "Authentication");
                    }
                    var memberTemp = _imemberService.GetMemberByAvaibleUserId(user.user_id);

                    if (memberTemp != null)
                    {
                        if (String.IsNullOrWhiteSpace(memberTemp.shirt_sizing))
                        {
                            FormsAuthentication.SetAuthCookie(user.user_name, false);
                            return RedirectToAction("RegisShirtSizing", "Authentication");
                        }
                    }
                    FormsAuthentication.SetAuthCookie(user.user_name, false);
                    return RedirectToAction("Index", "Home");
                }
                ViewData["LoginError"] = "Wrong username or password";
                return View();
            }
            ViewData["LoginError"] = "Username not exist";
            return View();
        }

        public ActionResult Logout()
        {
            //remove session
            Session.RemoveAll();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Authentication");
        }

        public ActionResult ChangePasswordFirst()
        {
            if (HttpContext.Session["username"] != null)
            {
                var u = _iapp_UserService.GetUserByUsername(HttpContext.Session["username"].ToString());
                if (u.verified == true)
                {
                    return RedirectToAction("ChangePassword", "Authentication");
                }
                return View(new app_userViewModel());
            }
            return RedirectToAction("Login", "Authentication");
        }

        [HttpPost]
        public async Task<ActionResult> ChangePasswordFirst(app_userViewModel app_UserIn)
        {
            if (HttpContext.Session["username"] != null)
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                UTF8Encoding utf8 = new UTF8Encoding();
                byte[] data = md5.ComputeHash(utf8.GetBytes(app_UserIn.psw));

                var passToData = Convert.ToBase64String(data);

                if (!String.IsNullOrWhiteSpace(app_UserIn.psw) && !String.IsNullOrWhiteSpace(app_UserIn.repsw)
                     && app_UserIn.psw == app_UserIn.repsw
                     && app_UserIn.psw.Length >= 6 && app_UserIn.repsw.Length >= 6)
                {
                    if (await _iapp_UserService.UpdatePasswordFirst(HttpContext.Session["username"].ToString(), app_UserIn.psw, passToData, app_UserIn.send_me_event))
                    {
                        return RedirectToAction("RegisShirtSizing", "Authentication");
                    }
                }
                ViewData["ChangePasswordError"] = "Change Password fail";
                return View();
            }
            return RedirectToAction("Login", "Authentication");
        }

        //[Authorize/*(Roles = "ORGANIZER")*/]
        public ActionResult ChangePassword()
        {
            Console.WriteLine("role ");
            if (HttpContext.Session["username"] != null)
            {
                var u = _iapp_UserService.GetUserByUsername(HttpContext.Session["username"].ToString());
                if (u.verified == false)
                {
                    return RedirectToAction("ChangePasswordFirst", "Authentication");
                }
                return View(new app_userViewModel());
            }
            return RedirectToAction("Login", "Authentication");
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(app_userViewModel app_UserIn)
        {
            if (HttpContext.Session["username"] != null)
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                UTF8Encoding utf8 = new UTF8Encoding();
                byte[] data = md5.ComputeHash(utf8.GetBytes(app_UserIn.psw));

                var passToData = Convert.ToBase64String(data);

                if (!String.IsNullOrWhiteSpace(app_UserIn.psw) && !String.IsNullOrWhiteSpace(app_UserIn.repsw)
                    && app_UserIn.psw == app_UserIn.repsw
                    && app_UserIn.psw.Length >= 6 && app_UserIn.repsw.Length >= 6)
                {
                    if (await _iapp_UserService.UpdatePassword(HttpContext.Session["username"].ToString(), app_UserIn.psw, passToData))
                    {
                        ViewData["ChangePasswordSuccess"] = "Change Password Successfully";
                        return View();
                    }
                }
                ViewData["ChangePasswordError"] = "Change Password fail";
                return View();
            }
            return RedirectToAction("Login", "Authentication");
        }

        public ActionResult RegisShirtSizing()
        {
            if (HttpContext.Session["username"] != null)
            {
                return View(new member());
            }
            return RedirectToAction("Login", "Authentication");
        }

        [HttpPost]
        public async Task<ActionResult> RegisShirtSizing(member member)
        {
            if (HttpContext.Session["username"] != null)
            {
                var userss = _iapp_UserService.GetUserByUsername(HttpContext.Session["username"].ToString());
                var m = _imemberService.GetMemberByUserId(userss.user_id);
                if (m != null)
                {
                    if (!String.IsNullOrEmpty(member.shirt_sizing))
                    {
                        var mem = await _imemberService.RegisterShirtSize(HttpContext.Session["username"].ToString(), member.shirt_sizing);
                        if (mem != null)
                        {
                            ViewData["Result"] = "Register Shirt Size Successfully !!!";
                            ViewData["color"] = "green";
                            return View(mem);
                            //return RedirectToAction("Index", "Home");
                        }
                    }
                    ViewData["Result"] = "Register Shirt Size Failed !!!";
                    ViewData["color"] = "red";
                    return View(m);
                }
            }
            return View();
        }

        // POST: Login/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Login/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Login/Edit/5
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

        // GET: Login/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Login/Delete/5
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