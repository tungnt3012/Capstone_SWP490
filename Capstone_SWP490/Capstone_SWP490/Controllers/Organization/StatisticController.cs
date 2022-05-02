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
using Capstone_SWP490.Sercurity;
using System.IO;
using OfficeOpenXml;

namespace Capstone_SWP490.Controllers.Organization
{
    [AuthorizationAccept(Roles = "ORGANIZER")]
    public class StatisticController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(StatisticController));
        private readonly interfaces.IschoolService _ischoolService = new services.schoolService();
        private readonly interfaces.Iapp_userService _iapp_UserService = new services.app_userService();
        private readonly interfaces.IeventService _ieventService = new services.eventService();
        private readonly interfaces.ImemberService _imemberService = new services.memberService();
        private readonly interfaces.IteamService _iteamService = new services.teamService();
        private readonly interfaces.IcontestService _icontestService = new services.contestService();

        [AuthorizationAccept(Roles = "ORGANIZER")]
        // GET: Statistic
        public ActionResult Index()
        {
            statistic_index_ViewModel model = new statistic_index_ViewModel();
            try
            {
                model.school_confirrmation = _ischoolService.findSchoolConfirmation();
                model.total_registered_school = _ischoolService.getRegistered();
                //model.total_contestant = _ischoolService.getTotalContestantInRegistered();
                model.total_teams = _ischoolService.GetTeams().Count();

                model.list_registered_contest = _icontestService.GetStaticContest();
                model.statistic_EventViewModel = _ieventService.EventStatic();
                model.statistic_shirtSizeViewModel = _imemberService.statistic_ShirtSizeView();
                return View(model);
            }
            catch (Exception e)
            {
                Log.Error(e);
                return RedirectToAction(ACTION_CONST.Home.INDEX, ACTION_CONST.Home.CONTROLLER);
            }
        }
        [AuthorizationAccept(Roles = "ORGANIZER")]
        public async Task<ActionResult> RegistrationAction(int? schoolId, string type, string note)
        {
            try
            {
                if (schoolId == null)
                {
                    Log.Error("School ID is null");
                    return RedirectToAction(ACTION_CONST.Home.INDEX, ACTION_CONST.Home.CONTROLLER);
                }
                await _ischoolService.processSchool((int)schoolId, type, note);
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

        [AuthorizationAccept(Roles = "ORGANIZER")]
        public ActionResult DetailSchool(int? schoolId)
        {
            statistic_index_ViewModel model = new statistic_index_ViewModel();
            if (schoolId == null)
            {
                Log.Error("School ID is null");
                return RedirectToAction(ACTION_CONST.Home.INDEX, ACTION_CONST.Home.CONTROLLER);
            }
            model.detail_school = _ischoolService.findById((int)schoolId);
            foreach (var item in model.detail_school.teams)
            {
                model.detail_school.teams.Where(x => x.team_id == item.team_id).FirstOrDefault().contest = _icontestService.getById(item.contest_id);
            }
            return View(model);
        }

        [AuthorizationAccept(Roles = "ORGANIZER")]
        public ActionResult TeamContest()
        {
            statistic_index_ViewModel model = new statistic_index_ViewModel();
            try
            {
                model.total_contestant = _icontestService.GetStaticContest().IndividualContest.Count;
                model.list_registered_contest = _icontestService.GetStaticContest();
                return View(model);
            }
            catch (Exception e)
            {
                Log.Error(e);
                return RedirectToAction(ACTION_CONST.Home.INDEX, ACTION_CONST.Home.CONTROLLER);
            }
        }

        [AuthorizationAccept(Roles = "ORGANIZER")]
        public FileResult DownloadContestStatistic()
        {
            string fname = "contest_member_form.xlsx";
            string _path = Path.Combine(Server.MapPath("/App_Data/"), fname);
            FileInfo file = new FileInfo(_path);
            if (file.Exists)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage(_path))
                {
                    registered_contest_ViewModel data = _icontestService.GetStaticContest();

                    ExcelWorksheet overView = package.Workbook.Worksheets["OVERVIEW"];
                    int overViewRow = 4;
                    foreach (var item in data.IndividualContest.Keys)
                    {
                        overView.Cells[overViewRow, 2].Value = item.contest_name;
                        overView.Cells[overViewRow, 3].Value = item.code;
                        overView.Cells[overViewRow, 4].Value = "Individual";
                        ExcelWorksheet contestSheet = package.Workbook.Worksheets.Copy("TEMP", item.contest_name + "(" + item.code + ")");

                        contestSheet.Cells[1, 3].Value = item.contest_name + "(" + item.code + ")";
                        contestSheet.Cells[2, 3].Value = "Individual";
                        int row = 5;
                        overView.Cells[overViewRow, 5].FillNumber(data.IndividualContest[item].Count);
                        overViewRow++;
                        foreach (var member in data.IndividualContest[item])
                        {
                            contestSheet.Cells[row, 2].Value = member.app_user.full_name;
                            contestSheet.Cells[row, 3].Value = member.email;
                            contestSheet.Cells[row, 4].Value = member.phone_number;
                            contestSheet.Cells[row, 5].Value = member.dob;
                            contestSheet.Cells[row, 6].Value = member.icpc_id;
                            row++;
                        }
                    }

                    //delete individual contest
                    package.Workbook.Worksheets.Delete(1);

                    foreach (var item in data.TeamContest.Keys)
                    {
                        ExcelWorksheet contestSheet = package.Workbook.Worksheets.Copy("TEAM-TEMP", item.contest_name + "(" + item.code + ")");
                        overView.Cells[overViewRow, 2].Value = item.contest_name;
                        overView.Cells[overViewRow, 3].Value = item.code;
                        overView.Cells[overViewRow, 4].Value = "Team";
                        overView.Cells[overViewRow, 5].FillNumber(data.TeamContest[item].Count);
                        overViewRow++;
                        contestSheet.Cells[1, 3].Value = item.contest_name + "(" + item.code + ")";
                        contestSheet.Cells[2, 3].Value = "Team";
                        int row = 5;
                        foreach (var team in data.TeamContest[item])
                        {
                            contestSheet.Cells[row, 1].Value = team.team_name;
                            foreach (var member in team.team_member)
                            {
                                contestSheet.Cells[row, 2].Value = member.member.app_user.full_name;
                                contestSheet.Cells[row, 3].Value = member.member.email;
                                contestSheet.Cells[row, 4].Value = member.member.phone_number;
                                contestSheet.Cells[row, 5].FillDateTime(member.member.dob);
                                contestSheet.Cells[row, 6].Value = member.member.icpc_id;
                                row++;
                            }
                        }
                    }
                    //delete team contest
                    package.Workbook.Worksheets.Delete(1);
                    return File(package.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, fname);
                }
            }
            else
            {
                Response.Write("This file does not exist.");
            }
            return null;
        }


        [AuthorizationAccept(Roles = "ORGANIZER")]
        public FileResult DownloadSchool()
        {
            string fname = "contest_member_form.xlsx";
            string _path = Path.Combine(Server.MapPath("/App_Data/"), fname);
            FileInfo file = new FileInfo(_path);
            if (file.Exists)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage(_path))
                {
                    registered_contest_ViewModel data = _icontestService.GetStaticContest();

                    ExcelWorksheet overView = package.Workbook.Worksheets["OVERVIEW"];
                    int overViewRow = 4;
                    foreach (var item in data.IndividualContest.Keys)
                    {
                        overView.Cells[overViewRow, 2].Value = item.contest_name;
                        overView.Cells[overViewRow, 3].Value = item.code;
                        overView.Cells[overViewRow, 4].Value = "Individual";
                        ExcelWorksheet contestSheet = package.Workbook.Worksheets.Copy("TEMP", item.contest_name + "(" + item.code + ")");

                        contestSheet.Cells[1, 3].Value = item.contest_name + "(" + item.code + ")";
                        contestSheet.Cells[2, 3].Value = "Individual";
                        int row = 5;
                        overView.Cells[overViewRow, 5].FillNumber(data.IndividualContest[item].Count);
                        overViewRow++;
                        foreach (var member in data.IndividualContest[item])
                        {
                            contestSheet.Cells[row, 2].Value = member.app_user.full_name;
                            contestSheet.Cells[row, 3].Value = member.email;
                            contestSheet.Cells[row, 4].Value = member.phone_number;
                            contestSheet.Cells[row, 5].Value = member.dob;
                            contestSheet.Cells[row, 6].Value = member.icpc_id;
                            row++;
                        }
                    }

                    //delete individual contest
                    package.Workbook.Worksheets.Delete(1);

                    foreach (var item in data.TeamContest.Keys)
                    {
                        ExcelWorksheet contestSheet = package.Workbook.Worksheets.Copy("TEAM-TEMP", item.contest_name + "(" + item.code + ")");
                        overView.Cells[overViewRow, 2].Value = item.contest_name;
                        overView.Cells[overViewRow, 3].Value = item.code;
                        overView.Cells[overViewRow, 4].Value = "Team";
                        overView.Cells[overViewRow, 5].FillNumber(data.TeamContest[item].Count);
                        overViewRow++;
                        contestSheet.Cells[1, 3].Value = item.contest_name + "(" + item.code + ")";
                        contestSheet.Cells[2, 3].Value = "Team";
                        int row = 5;
                        foreach (var team in data.TeamContest[item])
                        {
                            contestSheet.Cells[row, 1].Value = team.team_name;
                            foreach (var member in team.team_member)
                            {
                                contestSheet.Cells[row, 2].Value = member.member.app_user.full_name;
                                contestSheet.Cells[row, 3].Value = member.member.email;
                                contestSheet.Cells[row, 4].Value = member.member.phone_number;
                                contestSheet.Cells[row, 5].FillDateTime(member.member.dob);
                                contestSheet.Cells[row, 6].Value = member.member.icpc_id;
                                row++;
                            }
                        }
                    }
                    //delete team contest
                    package.Workbook.Worksheets.Delete(1);
                    return File(package.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, fname);
                }
            }
            else
            {
                Response.Write("This file does not exist.");
            }
            return null;
        }

        [AuthorizationAccept(Roles = "ORGANIZER")]
        public ActionResult RegisteredTeam()
        {
            statistic_index_ViewModel model = new statistic_index_ViewModel();
            try
            {
                model.total_teams = _ischoolService.GetTeams().Count();
                model.list_registered_team = _ischoolService.GetTeams();
                return View(model);
            }
            catch (Exception e)
            {
                Log.Error(e);
                return RedirectToAction(ACTION_CONST.Home.INDEX, ACTION_CONST.Home.CONTROLLER);
            }
        }

        [AuthorizationAccept(Roles = "ORGANIZER")]
        public ActionResult RegisteredSchool()
        {
            statistic_index_ViewModel model = new statistic_index_ViewModel();
            try
            {
                model.total_registered_school = _ischoolService.getRegistered();
                model.list_registered_school = _ischoolService.listRegisteredSchool();
                return View(model);
            }
            catch (Exception e)
            {
                Log.Error(e);
                return RedirectToAction(ACTION_CONST.Home.INDEX, ACTION_CONST.Home.CONTROLLER);
            }
        }

        [AuthorizationAccept(Roles = "ORGANIZER")]
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