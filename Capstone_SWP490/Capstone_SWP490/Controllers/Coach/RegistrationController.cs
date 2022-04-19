using Capstone_SWP490.Helper;
using Capstone_SWP490.Models.school_memberViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Capstone_SWP490.Constant.Enums;
using Capstone_SWP490.Constant.Const;
using Capstone_SWP490.Models.registrationViewModel;
using Capstone_SWP490.Models.app_userViewModel;
using Resources;
using Capstone_SWP490.Sercurity;

namespace Capstone_SWP490.Controllers.Coach
{
    public class RegistrationController : Controller
    {
        public static string SYSTEM_ERROR = "SYSTEM ERROR";
        private static readonly ILog Log = LogManager.GetLogger(typeof(RegistrationController));
        private readonly interfaces.IschoolService _ischoolService = new services.schoolService();
        private readonly interfaces.IteamService _iteamService = new services.teamService();
        private readonly interfaces.ImemberService _imemberService = new services.memberService();
        private readonly interfaces.Iapp_userService _iapp_UserService = new services.app_userService();
        private readonly interfaces.Iteam_memberService _iteam_memberService = new services.teamMemberService();
        private readonly interfaces.Icontest_memberService _icontest_memberService = new services.contest_memberService();
        private readonly RegistrationHelper registrationHelper = new RegistrationHelper();
        private readonly interfaces.IcontestService _icontestService = new services.contestService();

        // GET: Registration
        [AuthorizationAccept(Roles = "COACH,GUEST")]
        public ActionResult Index()
        {
            try
            {
                app_userViewModel logined = (app_userViewModel)Session[SESSION_CONST.Global.LOGIN_SESSION];
                if (logined == null)
                {
                    return RedirectToAction(ACTION_CONST.Registration.GUIDE, ACTION_CONST.Registration.CONTROLLER);
                }
                List<school> schools = _ischoolService.findByCoachId(logined.user_id);
                IndexViewModel model = new IndexViewModel(schools).build();
                List<import_error_ViewModel> result = (List<import_error_ViewModel>)Session[SESSION_CONST.Registration.INSERT_RESULT];
                Session.Remove(SESSION_CONST.Registration.INSERT_RESULT);
                //case insert data to database then check result in session
                if (result != null && result.Count > 0)
                {
                    model.insert_result = result;
                }
                return View(model);
            }
            catch (Exception e)
            {
                Log.Error(e);
                return RedirectToAction(ACTION_CONST.Home.INDEX, ACTION_CONST.Home.CONTROLLER);
            }
        }

        [AllowAnonymous]
        public ActionResult Guide()
        {
            return View();
        }
        [AuthorizationAccept(Roles = "COACH")]
        public ActionResult MemberDetail(int? id, int? teamId)
        {
            try
            {
                import_resultViewModel data = (import_resultViewModel)HttpContext.Session[SESSION_CONST.Registration.SCHOOL_SESSION];
                if (data == null)
                {
                    return RedirectToAction(ACTION_CONST.Registration.RESULT, ACTION_CONST.Registration.CONTROLLER);
                }
                if (id == null || teamId == null)
                {
                    return RedirectToAction(ACTION_CONST.Registration.RESULT, ACTION_CONST.Registration.CONTROLLER, new { team = 0 });
                }
                team team = data.School.teams.Where(x => x.team_id == (int)teamId).FirstOrDefault();
                team_member teamMember = registrationHelper.getTeamMember((int)id, team);
                if (teamMember != null)
                {
                    member_detail_ViewModel model = new member_detail_ViewModel().buildFromTeamMember(teamMember);
                    List<contest_member> contestMember = teamMember.member.contest_member.ToList();
                    List<member_contest_ViewModel> listContestModel = _icontestService.getContestMemberModel(contestMember);
                    model.contest_Members = listContestModel;
                    return View(model);
                }
                return RedirectToAction(ACTION_CONST.Registration.TEAM_DETAIL, ACTION_CONST.Home.CONTROLLER);
            }
            catch (Exception e)
            {
                Log.Error(e);
                return RedirectToAction(ACTION_CONST.Home.INDEX, ACTION_CONST.Home.CONTROLLER);
            }
        }

        [AuthorizationAccept(Roles = "COACH")]
        [HttpPost]
        public ActionResult SaveChange()
        {
            import_resultViewModel data = (import_resultViewModel)HttpContext.Session[SESSION_CONST.Registration.SCHOOL_SESSION];
            if (data == null)
            {
                return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER);
            }
            _ischoolService.update(data.School);
            foreach (var item in data.School.teams)
            {
                _iteamService.update(item);
                foreach (var member in item.team_member)
                {
                    _imemberService.update(member.member, member.member.member_id);

                }
            }
            return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER);
        }

        [AuthorizationAccept(Roles = "COACH")]
        public ActionResult SaveChangeTeam(string teamName, int teamId, int contestId)
        {
            try
            {
                import_resultViewModel data = (import_resultViewModel)HttpContext.Session[SESSION_CONST.Registration.SCHOOL_SESSION];
                if (data == null)
                {
                    return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER);
                }
                if (!StringUtils.isNullOrEmpty(teamName))
                {
                    data.School.teams.Where(x => x.team_id == teamId).FirstOrDefault().team_name = teamName;
                    Session.Add(SESSION_CONST.Registration.SCHOOL_SESSION, data);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            return RedirectToAction(ACTION_CONST.Registration.RESULT, ACTION_CONST.Registration.CONTROLLER);
        }

        [AuthorizationAccept(Roles = "COACH")]
        public ActionResult TeamDetail(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(ACTION_CONST.Registration.RESULT, ACTION_CONST.Registration.CONTROLLER);
            }
            import_resultViewModel data = (import_resultViewModel)HttpContext.Session[SESSION_CONST.Registration.SCHOOL_SESSION];
            if (data == null)
            {
                return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER);
            }
            data.setDisplayTeam((int)id);
            return View(data);
        }
        public ActionResult DownloadSample()
        {
            string fname = "registration_form.xlsx";
            string _path = Path.Combine(Server.MapPath("/App_Data/"), fname);
            FileInfo file = new FileInfo(_path);
            if (file.Exists)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    List<contest> individualContest = _icontestService.getIndividualContest();
                    ExcelWorksheet sheets = package.Workbook.Worksheets[3];
                    int startCol = 12;
                    int startRow = 2;
                    foreach (var item in individualContest)
                    {
                        sheets.Cells[startRow, startCol++].Value = item.contest_name;
                    }
                    package.Save();
                }
                FileInfo afterEdit = new FileInfo(_path);
                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=" + fname);
                Response.AddHeader("Content-Type", "application/Excel");
                Response.ContentType = "application/vnd.xls";
                Response.AddHeader("Content-Length", afterEdit.Length.ToString());
                Response.WriteFile(afterEdit.FullName);
                Response.End();
            }
            else
            {
                Response.Write("This file does not exist.");
            }
            return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER);
        }


        [AuthorizationAccept(Roles = "COACH")]
        public async Task<ActionResult> InsertMember()
        {
            List<import_error_ViewModel> result = new List<import_error_ViewModel>();
            app_userViewModel logined = (app_userViewModel)Session[SESSION_CONST.Global.LOGIN_SESSION];
            import_error_ViewModel error;
            school inserted_school = null;
            List<team> listTeam;
            List<contest_member> contestMemberList;

            try
            {

                import_resultViewModel data = (import_resultViewModel)HttpContext.Session[SESSION_CONST.Registration.SCHOOL_SESSION];
                Session.Remove(SESSION_CONST.Registration.SCHOOL_SESSION);

                if (data == null)
                {
                    return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER, new { error = false });
                }

                listTeam = data.School.teams.ToList();

                //insert school part
                inserted_school = registrationHelper.cleanSchool(data.School);
                inserted_school.coach_id = logined.user_id;
                //set school is in use if first insert
                inserted_school.active = 1;
                inserted_school.enabled = true;
                try
                {
                    inserted_school = await _ischoolService.insert(inserted_school);
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
                    error = new import_error_ViewModel();
                    error.objectName = data.School.school_name + "(" + data.School.institution_name + ")";
                    error.parentObject = "ROOT";
                    error.occur_position = "SCHOOL";
                    error.msg = "The process has been stopped becase of " + msg + ", please try again !";
                    result.Add(error);
                    Session.Add(SESSION_CONST.Registration.INSERT_RESULT, result);
                    return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER, new { error = true });

                }

                member coach = data.Coach;
                app_user coachUser = coach.app_user;
                coachUser.full_name = coach.first_name + " " + coach.middle_name + " " + coach.last_name;
                //update coach part
                await _iapp_UserService.update(coachUser);
                member storedCoach = _imemberService.getByEmail(coach.app_user.email);

                storedCoach.first_name = coach.first_name;
                storedCoach.middle_name = coach.middle_name;
                storedCoach.last_name = coach.last_name;
                if (!StringUtils.isNullOrEmpty(coach.phone_number))
                {
                    storedCoach.phone_number = coach.phone_number;
                }
                //update coach to member table
                await _imemberService.update(storedCoach, storedCoach.member_id);

                //insert vice coach part
                if (data.ViceCoach != null)
                {
                    member viceCoach = registrationHelper.cleanMember(data.ViceCoach);
                    app_user viceCoachUser = await registrationHelper.createAppUserForMember(viceCoach, storedCoach.user_id);
                    viceCoach.enabled = true;
                    viceCoach.user_id = viceCoachUser.user_id;
                    try
                    {
                        viceCoach = await _imemberService.insert(viceCoach);
                        //update vice coach account
                        viceCoachUser.active = (viceCoachUser.active || true);
                        await _iapp_UserService.update(viceCoachUser);

                        team_member coachTeam = storedCoach.team_member.FirstOrDefault();
                        if (coachTeam != null)
                        {
                            //insert vice coach member team
                            team_member viceCoachMember = new team_member();
                            viceCoachMember.member_id = viceCoach.member_id;
                            viceCoachMember.team_id = coachTeam.team_id;
                            await _iteam_memberService.insert(viceCoachMember);
                        }
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
                        error = new import_error_ViewModel();
                        error.objectName = data.ViceCoach.first_name + " " + data.ViceCoach.middle_name + " " + data.ViceCoach.last_name + "(VICE COACH)";
                        error.parentObject = "ROOT";
                        error.occur_position = "VICE COACH";
                        error.msg = "The process has been stopped becase of " + msg + ", please try again !";
                        result.Add(error);
                        Session.Add(SESSION_CONST.Registration.INSERT_RESULT, result);
                        await _ischoolService.deleteAsync(inserted_school);
                        await _imemberService.deleteAsync(coach);
                        return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER, new { error = true });
                    }
                }


                //insert team part
                team insertedTeam = null;
                int countInsertedTeam = 0;
                foreach (team item in listTeam)
                {
                    //insert team
                    insertedTeam = new team();
                    insertedTeam.school_id = inserted_school.school_id;
                    insertedTeam.team_name = item.team_name;
                    insertedTeam.type = APP_CONST.TEAM_ROLE.NORMAL_TEAM;
                    insertedTeam.team_member = null;
                    insertedTeam.contest_id = item.contest_id;
                    insertedTeam.enabled = item.enabled;
                    try
                    {
                        insertedTeam = await insertTeamAsync(insertedTeam);
                        countInsertedTeam += 1;
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
                        error = new import_error_ViewModel();
                        error.objectName = item.team_name;
                        error.parentObject = "school";
                        error.occur_position = "";
                        error.msg = "Insert Team fail, Reason: " + msg;
                        result.Add(error);
                        continue;
                    }

                    //insert member's team part
                    member insertMember = null;
                    foreach (team_member i in item.team_member)
                    {
                        contestMemberList = i.member.contest_member.ToList();
                        insertMember = registrationHelper.cleanMember(i.member);
                        app_user memberUser = await registrationHelper.createAppUserForMember(insertMember, storedCoach.user_id);
                        insertMember.user_id = memberUser.user_id;
                        insertMember.enabled = i.member.enabled;
                        try
                        {
                            insertMember.team_member = null;
                            insertMember.contest_member = null;
                            insertMember = await _imemberService.insert(insertMember);
                        }
                        catch (Exception e)
                        {
                            string msg = "";
                            if (e is MemberException)
                            {
                                msg = ((MemberException)e).message;
                            }
                            else
                            {
                                msg = e.Message;
                            }
                            error = new import_error_ViewModel();
                            error.objectName = i.member.first_name + " " + i.member.middle_name + " " + i.member.last_name;
                            error.msg = "Insert fail, Reason: " + msg;
                            result.Add(error);
                            continue;
                        }

                        //update login user for member
                        bool isSendMail = (memberUser.active || true);
                        memberUser.active = (memberUser.active || true);
                        string insertAppUserErrMsg = SYSTEM_ERROR;
                        try
                        {
                            await _iapp_UserService.update(memberUser);
                        }

                        catch (Exception e)
                        {
                            if (e is UserException)
                            {
                                UserException ue = (UserException)e;
                                insertAppUserErrMsg = ue.message;
                            }
                            memberUser = null;
                            Log.Error(e.Message);
                        }
                        if (memberUser == null)
                        {
                            error = new import_error_ViewModel();
                            error.objectName = insertMember.first_name + " " + insertMember.middle_name + " " + insertMember.last_name;
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
                        if (insertTeamMember == null)
                        {
                            error = new import_error_ViewModel();
                            error.objectName = insertMember.first_name + " " + insertMember.middle_name + " " + insertMember.last_name;
                            error.msg = "Insert fail, Reason: " + insertAppUserErrMsg;
                            result.Add(error);
                            _ = _imemberService.deleteAsync(insertMember);
                            _ = _iapp_UserService.delete(memberUser);
                            continue;
                        }

                        //insert contest member part
                        contest_member insertContestMember;
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
                                error = new import_error_ViewModel();
                                error.objectName = insertMember.first_name + " " + insertMember.middle_name + " " + insertMember.last_name;
                                error.msg = "Insert member's contest(" + contestMember.contest.contest_name + ") fail";
                                result.Add(error);

                            }
                        }
                        try
                        {
                            new MailHelper().sendMailToInsertedUser(memberUser);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e.Message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                error = new import_error_ViewModel();
                error.objectName = "SCHOOL";
                error.msg = "INSERT FAIL, SYSTEM ERROR";
                result.Add(error);
                inserted_school.active = 1;
                _ischoolService.disable(inserted_school);
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


        [AuthorizationAccept(Roles = "COACH")]
        [HttpPost]
        public ActionResult MemberDetail(member_detail_ViewModel model, List<string> contest_members)
        {
            import_resultViewModel data = (import_resultViewModel)HttpContext.Session[SESSION_CONST.Registration.SCHOOL_SESSION];
            if (data == null)
            {
                return RedirectToAction(ACTION_CONST.Registration.RESULT, ACTION_CONST.Registration.CONTROLLER);
            }
            try
            {

                team_member teamMember = data.School.teams.Where(x => x.team_id == model.team_id).FirstOrDefault().
                     team_member.Where(y => y.member.member_id == model.member_id).FirstOrDefault();
                string errorMsg = "";
                if (StringUtils.isNullOrEmpty(model.first_name))
                {
                    errorMsg = "First Name cannot be empty";
                }
                else if (StringUtils.isNullOrEmpty(model.middle_name))
                {
                    errorMsg = "Middle Name cannot be empty";
                }
                else if (StringUtils.isNullOrEmpty(model.last_name))
                {
                    errorMsg = "Last Name cannot be empty";
                }
                if (!errorMsg.Equals(""))
                {
                    @ViewData["Update_ERROR"] = errorMsg;
                    List<member_contest_ViewModel> listContestModel = registrationHelper.createContestViewMode(teamMember.member.contest_member.ToList());
                    model.contest_Members = listContestModel;
                    return View(model);
                }
                teamMember.member = model.buildMember();
                teamMember.member.contest_member.Clear();
                foreach (var contestCode in contest_members)
                {

                    contest contest = _icontestService.getByCode(contestCode);
                    if (contest != null)
                    {
                        contest_member cm = new contest_member();
                        cm.member_id = teamMember.member.member_id;
                        cm.member = teamMember.member;
                        cm.contest_id = contest.contest_id;
                        cm.contest = contest;
                        teamMember.member.contest_member.Add(cm);
                    }
                }

                try
                {
                    registrationHelper.validImportMember(teamMember.member);
                    if (model.is_leader)
                    {
                        data.School.teams.Where(x => x.team_id == model.team_id).FirstOrDefault().
                             team_member.Where(y => y.member.member_role == 3).FirstOrDefault().member.member_role = 4;
                        data.School.teams.Where(x => x.team_id == model.team_id).FirstOrDefault().
                            team_member.Where(y => y.member.member_id == model.member_id).FirstOrDefault().member.member_role = 3;
                    }

                    Session.Add(SESSION_CONST.Registration.SCHOOL_SESSION, data);
                }
                catch (Exception e)
                {
                    @ViewData["Update_ERROR"] = e.Message;
                    return View(model);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            return RedirectToAction(ACTION_CONST.Registration.RESULT, ACTION_CONST.Registration.CONTROLLER, new { team = model.team_id });
        }

        [AuthorizationAccept(Roles = "COACH")]
        [HttpGet]
        public ActionResult Result(string team)
        {
            Session.Remove(SESSION_CONST.Registration.READ_FILE_ERROR);
            Session.Remove(SESSION_CONST.Registration.INSERT_ERROR);
            Session.Remove(SESSION_CONST.Registration.READ_FILE_ERROR);
            import_resultViewModel dataSession = (import_resultViewModel)HttpContext.Session[SESSION_CONST.Registration.SCHOOL_SESSION];
            if (dataSession == null || dataSession.School.teams == null || dataSession.School.teams.Count == 0)
            {
                return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER);
            }
            int teamId = -1;
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

        [AuthorizationAccept(Roles = "COACH")]
        public async Task<ActionResult> history(string school_id)

        {
            import_resultViewModel data = new import_resultViewModel();
            school school = null;
            List<team> teams = null;
            try
            {
                data.Source = "VIEW";
                int schoolId = Int32.Parse(school_id);
                school = _ischoolService.findActiveById(schoolId);
                teams = school.teams.ToList();
                app_userViewModel logined = (app_userViewModel)Session["profile"];
                member storedCoach = _imemberService.GetMemberByUserId(logined.user_id);
                teams = teams.Where(x => !x.type.Trim().Equals(APP_CONST.TEAM_ROLE.COACH_TEAM)).ToList();
                foreach (var item in teams)
                {
                    item.contest = _icontestService.getById(item.contest_id);
                }
                school.teams = teams;
                data.School = school;
                team coachTeam = storedCoach.team_member.FirstOrDefault().team;
                if (coachTeam != null)
                {
                    List<team_member> coachTeamMember = _iteam_memberService.getCoachTeamMember(coachTeam.team_id);
                    team_member coachMember = coachTeamMember.Where(x => x.member.member_role == 1).FirstOrDefault();
                    if (coachMember != null)
                    {
                        data.Coach = coachMember.member;
                    }
                    team_member viceCoachMember = coachTeamMember.Where(x => x.member.member_role == 2).FirstOrDefault();
                    if (viceCoachMember != null)
                    {
                        data.ViceCoach = viceCoachMember.member;
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
            data.School = school;
            data.setDisplayTeam(0);
            Session.Add(SESSION_CONST.Registration.SCHOOL_SESSION, data);
            return View(ACTION_CONST.Registration.RESULT, data);
        }

        [AuthorizationAccept(Roles = "COACH")]
        [HttpPost]
        public async Task<ActionResult> Result(HttpPostedFileBase file)
        {
            import_resultViewModel data = new import_resultViewModel();
            import_error_ViewModel error;
            if (file is null)
            {
                error = new import_error_ViewModel();
                error.objectName = "N/A";
                error.parentObject = APP_CONST.ROOT;
                error.occur_position = "N/A";
                error.msg = Message.MSG013;
                data.error.Add(error);
                data.rootError = true;
                return View(data);
            }

            try
            {
                string _FileName = registrationHelper.createFileName(Path.GetExtension(file.FileName));
                string _path = Path.Combine(Server.MapPath("/App_Data/temp"), _FileName);
                if (!_path.EndsWith(".xlsx"))
                {
                    error = new import_error_ViewModel();
                    error.objectName = "N/A";
                    error.parentObject = APP_CONST.ROOT;
                    error.occur_position = "N/A";
                    error.msg = Message.MSG013;
                    data.error.Add(error);
                    data.rootError = true;
                    return View(data);
                }
                _path = _path.Replace(".xlsx", ".txt");
                file.SaveAs(_path);
                data = await importAsync(_path);
                if (data.School != null)
                {
                    data.setDisplayTeam(0);
                }
                data.Source = "IMPORT";
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

        [AuthorizationAccept(Roles = "COACH")]
        public ActionResult RemoveMember(int team_id, int member_id)
        {
            import_resultViewModel data = (import_resultViewModel)HttpContext.Session[SESSION_CONST.Registration.SCHOOL_SESSION];
            if (data == null)
            {
                return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER);
            }
            data.School.teams.Where(x => x.team_id == team_id).FirstOrDefault().team_member.Where(x => x.member.member_id == member_id).FirstOrDefault().member.enabled = false;
            Session.Add(SESSION_CONST.Registration.SCHOOL_SESSION, data);
            return RedirectToAction(ACTION_CONST.Registration.RESULT, ACTION_CONST.Registration.CONTROLLER, new { team = team_id });
        }

        [AuthorizationAccept(Roles = "COACH")]
        public ActionResult RemoveTeam(int id)
        {
            import_resultViewModel data = (import_resultViewModel)HttpContext.Session[SESSION_CONST.Registration.SCHOOL_SESSION];
            if (data == null)
            {
                return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER);
            }
            team team = data.School.teams.Where(x => x.team_id == id).FirstOrDefault();
            if (team != null && data.School.teams.Count >= 2)
            {
                data.School.teams.Where(x => x.team_id == id).FirstOrDefault().enabled = false;
            }
            Session.Add(SESSION_CONST.Registration.SCHOOL_SESSION, data);
            return RedirectToAction(ACTION_CONST.Registration.RESULT, ACTION_CONST.Registration.CONTROLLER, new { team = 0 });
        }

        [AuthorizationAccept(Roles = "COACH")]
        public async Task<ActionResult> AcceptSchool(int id)
        {

            app_userViewModel logined = (app_userViewModel)Session["profile"];
            try
            {
                await _ischoolService.useSchool(id, logined.user_id);
                ViewData[Message.MESSGAE_RESULT] = Message.MSG029;
            }
            catch (Exception e)
            {
                ViewData[Message.ERROR] = e.Message;
            }
            return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER);
        }

        [AuthorizationAccept(Roles = "COACH")]
        public async Task<ActionResult> RemoveSchool(int id)
        {
            app_userViewModel logined = (app_userViewModel)Session["profile"];
            school school = _ischoolService.findActiveById(id);
            if (school == null)
            {
                return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER);
            }
            if (school.active != 3)
            {
                school.enabled = false;
            }
            //get app user belong to acitive school
            school schoolT = _ischoolService.getInConfirmation(logined.user_id);
            List<app_user> activeAppUserSchoolList = new List<app_user>();
            List<team> teamList = schoolT.teams.ToList();
            foreach (var item in teamList)
            {
                List<team_member> teamMembetList = item.team_member.ToList();
                foreach (var tm in teamMembetList)
                {
                    activeAppUserSchoolList.Add(tm.member.app_user);
                }
            }
            //get app user belong to wish disable school
            List<app_user> disableList = new List<app_user>();
            List<team> teamListDisable = school.teams.ToList();
            foreach (var item in teamListDisable)
            {
                List<team_member> teamMemberList = item.team_member.ToList();
                foreach (var tm in teamMemberList)
                {
                    disableList.Add(tm.member.app_user);
                }
            }
            //disable app user if it is not exist in active school
            foreach (var appUser in disableList)
            {
                if (!activeAppUserSchoolList.Exists(x => x.user_name.Equals(appUser.user_name)))
                {
                    appUser.active = false;
                    await _iapp_UserService.update(appUser);
                }
            }
            _ischoolService.disable(school);
            return RedirectToAction(ACTION_CONST.Registration.INDEX, ACTION_CONST.Registration.CONTROLLER);
        }

        public async Task<import_resultViewModel> importAsync(string filePath)
        {
            import_resultViewModel result = new import_resultViewModel();
            member coach = new member();
            app_userViewModel logined = (app_userViewModel)Session[SESSION_CONST.Global.LOGIN_SESSION];
            coach.app_user = _iapp_UserService.getByUserName(logined.user_name);
            result.Coach = coach;
            import_error_ViewModel error;
            FileInfo existingFile = new FileInfo(filePath);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                using (ExcelPackage package = new ExcelPackage(existingFile))
                {
                    List<ExcelWorksheet> sheets = package.Workbook.Worksheets.ToList();
                    string sheetNotExist = registrationHelper.checkExistSheets(sheets);
                    if (!sheetNotExist.Equals(""))
                    {
                        error = new import_error_ViewModel();
                        error.objectName = "N/A";
                        error.parentObject = APP_CONST.ROOT;
                        error.occur_position = "N/A";
                        error.msg = string.Format(Message.MSG015, sheetNotExist);
                        error.type = 1;
                        result.error.Add(error);
                        result.rootError = true;
                        return result;

                    }
                    //read school
                    try
                    {
                        iImport.SchoolImport schoolImport = new iImport.SchoolImport();
                        ExcelWorksheet schoolSheet = registrationHelper.getSheetByName(sheets, iImport.SchoolImport.sheetName);
                        result = registrationHelper.readSchoolSheet(result, schoolSheet, schoolImport);
                        }
                    catch (Exception e)
                    {
                        //read school information error then stop
                        string msg = Message.MSG005;
                        msg = msg.Replace("#SHEET_NAME#", iImport.SchoolImport.sheetName);
                        if (e is SchoolException)
                        {
                            SchoolException se = (SchoolException)e;
                            msg = se.message;
                        }
                        Log.Error(e.Message);
                        error = new import_error_ViewModel();
                        error.objectName = "N/A";
                        error.parentObject = APP_CONST.ROOT;
                        error.occur_position = "N/A";
                        error.msg = msg;
                        error.type = 1;
                        result.error.Add(error);
                        result.rootError = true;
                        return result;
                    }
                    //read team
                    iImport.TeamImport teamImport = new iImport.TeamImport();
                    ExcelWorksheet teamSheet = registrationHelper.getSheetByName(sheets, iImport.TeamImport.sheetName);
                    result = registrationHelper.readTeamSheet(result, teamSheet, teamImport);
                    if (result.School.teams == null || result.School.teams.Count == 0)
                    {
                        error = new import_error_ViewModel();
                        error.objectName = "N/A";
                        error.parentObject = APP_CONST.ROOT;
                        error.occur_position = "N/A";
                        error.msg = Message.MSG022;
                        error.type = 1;
                        result.error.Add(error);
                        result.rootError = true;
                        return result;
                    }

                    //read team member
                    iImport.MemberImport memberImport = new iImport.MemberImport();
                    ExcelWorksheet memberSheet = registrationHelper.getSheetByName(sheets, iImport.MemberImport.sheetName);
                    result = registrationHelper.readMemberSheet(result, memberSheet, memberImport);
                    //display team at index 0 first
                    result.setDisplayTeam(0);
                }
            }
            catch (Exception e)
            {
                error = new import_error_ViewModel();
                error.objectName = "TEAM";
                error.parentObject = "MEMBER";
                error.occur_position = "UNKOWN";
                error.msg = "IMPORT ERROR, PLEASE INSERT DATA CAREFULLY !";
                result.error.Add(error);
                result.rootError = true;
                Log.Error(e.Message);
            }
            finally
            {
                deleteFileAfterImport(filePath);
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