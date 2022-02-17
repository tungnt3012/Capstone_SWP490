using Capstone_SWP490.Helper;
using Capstone_SWP490.Models.coachViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using interfaces = Capstone_SWP490.Services.Interfaces;
using services = Capstone_SWP490.Services;

namespace Capstone_SWP490.Controllers.Coach
{
    public class SignUpController : Controller
    {
        private readonly RegistrationHelper registrationHelper = new RegistrationHelper();
        private readonly interfaces.Iapp_userService _iapp_UserService = new services.app_userService();
        // GET: SignUp
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(coachSignUpViewModel data)
        {
            try
            {
                if (_iapp_UserService.GetUserByUsername(data.email) != null)
                {
                    @ViewData["CREATE_RESULT"] = "Email is used, please try again!";
                    return View();
                }
                app_user coachUser = new app_user();
                coachUser.user_name = data.email;
                coachUser.full_name = coachUser.full_name;
                coachUser.active = false;
                coachUser.verified = false;
                coachUser.user_role = "COACH";
                coachUser.psw = registrationHelper.CreatePassword(8);
                coachUser.encrypted_psw = coachUser.psw;
                _iapp_UserService.CreateUser(coachUser);
                new MailHelper().sendMailToInsertedUser(coachUser);
            }
            catch (Exception e)
            {
                @ViewData["CREATE_RESULT"] = "SYSTEM ERROR, please try again !";
            }
            return View();
        }
    }
}