using Capstone_SWP490.DTO;
using Capstone_SWP490.Helper;
using Capstone_SWP490.Models.school_memberViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using iImport = Capstone_SWP490.Common.ExcelImportPosition;
using schoolImport = Capstone_SWP490.Common.ExcelImportPosition;
using interfaces = Capstone_SWP490.Services.Interfaces;
using services = Capstone_SWP490.Services;
using log4net;
using OfficeOpenXml;
using System.Threading.Tasks;
using System.Net;
using Capstone_SWP490.ExceptionHandler;
using System.Web.Configuration;
using Capstone_SWP490.Common.Enums;
using Capstone_SWP490.Common.Const;
using Capstone_SWP490.Models.registrationViewModel;
using Capstone_SWP490.Models.app_userViewModel;

namespace Capstone_SWP490.Controllers.Coach
{
    public class RegistrationController : Controller
    {
        public static string SYSTEM_ERROR = "SYSTEM ERROR";
        private static readonly ILog Log = LogManager.GetLogger(typeof(RegistrationController));
        private readonly interfaces.IcontestService _icontestService = new services.contestService();
        private readonly interfaces.IschoolService _ischoolService = new services.schoolService();
        private readonly interfaces.IteamService _iteamService = new services.teamService();
        private readonly interfaces.ImemberService _imemberService = new services.memberService();
        private readonly interfaces.Iapp_userService _iapp_UserService = new services.app_userService();
        private readonly interfaces.Iteam_memberService _iteam_memberService = new services.teamMemberService();
        private readonly interfaces.Icontest_memberService _icontest_memberService = new services.contest_memberService();
        private readonly RegistrationHelper registrationHelper = new RegistrationHelper();
        private List<string> memberEmail;

        // GET: Registration
        public ActionResult Index()
        {
            //check user session
            if (HttpContext.Session["username"] != null)
            {
                var u = _iapp_UserService.GetUserByUsername(HttpContext.Session["username"].ToString());
                //if user is COACH or CO-COACH so return Import file excel 
                if (u.user_role.Equals("COACH") || u.user_role.Equals("CO-COACH"))
                {
                    IndexViewModel model = new IndexViewModel();
                    List<school> schools = _ischoolService.findByCoachId(u.user_id);
                    model.school = schools;
                    List<insert_member_result_ViewModel> result = (List<insert_member_result_ViewModel>)Session[SESSION_CONST.Registration.INSERT_RESULT];
                    Session.Remove(SESSION_CONST.Registration.INSERT_RESULT);
                    //case insert data to database then check result in session
                    if (result != null && result.Count > 0)
                    {
                        model.insert_result = result;
                    }
                    return View(model);
                }
            }
            //if user is NOT COACH or CO-COACH so return view Guild 
            return RedirectToAction(ACTION_CONST.Registration.GUIDE, ACTION_CONST.Registration.CONTROLLER);
        }

        public ActionResult Guide()
        {
            return View();
        }
        public ActionResult RegistrationCoach()
        {
            return View();
        }

        public ActionResult MemberDetail(string id, string teamId)
        {
            school_memberViewModel data = (school_memberViewModel)HttpContext.Session[SESSION_CONST.Registration.SCHOOL_SESSION];
            if (data == null)
            {
                return RedirectToAction(ACTION_CONST.Registration.RESULT, ACTION_CONST.Registration.CONTROLLER);
            }
            if (id == null || id.Equals(""))
            {
                return RedirectToAction(ACTION_CONST.Registration.RESULT, ACTION_CONST.Registration.CONTROLLER, new { team = 0 });
            }
            int memberId;
            int teamIdInt;
            try
            {
                memberId = registrationHelper.toInt32(id, -1);
                teamIdInt = registrationHelper.toInt32(teamId, -1);
                team_member teamMember = registrationHelper.getTeamMember(memberId, data.school.teams.Where(x => x.team_id == teamIdInt).First());
                if (teamMember != null)
                {
                    return View(teamMember);
                }
            }
            catch { }
            return View();
        }

        public async Task<ActionResult> InsertMember()
        {
            List<insert_member_result_ViewModel> result = new List<insert_member_result_ViewModel>();
            app_userViewModel logined = (app_userViewModel)Session["profile"];
            try
            {
                school_memberViewModel data = (school_memberViewModel)HttpContext.Session[SESSION_CONST.Registration.SCHOOL_SESSION];
                Session.Remove(SESSION_CONST.Registration.SCHOOL_SESSION);
                List<string> insertedEmail = new List<string>();
                insert_member_result_ViewModel error;
                school inserted_school;
                List<team> listTeam;
                List<contest_member> contestMemberList;


                if (data == null)
                {
                    return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER, new { error = false });
                }

                listTeam = data.school.teams.ToList();

                //insert school part
                inserted_school = registrationHelper.cleanSchool(data.school);
                inserted_school.coach_id = logined.user_id;
                inserted_school.insert_date = DateTime.Now + "";
                inserted_school.update_date = DateTime.Now + "";
                try
                {
                    inserted_school = await insertSchoolAsync(inserted_school);
                }
                catch (Exception e)
                {
                    string msg = "";
                    if (e is SchoolException)
                    {
                        SchoolException se = (SchoolException)e;
                        msg = se.message;
                    }
                    else
                    {
                        msg = e.Message;
                    }
                    error = new insert_member_result_ViewModel();
                    error.objectName = data.school.school_name + "(" + data.school.short_name + ")";
                    error.parentObject = "ROOT";
                    error.occur_position = "SCHOOL";
                    error.msg = "The process has been stopped becase of " + msg + ", please try again !";
                    result.Add(error);
                    Session.Add(SESSION_CONST.Registration.INSERT_RESULT, result);
                    return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER, new { error = true });

                }

                //insert coach part
                member coach = registrationHelper.cleanMember(data.coach);
                coach.member_role = 1;
                coach.user_id = logined.user_id;
                try
                {
                    coach = await insertCoachAsyn(coach, logined.user_id);
                }
                catch (Exception e)
                {
                    string msg = "";
                    if (e is MemberException)
                    {
                        MemberException me = (MemberException)e;
                        msg = me.message;
                    }
                    else
                    {
                        msg = e.Message;
                    }
                    error = new insert_member_result_ViewModel();
                    error.objectName = data.coach.first_name + " " + data.coach.middle_name + " " + data.coach.last_name + "(COACH)";
                    error.parentObject = "ROOT";
                    error.occur_position = "COACH";
                    error.msg = "The process has been stopped becase of " + msg + ", please try again !";
                    result.Add(error);
                    Session.Add("INSERT_RESULT", result);
                    //remove if insert fail
                    await _ischoolService.deleteAsync(inserted_school);
                    return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER, new { error = true });
                }


                //insert vice coach part
                member viceCoach = registrationHelper.cleanMember(data.vice_coach);
                viceCoach.member_role = 2;
                app_user viceCoachUser = registrationHelper.createAppUserFromMember(viceCoach);
                string insertViceCoachAppUserErrMsg = SYSTEM_ERROR;
                try
                {
                    viceCoachUser = await _iapp_UserService.CreateUser(viceCoachUser);
                }

                catch (Exception e)
                {
                    viceCoachUser = null;
                    if (e is UserException)
                    {
                        UserException ue = (UserException)e;
                        insertViceCoachAppUserErrMsg = ue.message;
                    }
                }
                //insert vice coach user fail
                if (viceCoachUser == null)
                {
                    error = new insert_member_result_ViewModel();
                    error.objectName = "vice coach";
                    error.parentObject = "team";
                    error.occur_position = "";
                    error.msg = "Insert fail, Reason: " + insertViceCoachAppUserErrMsg;
                    result.Add(error);
                    await _ischoolService.deleteAsync(inserted_school);
                    await _imemberService.deleteAsync(coach);
                    return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER, new { error = true });
                }

                viceCoach.user_id = viceCoachUser.user_id;
                try
                {
                    viceCoach = await insertViceCoachAsync(viceCoach, logined.user_id);
                }
                catch (Exception e)
                {
                    string msg = "";
                    if (e is MemberException)
                    {
                        MemberException me = (MemberException)e;
                        msg = me.message;
                    }
                    else
                    {
                        msg = e.Message;
                    }
                    error = new insert_member_result_ViewModel();
                    error.objectName = data.vice_coach.first_name + " " + data.vice_coach.middle_name + " " + data.vice_coach.last_name + "(VICE COACH)";
                    error.parentObject = "ROOT";
                    error.occur_position = "VICE COACH";
                    error.msg = "The process has been stopped becase of " + msg + ", please try again !";
                    result.Add(error);
                    Session.Add(SESSION_CONST.Registration.INSERT_RESULT, result);
                    await _ischoolService.deleteAsync(inserted_school);
                    await _imemberService.deleteAsync(coach);
                    return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER, new { error = true });
                }

                //insert coach team
                team coachTeam = new team();
                coachTeam.school_id = inserted_school.school_id;
                coachTeam.team_name = "COACH_TEAM";
                coachTeam.type = "COACH";
                try
                {
                    coachTeam = await insertTeamAsync(coachTeam);
                }
                catch (Exception e)
                {
                    error = new insert_member_result_ViewModel();
                    error.objectName = "Coach/ Vice Coach";
                    error.parentObject = "ROOT";
                    error.occur_position = "COACH";
                    error.msg = "Xử lý thất bại do không thể tạo dữ liêu của cán bộ nhà trường, vui lòng thử lại!";
                    result.Add(error);
                    Session.Add("INSERT_RESULT", result);
                    //remove if insert fail
                    await _ischoolService.deleteAsync(inserted_school);
                    await _imemberService.deleteAsync(coach);
                    await _imemberService.deleteAsync(viceCoach);
                    return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER, new { error = true });
                }


                //insert coach member team
                team_member coachMember = new team_member();
                coachMember.member_id = coach.member_id;
                coachMember.team_id = coachTeam.team_id;
                await _iteam_memberService.insert(coachMember);

                //insert vice coach member team
                team_member viceCoachMember = new team_member();
                viceCoachMember.member_id = viceCoach.member_id;
                viceCoachMember.team_id = coachTeam.team_id;
                await _iteam_memberService.insert(viceCoachMember);

                //insert team part
                team insertedTeam = null;
                int countInsertedTeam = 0;
                foreach (team item in listTeam)
                {
                    //insert team
                    insertedTeam = new team();
                    insertedTeam.school_id = inserted_school.school_id;
                    insertedTeam.team_name = item.team_name;
                    insertedTeam.school = inserted_school;
                    insertedTeam.type = "NORMAL";
                    insertedTeam.team_member = null;
                    try
                    {
                        insertedTeam = await insertTeamAsync(insertedTeam);
                    }
                    catch (Exception e)
                    {
                        string msg = "";
                        if (e is TeamException)
                        {
                            msg = ((TeamException)e).message;
                        }
                        else
                        {
                            msg = e.Message;
                        }
                        error = new insert_member_result_ViewModel();
                        error.objectName = item.team_name;
                        error.parentObject = "school";
                        error.occur_position = "";
                        error.msg = "Insert Team fail, Reason: " + msg;
                        result.Add(error);
                        continue;
                    }

                    //insert member's team part
                    Dictionary<string, string> paramMember = new Dictionary<string, string>();
                    member insertMember = null;
                    int insertedMemberCount = 0;
                    foreach (team_member i in item.team_member)
                    {
                        contestMemberList = i.member.contest_member.ToList();
                        insertMember = registrationHelper.cleanMember(i.member);
                        insertMember.user_id = logined.user_id;
                        try
                        {
                            insertMember.team_member = null;
                            insertMember.contest_member = null;
                            insertMember = await _imemberService.insert(insertMember, logined.user_id);
                        }
                        catch (Exception e)
                        {
                            string msg = "";
                            if(e is MemberException)
                            {
                                msg = ((MemberException)e).message;
                            }
                            else
                            {
                                msg = e.Message;
                            }
                            error = new insert_member_result_ViewModel();
                            error.objectName = i.member.first_name + " " + i.member.middle_name + " " + i.member.last_name;
                            error.parentObject = "team";
                            error.occur_position = "";
                            error.msg = "Insert fail, Reason: " + msg;
                            result.Add(error);
                            continue;
                        }

                        //create login user for member
                        app_user insertUser = registrationHelper.createAppUserFromMember(insertMember);
                        string insertAppUserErrMsg = SYSTEM_ERROR;
                        try
                        {
                            insertUser = await _iapp_UserService.CreateUser(insertUser);
                        }

                        catch (Exception e)
                        {
                            if (e is UserException)
                            {
                                UserException ue = (UserException)e;
                                insertAppUserErrMsg = ue.message;
                            }
                            insertUser = null;
                            Log.Error(e.Message);
                        }
                        if (insertUser == null)
                        {
                            error = new insert_member_result_ViewModel();
                            error.objectName = insertMember.first_name + " " + insertMember.middle_name + " " + insertMember.last_name;
                            error.parentObject = "team";
                            error.occur_position = "";
                            error.msg = "Insert fail, Reason: " + insertAppUserErrMsg;
                            result.Add(error);
                            _ = _imemberService.deleteAsync(insertMember);
                            continue;
                        }

                        //insert into team member
                        team_member insertTeamMember = new team_member();
                        insertTeamMember.member_id = insertMember.member_id;
                        insertTeamMember.team_id = insertedTeam.team_id;
                        try
                        {
                            insertTeamMember.member = null;
                            insertMember.team_member = null;
                            insertTeamMember = await _iteam_memberService.insert(insertTeamMember);
                        }
                        catch
                        {
                            insertTeamMember = null;
                            await _imemberService.deleteAsync(insertMember);
                            //delete app user
                            continue;
                        }
                        if(insertTeamMember == null)
                        {
                            error = new insert_member_result_ViewModel();
                            error.objectName = insertMember.first_name + " " + insertMember.middle_name + " " + insertMember.last_name;
                            error.parentObject = "team";
                            error.occur_position = "";
                            error.msg = "Insert fail, Reason: " + insertAppUserErrMsg;
                            result.Add(error);
                            _ = _imemberService.deleteAsync(insertMember);
                            _ = _iapp_UserService.delete(insertUser);
                            continue;
                        }

                        insertMember.user_id = insertUser.user_id;
                        await _imemberService.update(insertMember, insertMember.member_id);

                        //insert contest member part
                        contest_member insertContestMember;
                        int insertedContestMemberCount = 0;
                        foreach (contest_member contestMember in contestMemberList)
                        {
                            insertContestMember = new contest_member();
                            insertContestMember.contest_id = contestMember.contest.contest_id;
                            insertContestMember.member_id = insertMember.member_id;

                            try
                            {
                                insertContestMember.member = null;
                                insertContestMember = await _icontest_memberService.insert(insertContestMember);
                            }
                            catch (Exception e)
                            {
                                insertContestMember = null;
                                if (e is MemberException)
                                {
                                    MemberException me = (MemberException)e;
                                }

                            }

                            if (insertContestMember == null)
                            {
                                error = new insert_member_result_ViewModel();
                                error.objectName = insertMember.first_name + " " + insertMember.middle_name + " " + insertMember.last_name;
                                error.parentObject = "Contest";
                                error.occur_position = "";
                                error.msg = "Insert member's contest(" + contestMember.contest.constest_name + ") fail";
                                result.Add(error);

                            }
                            else
                            {
                                insertedContestMemberCount += 1;
                            }
                        }
                        if (!registrationHelper.checkExistEmail(insertedEmail, insertUser.user_name))
                        {
                            try
                            {
                                new MailHelper().sendMailToInsertedUser(insertUser);
                                insertedEmail.Add(insertUser.user_name);
                            }
                            catch (Exception e)
                            {
                                Log.Error(e.Message);
                            }
                        }
                        if (insertedContestMemberCount != 0)
                        {
                           
                            insertedMemberCount += 1;
                        }
                    }
                    if (insertedMemberCount == 0)
                    {
                        //delete member
                        error = new insert_member_result_ViewModel();
                        error.objectName = inserted_school.school_name + "(" + inserted_school.short_name + ")";
                        error.parentObject = "ROOT";
                        error.occur_position = "VICE COACH";
                        error.msg = "Team '" + insertedTeam.team_name + " không được tạo do không có member nào được thêm vào!";
                        await _iteamService.delete(insertedTeam);
                        result.Add(error);
                    }
                    else
                    {
                        countInsertedTeam += 1;
                    }
                }
                if (countInsertedTeam == 0)
                {
                    await _ischoolService.deleteAsync(inserted_school);
                    //delete member
                    error = new insert_member_result_ViewModel();
                    error.objectName = inserted_school.school_name + "(" + inserted_school.short_name + ")";
                    error.parentObject = "ROOT";
                    error.occur_position = "VICE COACH";
                    error.msg = "Dữ liệu của trường tạo thất bại do không có team nào được tạo !";
                    await _iteamService.delete(insertedTeam);
                    result.Add(error);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            Session.Add(SESSION_CONST.Registration.INSERT_RESULT, result);
            return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER);
        }
      
        private async Task<team> insertTeamAsync(team team)
        {
            team.school = null;
            try
            {
                team.team_member = null;
                team = await _iteamService.insert(team);
            }
            catch (Exception e)
            {
                if (e is TeamException)
                {
                    throw e;
                }
                team = null;
            }
            if (team == null)
            {
                throw new Exception(SYSTEM_ERROR);
            }
            return team;
        }
        private async Task<member> insertViceCoachAsync(member viceCoach, int importer)
        {
            try
            {
                viceCoach = await _imemberService.insert(viceCoach, importer);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                if (e is MemberException)
                {
                    throw e;
                }
                viceCoach = null;
            }
            if (viceCoach == null)
            {
                throw new Exception(SYSTEM_ERROR);
            }
            return viceCoach;
        }
        private async Task<member> insertCoachAsyn(member coach, int importer)
        {
            try
            {
                coach = await _imemberService.insert(coach , importer);

            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                if (e is MemberException)
                {
                    throw e;
                }
                coach = null;
            }
            if (coach == null)
            {
                throw new Exception(SYSTEM_ERROR);
            }
            return coach;
        }

        private async Task<school> insertSchoolAsync(school school)
        {
            try
            {
                school = await _ischoolService.insert(school);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                if (e is SchoolException)
                {
                    throw e;
                }
                school = null;
            }
            if (school == null)
            {
                throw new Exception(SYSTEM_ERROR);
            }
            return school;
        }

        [HttpPost]
        public ActionResult UpdateMember(int teamId, int id, string firstName, string middleName,
            string lastName, string dob, string email, string phone, int icpc, int year,
            string gender, string award)
        {
            school_memberViewModel data = (school_memberViewModel)HttpContext.Session[SESSION_CONST.Registration.SCHOOL_SESSION];
            if (data == null)
            {
                return RedirectToAction(ACTION_CONST.Registration.RESULT, ACTION_CONST.Registration.CONTROLLER);
            }
            try
            {

                member member = data.school.teams.Where(x => x.team_id == teamId).First().
                     team_member.Where(y => y.member.member_id == id).First().member;
                member.first_name = firstName;
                member.middle_name = middleName;
                member.last_name = lastName;
                member.dob = registrationHelper.toDateTime(dob);
                //member.dateStr = member.dob.Year + "-" + member.dob.Month + "-" + member.dob.Day;
                member.email = email;
                member.phone_number = phone;
                member.year = year;
                member.gender = registrationHelper.toShort(gender);
                member.award = award;
                member.icpc_id = icpc;

                data.school.teams.Where(x => x.team_id == teamId).First().
                     team_member.Where(y => y.member.member_id == id).First().member = member;
                Session.Add(SESSION_CONST.Registration.SCHOOL_SESSION, data);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            return RedirectToAction(ACTION_CONST.Registration.RESULT, ACTION_CONST.Registration.CONTROLLER, new { team = teamId });
        }

        [HttpGet]
        public ActionResult Result(string team)
        {
            Session.Remove(SESSION_CONST.Registration.READ_FILE_ERROR);
            Session.Remove(SESSION_CONST.Registration.INSERT_ERROR);
            Session.Remove(SESSION_CONST.Registration.READ_FILE_ERROR);
            school_memberViewModel dataSession = (school_memberViewModel)HttpContext.Session[SESSION_CONST.Registration.SCHOOL_SESSION];
            if (dataSession == null || dataSession.school.teams == null || dataSession.school.teams.Count == 0)
            {
                return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER);
            }
            int teamId = 0;
            dataSession.error = null;
            try
            {
                teamId = Int32.Parse(team);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            dataSession.setDisplayTeam(teamId);
            return View(dataSession);
        }

        public ActionResult history(string school_id)
        {
            school_memberViewModel data = new school_memberViewModel();
            school school = null;
            List<team> teams = null;
            try
            {
                int schoolId = Int32.Parse(school_id);
                school = _ischoolService.findById(schoolId);
                teams = school.teams.ToList();
                team coachTeam = teams.Where(x => x.type.Trim().Equals("COACH")).FirstOrDefault();
                teams = teams.Where(x => !x.type.Trim().Equals("COACH")).ToList();
                school.teams = teams;
                data.school = school;
                if (coachTeam != null)
                {
                    List<team_member> coachTeamMember = _iteam_memberService.getCoachTeamMember(coachTeam.team_id);
                    team_member coachMember = coachTeamMember.Where(x => x.member.member_role == 1).FirstOrDefault();
                    if (coachMember != null)
                    {
                        data.coach = coachMember.member;
                    }
                    team_member viceCoachMember = coachTeamMember.Where(x => x.member.member_role == 2).FirstOrDefault();
                    if (viceCoachMember != null)
                    {
                        data.vice_coach = viceCoachMember.member;
                    }
                }

            }
            catch
            {

            }
            if (school == null)
            {
                return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER);
            }
            data.school = school;
            data.setDisplayTeam(-1);
            Session.Add(SESSION_CONST.Registration.SCHOOL_SESSION, data);
            return View(ACTION_CONST.Registration.RESULT, data);
        }

        [HttpPost]
        public ActionResult Result(HttpPostedFileBase file)
        {

            if (file is null)
            {
                ViewData[SESSION_CONST.Registration.READ_FILE_ERROR] = "Please select file end with .xlsx";
                return View();
            }

            try
            {
                string _FileName = registrationHelper.createFileName(Path.GetExtension(file.FileName));
                string _path = Path.Combine(Server.MapPath("/App_Data/temp"), _FileName);
                if (!_path.EndsWith(".xlsx"))
                {
                    ViewData[SESSION_CONST.Registration.READ_FILE_ERROR] = "Please select file end with .xlsx";
                    return View();
                }
                _path = _path.Replace(".xlsx", ".txt");
                file.SaveAs(_path);
                school_memberViewModel data = import(_path);
                Session.Add(SESSION_CONST.Registration.SCHOOL_SESSION, data);
                return View(data);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            finally
            {
                if (file != null)
                    file.InputStream.Close();
            }
            return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER);
        }

        public ActionResult RemoveMember(int id, int teamId)
        {
            school_memberViewModel data = (school_memberViewModel)HttpContext.Session[SESSION_CONST.Registration.SCHOOL_SESSION];
            if (data == null)
            {
                return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER);
            }
            team_member member = data.school.teams.Where(x => x.team_id == teamId).FirstOrDefault().team_member.Where(x => x.member.member_id == id).FirstOrDefault();
            data.school.teams.Where(x => x.team_id == teamId).FirstOrDefault().team_member.Remove(member);
            Session.Add(SESSION_CONST.Registration.SCHOOL_SESSION, data);
            return RedirectToAction(ACTION_CONST.Registration.RESULT, ACTION_CONST.Registration.CONTROLLER, new { team = teamId });
        }
        public ActionResult RemoveSchool(int id)
        {
            school school = _ischoolService.findById(id);
            if (school == null)
            {
                return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER);
            }
            school.enabled = false;
            _ischoolService.disable(school);
            return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER);
        }
        public school_memberViewModel import(string filePath)
        {
            school_memberViewModel result = new school_memberViewModel();
            FileInfo existingFile = new FileInfo(filePath);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                using (ExcelPackage package = new ExcelPackage(existingFile))
                {
                    //read school
                    schoolImport.SchoolImport schoolImport = new iImport.SchoolImport();
                    ExcelWorksheet schoolSheet = package.Workbook.Worksheets[schoolImport.sheetPosition];
                    string[,] data = readExcelSheetCustom(schoolSheet, schoolImport);
                    school school = new school();
                    try
                    {
                        school = registrationHelper.getSchool(data);
                    }
                    catch (Exception e)
                    {
                        //read school information error then stop
                        string msg = "UNKOWN";
                        if (e is SchoolException)
                        {
                            SchoolException se = (SchoolException)e;
                            msg = se.message;
                        }
                        Log.Error(e.Message);
                        //read school information error then stop
                        insert_member_result_ViewModel error = new insert_member_result_ViewModel();
                        error.objectName = "School";
                        error.parentObject = "ROOT";
                        error.occur_position = "SCHOOL";
                        error.msg = msg;
                        result.error.Add(error);
                    }

                    result.school = school;
                    //read coach
                    result.coach = registrationHelper.getCoach(data);
                    //read vice coach
                    result.vice_coach = registrationHelper.getViceCoach(data);
                    //read teams
                    memberEmail = new List<string>();
                    iImport.TeamImport teamImport = new iImport.TeamImport();
                    ExcelWorksheet teamSheet = package.Workbook.Worksheets[teamImport.sheetPosition];
                    result = readTeam(result, teamSheet, teamImport);
                    //read team member
                    iImport.MemberImport memberImport = new iImport.MemberImport();
                    ExcelWorksheet memberSheet = package.Workbook.Worksheets[memberImport.sheetPosition];
                    result = readMember(result, memberSheet, memberImport);
                    //display team at index 0 first
                    result.setDisplayTeam(0);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            finally
            {
                deleteFileAfterImport(filePath);
            }
            return result;
        }

        private string[,] readExcelSheetCustom(ExcelWorksheet sheet, iImport.IExcelPosition positions)
        {
            int[,] index = positions.GetPosition();
            int dataLen = index.Length / 2;
            string[,] data = new string[dataLen, 1];
            try
            {
                for (int i = 0; i < dataLen; i++)
                {
                    ExcelRange cell = sheet.Cells[index[i, 0], index[i, 1]];
                    var val = cell.Value;

                    data[i, 0] = (val == null) ? "" : val + "";
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            return data;
        }

        private school_memberViewModel readTeam(school_memberViewModel result, ExcelWorksheet teamSheet, iImport.IExcelPosition excelImport)
        {

            List<team> teamList = new List<team>();
            insert_member_result_ViewModel error;
            contest contest = new contest();
            team team;
            member leader;
            contest_member contestMember;
            team_member teamMember;
            int rowCount = teamSheet.Dimension.End.Row;     //get row count
            int teamId = 0;
            int col;

            for (int row = excelImport.getStartAtRow(); row <= rowCount; row++)
            {
                try
                {
                    col = excelImport.getStartAtCol();
                    var cellVal = teamSheet.Cells[row, col].Value + "";
                    //check for contest column value
                    if (!cellVal.Equals(""))
                    {
                        contest = _icontestService.getByCode(cellVal.ToUpper());
                        if (contest == null)
                        {
                            error = new insert_member_result_ViewModel();
                            error.objectName = "CONTEST";
                            error.parentObject = "TEAM";
                            error.occur_position = "Row = " + row;
                            error.msg = "the Contest '" + cellVal + "' is not existed";
                            result.error.Add(error);
                        }
                    }
                    //skip because contest not exist
                    if (contest == null)
                    {
                        continue;
                    }

                    //read team
                    string teamName = teamSheet.Cells[row, ++col].Value + "";
                    string leaderFullName = teamSheet.Cells[row, ++col].Value + "";
                    string leaderEmail = teamSheet.Cells[row, ++col].Value + "";
                    string leaderPhone = teamSheet.Cells[row, ++col].Value + "";
                    //skip because team leader email or team name is not valid
                    if (StringUtils.isNullOrEmpty(teamName) || !registrationHelper.IsValidEmail(leaderEmail))
                    {
                        error = new insert_member_result_ViewModel();
                        error.objectName = "MEMBER-LEADER";
                        error.parentObject = "TEAM";
                        error.occur_position = "Row = " + row;
                        error.msg = "Team leader email and Team Name cannot be blank";
                        result.error.Add(error);
                        continue;
                    }

                    if (registrationHelper.checkExistEmail(memberEmail, leaderEmail))
                    {
                        error = new insert_member_result_ViewModel();
                        error.objectName = "MEMBER-LEADER";
                        error.parentObject = "TEAM";
                        error.occur_position = "Row = " + row;
                        error.msg = "Team leader email '" + leaderEmail + "' used";
                        result.error.Add(error);
                        continue;
                    }

                    team = registrationHelper.getTeamByTeamName(teamList, teamName);
                    //skip because team existed
                    if (!StringUtils.isNullOrEmpty(team.team_name))
                    {
                        error = new insert_member_result_ViewModel();
                        error.objectName = "TEAM";
                        error.parentObject = "TEAM";
                        error.occur_position = "Row = " + row;
                        error.msg = "The Team '" + teamName + "'existed";
                        result.error.Add(error);
                        continue;
                    }

                    team.team_id = teamId++;
                    team.school = result.school;
                    team.school_id = result.school.school_id;
                    team.team_name = teamName;
                    //read leader
                    leader = new member();
                    leader.member_role = 3;
                    leader.member_id = 0;
                    leader.email = leaderEmail;
                    leader.first_name = registrationHelper.extractFirstName(leaderFullName);
                    leader.middle_name = registrationHelper.extractMiddleName(leaderFullName);
                    leader.last_name = registrationHelper.extractLastName(leaderFullName);
                    leader.phone_number = leaderPhone;
                    leader.app_user = registrationHelper.createAppUserFromMember(leader);
                    leader.dob = new DateTime();
                    leader.gender = -1;
                    leader.year = -1;

                    contestMember = new contest_member();
                    contestMember.contest = contest;
                    contestMember.member = leader;
                    contestMember.member_id = leader.user_id;
                    contestMember.contest_id = contest.contest_id;
                    leader.contest_member.Add(contestMember);

                    //add leader to member of team
                    teamMember = new team_member();
                    teamMember.member = leader;
                    teamMember.team = team;

                    team.team_member.Add(teamMember);
                    if (team != null)
                    {
                        teamList.Add(team);
                        memberEmail.Add(leaderEmail);
                    }
                }
                catch (Exception e)
                {
                    error = new insert_member_result_ViewModel();
                    error.objectName = "TEAM";
                    error.parentObject = "TEAM";
                    error.occur_position = "Row = " + row;
                    error.msg = "Unkown ERROR";
                    result.error.Add(error);
                    Log.Error(e.Message);
                }
            }
            result.school.teams = teamList;
            return result;
        }

        private school_memberViewModel readMember(school_memberViewModel result, ExcelWorksheet memberSheet, iImport.IExcelPosition memberImport)
        {
            insert_member_result_ViewModel error;
            int rowCount = memberSheet.Dimension.End.Row;     //get row count
            int memberId = 1;
            int col;
            team team = null;
            member member;
            team_member teamMember;
            List<team> teamList = new List<team>();
            List<team_member> teamMemberList = new List<team_member>();
            for (int row = memberImport.getStartAtRow(); row <= rowCount; row++)
            {
                string errMsg = "";
                try
                {
                    col = memberImport.getStartAtCol();
                    var cellVal = memberSheet.Cells[row, col].Value + "";
                    //check for team name column value
                    if (!cellVal.Equals(""))
                    {
                        //read team from sheet team
                        team = result.school.teams.Where(x => x.team_name.ToUpper() == cellVal.ToUpper()).ToList().FirstOrDefault();
                        if (team == null)
                        {
                            error = new insert_member_result_ViewModel();
                            error.objectName = "MEMBER_NORMAL";
                            error.parentObject = "MEMBER";
                            error.occur_position = "Row = " + row;
                            error.msg = "- the Team '" + cellVal + "' not existed, please check at sheet 'TEAM'";
                            result.error.Add(error);
                        }
                    }
                    //skip
                    if (team == null)
                    {
                        continue;
                    }

                    member = new member();
                    member.member_id = memberId++;
                    //normal member
                    member.member_role = 4;
                    string memberName = memberSheet.Cells[row, ++col].Value + "";
                    member.first_name = registrationHelper.extractFirstName(memberName);
                    member.middle_name = registrationHelper.extractMiddleName(memberName);
                    member.last_name = registrationHelper.extractLastName(memberName);
                    string dtString = memberSheet.Cells[row, ++col].Value + "";
                    DateTime memberDob = registrationHelper.toDateTime(dtString);
                    if (!registrationHelper.isOver15YearOld(memberDob))
                    {
                        errMsg += "- Member age must greater than or equal 15 year old ";
                    }
                    member.dob = registrationHelper.toDateTime(dtString);
                    string email = memberSheet.Cells[row, ++col].Value + "";

                    if (!registrationHelper.IsValidEmail(email))
                    {
                        member.email = "";
                    }
                    else
                    {
                        member.email = email;
                    }
                    //skip if both email and name is blank
                    if (StringUtils.isNullOrEmpty(memberName) && StringUtils.isNullOrEmpty(member.email))
                    {
                        errMsg += "\n- Member name and email cannot both blank ";
                    }
                    else
                    {
                        member.phone_number = memberSheet.Cells[row, ++col].Value + "";
                        member.icpc_id = registrationHelper.toInt32(memberSheet.Cells[row, ++col].Value + "", -1);
                        member.gender = registrationHelper.getGender(memberSheet.Cells[row, ++col].Value + "");
                        member.year = registrationHelper.toInt32(memberSheet.Cells[row, ++col].Value + "", 0);
                        member.award = memberSheet.Cells[row, ++col].Value + "";


                        team_member leader = team.team_member.Where(x => x.member.member_role == 3).FirstOrDefault();
                        contest_member contestMemmber = new contest_member();
                        contestMemmber.contest = leader.member.contest_member.FirstOrDefault().contest;
                        contestMemmber.member = member;
                        member.contest_member.Add(contestMemmber);
                        //because of leader added before therefore will be skipped
                        if (!registrationHelper.isTeamLeader(member, leader.member))
                        {
                            teamMember = new team_member();
                            teamMember.member = member;
                            teamMember.team = team;
                            //add to team
                            result.school.teams.Where(x => x.team_id == team.team_id).FirstOrDefault().team_member.Add(teamMember);
                        }
                        else
                        {
                            //update leader data
                            member.member_role = 3;
                            leader.member = member;
                            //leader.member.dateStr = member.dateStr;
                            team.team_member.Where(x => x.member.member_role == 3).FirstOrDefault().member = leader.member;
                        }
                    }
                }
                catch (Exception e)
                {
                    errMsg += "\n- UNKOW ERROR";
                    Log.Error(e.Message);
                }

                if (!errMsg.Equals(""))
                {
                    error = new insert_member_result_ViewModel();
                    error.objectName = "MEMBER_NORMAL";
                    error.parentObject = "MEMBER";
                    error.occur_position = "Row = " + row;
                    error.msg = errMsg;
                    result.error.Add(error);
                }
            }

            return result;
        }


        private void deleteFileAfterImport(string filePath)
        {
            try
            {
                if (System.IO.File.Exists(Path.Combine(filePath)))
                {
                    System.IO.File.Delete(Path.Combine(filePath));
                }
            }
            catch (IOException ioExp)
            {
                Log.Error(ioExp.Message);
                throw ioExp;
            }
        }

    }
}