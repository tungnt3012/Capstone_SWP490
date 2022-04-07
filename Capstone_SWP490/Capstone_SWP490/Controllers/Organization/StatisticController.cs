using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using interfaces = Capstone_SWP490.Services.Interfaces;
using services = Capstone_SWP490.Services;
using log4net;
using Capstone_SWP490.Models.statisticViewModel;
using Capstone_SWP490.Constant.Const;
using Capstone_SWP490.Helper;

namespace Capstone_SWP490.Controllers.Organization
{
    public class StatisticController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(StatisticController));
        private readonly interfaces.IschoolService _ischoolService = new services.schoolService();
        private readonly interfaces.Iapp_userService _iapp_UserService = new services.app_userService();

        // GET: Statistic
        public ActionResult Index()
        {
            statistic_index_ViewModel model = new statistic_index_ViewModel();
            try
            {

                model.school_confirrmation = _ischoolService.findSchoolConfirmation();
                model.total_registered_school = _ischoolService.getRegistered();
                model.total_contestant = _ischoolService.getTotalContestantInRegistered();
                return View(model);
            }
            catch (Exception e)
            {
                Log.Error(e);
                return RedirectToAction(ACTION_CONST.Home.INDEX, ACTION_CONST.Home.CONTROLLER);
            }
        }
        public ActionResult RegistrationAction(int? schoolId, string action, string note)
        {
            try
            {
                if (schoolId == null)
                {
                    Log.Error("School ID is null");
                    return RedirectToAction(ACTION_CONST.Home.INDEX, ACTION_CONST.Home.CONTROLLER);
                }
                if (!action.Equals("1"))
                {
                    if (!action.Equals("2"))
                    {
                        Log.Error("Action fail");
                        return RedirectToAction(ACTION_CONST.Home.INDEX, ACTION_CONST.Home.CONTROLLER);
                    }
                }

                school school = _ischoolService.findById((int)schoolId);
                if (action.Equals("1"))
                {
                    school.active = 3;
                }
                else if (action.Equals("2"))
                {
                    school.active = -1;
                }
                _ischoolService.update(school);
                app_user coachUser = _iapp_UserService.getByUserId((int)school.coach_id);
                new MailHelper().sendMailConfrimRegistration(coachUser, action.Equals("1") ? "Accepted" : "Rejected", note);
                return RedirectToAction(ACTION_CONST.Statistic.INDEX, ACTION_CONST.Statistic.CONTROLLER);
            }
            catch (Exception e)
            {
                Log.Error(e);
                return RedirectToAction(ACTION_CONST.Home.INDEX, ACTION_CONST.Home.CONTROLLER);
            }
        }
        public ActionResult DetailSchool(int? schoolId)
        {
            if (schoolId == null)
            {
                Log.Error("School ID is null");
                return RedirectToAction(ACTION_CONST.Home.INDEX, ACTION_CONST.Home.CONTROLLER);
            }
            school school = _ischoolService.findById((int)schoolId);
            return View(school);
        }
    }
}