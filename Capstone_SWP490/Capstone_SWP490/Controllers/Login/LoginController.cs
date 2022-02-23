using Capstone_SWP490.Models.app_userViewModel;
using Capstone_SWP490.Models.memberViewModel;
using Capstone_SWP490.Services;
using Capstone_SWP490.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Capstone_SWP490.Controllers
{
    public class LoginController : Controller
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
                    ViewData["LoginError"] = "User is Deactive";
                    return View();
                }
                if (_iapp_UserService.CheckLogin(app_User))
                {
                    //add session
                    Session.Add("username", user.user_name);
                    Session["profile"] = user;
                    if (user.verified == false)
                    {
                        return RedirectToAction("ChangePasswordFirst", "Login");
                    }
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
            return RedirectToAction("Login", "Login");
        }

        public ActionResult ChangePasswordFirst()
        {
            if (HttpContext.Session["username"] != null)
            {
                var u = _iapp_UserService.GetUserByUsername(HttpContext.Session["username"].ToString());
                if (u.verified == true)
                {
                    return RedirectToAction("ChangePassword", "Login");
                }
                return View(new app_userViewModel());
            }
            return RedirectToAction("Login", "Login");
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
                    if (await _iapp_UserService.UpdatePasswordFirst(HttpContext.Session["username"].ToString(), passToData, app_UserIn.send_me_event))
                    {
                        return RedirectToAction("RegisShirtSizing", "Login");
                    }
                }
                ViewData["ChangePasswordError"] = "Change Password fail";
                return View();
            }
            return RedirectToAction("Login", "Login");
        }

        public ActionResult ChangePassword()
        {
            if (HttpContext.Session["username"] != null)
            {
                var u = _iapp_UserService.GetUserByUsername(HttpContext.Session["username"].ToString());
                if (u.verified == false)
                {
                    return RedirectToAction("ChangePasswordFirst", "Login");
                }
                return View(new app_userViewModel());
            }
            return RedirectToAction("Login", "Login");
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
                    if (await _iapp_UserService.UpdatePassword(HttpContext.Session["username"].ToString(), passToData))
                    {
                        ViewData["ChangePasswordSuccess"] = "Change Password Successfully";
                        return View();
                    }
                }
                ViewData["ChangePasswordError"] = "Change Password fail";
                return View();
            }
            return RedirectToAction("Login", "Login");
        }

        public ActionResult RegisShirtSizing()
        {
            if (HttpContext.Session["username"] != null)
            {
                return View(new member());
            }
            return RedirectToAction("Login", "Login");
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
