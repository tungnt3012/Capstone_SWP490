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
using System.Threading.Tasks;

namespace Capstone_SWP490.Controllers.Organization
{
    public class StatisticController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(StatisticController));
        private readonly interfaces.IschoolService _ischoolService = new services.schoolService();
        private readonly interfaces.Iapp_userService _iapp_UserService = new services.app_userService();
        private readonly interfaces.IeventService _ieventService = new services.eventService();

        // GET: Statistic
        public ActionResult Index()
        {
            statistic_index_ViewModel model = new statistic_index_ViewModel();
            try
            {

                model.school_confirrmation = _ischoolService.findSchoolConfirmation();
                model.total_registered_school = _ischoolService.getRegistered();
                model.total_contestant = _ischoolService.getTotalContestantInRegistered();

                model.statistic_EventViewModel = _ieventService.EventStatic();
                return View(model);
            }
            catch (Exception e)
            {
                Log.Error(e);
                return RedirectToAction(ACTION_CONST.Home.INDEX, ACTION_CONST.Home.CONTROLLER);
            }
        }
        public async Task<ActionResult> RegistrationAction(int? schoolId, string type, string note)
        {
            try
            {
                if (schoolId == null)
                {
                    Log.Error("School ID is null");
                    return RedirectToAction(ACTION_CONST.Home.INDEX, ACTION_CONST.Home.CONTROLLER);
                }
                await _ischoolService.processSchool((int)schoolId, type);
                school school = _ischoolService.findById((int)schoolId);
                app_user coachUser = _iapp_UserService.getByUserId((int)school.coach_id);
                new MailHelper().sendMailConfrimRegistration(coachUser, type.Equals("1") ? "Accepted" : "Rejected", note);
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

        public ActionResult TeamContest()
        {
            return View();
        }

        public ActionResult DownloadTeamContest()
        {
            return View();
        }

        public ActionResult RegisteredTeam()
        {
            return View();
        }

        public ActionResult RegisteredSchool()
        {
            return View();
        }
        public ActionResult RegistrationSchedule(int eventId)
        {
            var st = _ieventService.AllUsersInEvent(eventId);
            if (st != null)
            {
                return View(st);
            }
            return View(new statistic_eventDetailViewModel { event_id = 0 });
        }
    }
}