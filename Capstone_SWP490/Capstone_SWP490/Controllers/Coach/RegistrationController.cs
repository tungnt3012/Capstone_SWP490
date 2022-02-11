﻿using Capstone_SWP490.DTO;
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

namespace Capstone_SWP490.Controllers.Coach
{
    public class RegistrationController : Controller
    {
        private static string REGISTRATION_ERROR_SESSION = "REGISTRATION_ERROR";
        private static string INSERT_ERROR = "IMPORT_ERROR";
        private static string READ_FILE_ERROR = "READ_FILE_ERROR";
        private static string SCHOOL_SESSION = "SCHOOL";
        private static string SYSTEM_ERROR = "SYSTEM ERROR";
        private static readonly ILog Log = LogManager.GetLogger(typeof(RegistrationController));
        private readonly interfaces.IcontestService _icontestService = new services.contestService();
        private readonly interfaces.IschoolService _ischoolService = new services.schoolService();
        private readonly interfaces.IteamService _iteamService = new services.teamService();
        private readonly interfaces.ImemberService _imemberService = new services.memberService();
        private readonly interfaces.Iapp_userService _iapp_UserService = new services.app_userService();
        private readonly interfaces.Iteam_memberService _iteam_memberService = new services.teamMemberService();
        private readonly interfaces.Icontest_memberService _icontest_memberService = new services.contest_memberService();
        private readonly RegistrationHelper registrationHelper = new RegistrationHelper();
        private List<String> memberEmail;
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
                    List<insert_member_result_ViewModel> result = (List<insert_member_result_ViewModel>)Session["INSERT_RESULT"];
                    if (result != null && result.Count > 0)
                    {
                        return View(result);
                    }
                    return View();
                }
            }
            //if user is NOT COACH or CO-COACH so return view Guild 
            return RedirectToAction("Guide", "Registration");
        }

        public ActionResult Guide()
        {
            return View();
        }

        public ActionResult MemberDetail(string id, string teamId)
        {
            school_memberViewModel data = (school_memberViewModel)HttpContext.Session[SCHOOL_SESSION];
            if (data == null)
            {
                Log.Info("Hi I am log4net Info Level");
                return RedirectToAction("Result", "Registration");
            }
            if (id == null || id.Equals(""))
            {
                return RedirectToAction("Result", "Registration", new { team = 0 });
            }
            int memberId;
            int teamIdInt;
            try
            {
                memberId = Int32.Parse(id);
                teamIdInt = Int32.Parse(teamId);
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
            try
            {
                school_memberViewModel data = (school_memberViewModel)HttpContext.Session[SCHOOL_SESSION];
                List<string> insertedEmail = new List<string>();
                insert_member_result_ViewModel error;
                school inserted_school;
                List<team> listTeam;
                List<contest_member> contestMemberList;


                if (data == null)
                {
                    return RedirectToAction("Index", "Registration", new { error = false });
                }

                listTeam = data.school.teams.ToList();

                //insert school part
                inserted_school = registrationHelper.cleanSchool(data.school);
                bool isInsertSchoolError = false;
                string insertSchoolErrMsg = SYSTEM_ERROR;
                try
                {
                    inserted_school = await _ischoolService.insert(inserted_school);
                    if (inserted_school == null)
                    {
                        isInsertSchoolError = true;
                    }
                }
                catch (Exception e)
                {
                    if (e is SchoolException)
                    {
                        SchoolException se = (SchoolException)e;
                        insertSchoolErrMsg = se.message;
                    }
                    isInsertSchoolError = true;

                }

                if (isInsertSchoolError)
                {
                    error = new insert_member_result_ViewModel();
                    error.objectName = data.school.school_name + "(" + data.school.short_name + ")";
                    error.parentObject = "ROOT";
                    error.occur_position = "SCHOOL";
                    error.msg = "The process has been stopped becase of " + insertSchoolErrMsg + ", please try again !";
                    result.Add(error);
                    Session.Add("INSERT_RESULT", result);
                    return RedirectToAction("Index", "Registration", new { error = true });

                }

                //insert team part
                team insertedTeam = null;
                foreach (team item in listTeam)
                {
                    insertedTeam = new team();
                    insertedTeam.school_id = inserted_school.school_id;
                    insertedTeam.team_name = item.team_name;
                    bool isInsertTeamError = false;
                    string insertTeamErrMsg = SYSTEM_ERROR;
                    try
                    {
                        insertedTeam = await _iteamService.insert(insertedTeam);
                        if (insertedTeam == null)
                        {
                            isInsertTeamError = true;
                        }
                    }
                    catch (Exception e)
                    {

                        if (e is TeamException)
                        {
                            TeamException te = (TeamException)e;
                            insertTeamErrMsg = te.message;
                        }
                        isInsertTeamError = true;
                    }

                    //skip if insert team error
                    if (isInsertTeamError)
                    {
                        error = new insert_member_result_ViewModel();
                        error.objectName = item.team_name;
                        error.parentObject = "school";
                        error.occur_position = "";
                        error.msg = "Insert Team fail, Reason: " + insertTeamErrMsg;
                        result.Add(error);
                        continue;
                    }

                    //insert member's team part
                    Dictionary<string, string> paramMember = new Dictionary<string, string>();
                    member insertMember;
                    foreach (team_member i in item.team_member)
                    {
                        contestMemberList = i.member.contest_member.ToList();
                        bool isInsertMemeberError = false;
                        string insertMemberErrMsg = SYSTEM_ERROR;
                        insertMember = registrationHelper.cleanMember(i.member);

                        try
                        {
                            insertMember = await _imemberService.insert(insertMember);
                            if (insertMember == null)
                            {
                                isInsertMemeberError = true;
                            }
                        }
                        catch (Exception e)
                        {
                            isInsertMemeberError = true;
                            if (e is MemberException)
                            {
                                MemberException me = (MemberException)e;
                                insertMemberErrMsg = me.message;
                            }

                        }

                        if (isInsertMemeberError)
                        {
                            error = new insert_member_result_ViewModel();
                            error.objectName = i.member.first_name + " " + i.member.middle_name + " " + i.member.last_name;
                            error.parentObject = "team";
                            error.occur_position = "";
                            error.msg = "Insert fail, Reason: " + insertMemberErrMsg;
                            result.Add(error);
                            continue;
                        }

                        //insert into team member
                        team_member insertTeamMember = new team_member();
                        insertTeamMember.member_id = insertMember.member_id;
                        insertTeamMember.team_id = insertedTeam.team_id;
                        try
                        {
                            insertTeamMember = await _iteam_memberService.insert(insertTeamMember);
                        }
                        catch
                        {

                        }

                        //create login user for member
                        app_user insertUser = registrationHelper.createAppUserFromMember(insertMember);
                        bool isInsertAppUserError = false;
                        string insertAppUserErrMsg = SYSTEM_ERROR;
                        try
                        {
                            insertUser = await _iapp_UserService.CreateUser(insertUser);
                            if (insertUser == null)
                            {
                                isInsertAppUserError = true;
                            }
                        }

                        catch (Exception e)
                        {
                            if (e is UserException)
                            {
                                UserException ue = (UserException)e;
                                insertAppUserErrMsg = ue.message;
                            }
                        }
                        if (isInsertAppUserError)
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

                        insertMember.user_id = insertUser.user_id;
                        await _imemberService.update(insertMember, insertMember.member_id);

                        //insert contest member part
                        contest_member insertContestMember;
                        foreach (contest_member contestMember in contestMemberList)
                        {
                            bool isInsertContestMemeberError = false;
                            string insertContestMemberErrMsg = SYSTEM_ERROR;
                            insertContestMember = new contest_member();
                            insertContestMember.contest_id = contestMember.contest.contest_id;
                            insertContestMember.member_id = insertMember.member_id;

                            try
                            {
                                insertContestMember = await _icontest_memberService.insert(insertContestMember);
                                if (insertContestMember == null)
                                {
                                    isInsertContestMemeberError = true;
                                }
                            }
                            catch (Exception e)
                            {
                                isInsertContestMemeberError = true;
                                if (e is MemberException)
                                {
                                    MemberException me = (MemberException)e;
                                    insertContestMemberErrMsg = me.message;
                                }

                            }

                            if (isInsertContestMemeberError)
                            {
                                error = new insert_member_result_ViewModel();
                                error.objectName = insertMember.first_name + " " + insertMember.middle_name + " " + insertMember.last_name;
                                error.parentObject = "Contest";
                                error.occur_position = "";
                                error.msg = "Insert member's contest(" + contestMember.contest.constest_name + ") fail, Reason: " + insertMemberErrMsg;
                                result.Add(error);

                            }
                        }

                        if (!registrationHelper.checkExistEmail(insertedEmail, insertUser.user_name))
                        {
                            try
                            {
                                sendMailToInsertedUser(insertUser);
                                insertedEmail.Add(insertUser.user_name);
                            }
                            catch
                            {

                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {

            }
            Session.Add("INSERT_RESULT", result);
            return RedirectToAction("Index", "Registration");
        }

        [HttpPost]
        public ActionResult UpdateMember(int teamId, int id, string firstName, string middleName,
            string lastName, string dob, string email, string phone, int icpc, int year,
            string gender, string award)
        {
            school_memberViewModel data = (school_memberViewModel)HttpContext.Session[SCHOOL_SESSION];
            if (data == null)
            {
                return RedirectToAction("Result", "Registration");
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
                Session.Add(SCHOOL_SESSION, data);
            }
            catch { }
            return RedirectToAction("Result", "Registration", new { team = teamId });
        }

        [HttpGet]
        public ActionResult Result(string team)
        {
            Session.Remove(READ_FILE_ERROR);
            Session.Remove(INSERT_ERROR);
            Session.Remove(READ_FILE_ERROR);
            school_memberViewModel dataSession = (school_memberViewModel)HttpContext.Session[SCHOOL_SESSION];
            if (dataSession == null || dataSession.school.teams == null || dataSession.school.teams.Count == 0)
            {
                return RedirectToAction("Index", "Registration");
            }
            int teamId = 0;
            dataSession.error = null;
            try
            {
                teamId = Int32.Parse(team);
            }
            catch
            { }

            dataSession.setDisplayTeam(teamId);
            return View(dataSession);
        }

        [HttpPost]
        public ActionResult Result(HttpPostedFileBase file)
        {

            if (file is null)
            {
                Log.Error("File imported is null");
                ViewData[READ_FILE_ERROR] = "Please select file end with .xlsx";
                return View();
            }

            try
            {
                string _FileName = registrationHelper.createFileName(Path.GetExtension(file.FileName));
                string _path = Path.Combine(Server.MapPath("/App_Data/temp"), _FileName);
                if (!_path.EndsWith(".xlsx"))
                {
                    ViewData[READ_FILE_ERROR] = "Please select file end with .xlsx";
                    return View();
                }
                _path = _path.Replace(".xlsx", ".txt");
                file.SaveAs(_path);
                school_memberViewModel data = import(_path);
                Session.Add(SCHOOL_SESSION, data);
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
            return RedirectToAction("Index", "Registration");
        }

        public ActionResult RemoveMember(int id, int teamId)
        {
            school_memberViewModel data = (school_memberViewModel)HttpContext.Session[SCHOOL_SESSION];
            if (data == null)
            {
                return RedirectToAction("Index", "Registration");
            }
            team_member member = data.school.teams.Where(x => x.team_id == teamId).FirstOrDefault().team_member.Where(x => x.member.member_id == id).FirstOrDefault();
            data.school.teams.Where(x => x.team_id == teamId).FirstOrDefault().team_member.Remove(member);
            Session.Add(SCHOOL_SESSION, data);
            return RedirectToAction("Result", "Registration", new { team = teamId });
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
                    school school;
                    try
                    {
                        school = registrationHelper.getSchool(data);
                    }
                    catch (Exception e)
                    {
                        //read school information error then stop
                        throw e;
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
                            error.parentObject = "SCHOOL";
                            error.occur_position = "TEAM";
                            error.msg = "the Contest '" + cellVal + "' is not existed At ROW = " + row;
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
                        error.parentObject = "SCHOOL";
                        error.occur_position = "TEAM";
                        error.msg = "Team leader email and Team Name cannot be blank At ROW = " + row;
                        result.error.Add(error);
                        continue;
                    }

                    if (registrationHelper.checkExistEmail(memberEmail, leaderEmail))
                    {
                        error = new insert_member_result_ViewModel();
                        error.objectName = "MEMBER-LEADER";
                        error.parentObject = "SCHOOL";
                        error.occur_position = "TEAM";
                        error.msg = "Team leader email used At ROW = " + row;
                        result.error.Add(error);
                        continue;
                    }

                    team = registrationHelper.getTeamByTeamName(teamList, teamName);
                    //skip because team existed
                    if (!StringUtils.isNullOrEmpty(team.team_name))
                    {
                        error = new insert_member_result_ViewModel();
                        error.objectName = "TEAM";
                        error.parentObject = "SCHOOL";
                        error.occur_position = "TEAM";
                        error.msg = "The Team '" + teamName + "'existed  At ROW = " + row;
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
                    error.parentObject = "SCHOOL";
                    error.occur_position = "Team";
                    error.msg = "Unkown ERROR At Row = " + row;
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
                            error.parentObject = "TEAM";
                            error.occur_position = "MEMBER";
                            error.msg = "the Team '" + cellVal + "' not existed At Row = " + row;
                            result.error.Add(error);
                        }
                    }
                    //skip because team not exist
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
                        error = new insert_member_result_ViewModel();
                        error.objectName = "MEMBER_NORMAL";
                        error.parentObject = "TEAM";
                        error.occur_position = "MEMBER";
                        error.msg = "Member name and email cannot both blank At Row = " + row;
                        result.error.Add(error);
                        continue;
                    }
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
                catch (Exception e)
                {
                    error = new insert_member_result_ViewModel();
                    error.objectName = "MEMBER_NORMAL";
                    error.parentObject = "TEAM";
                    error.occur_position = "MEMBER";
                    error.msg = "Unkown ERROR At Row = " + row;
                    result.error.Add(error);
                    Log.Error(e.Message);
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

        private bool sendMailAsync(EmailModel emailModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var message = new MailMessage();
                    message.To.Add(new MailAddress(emailModel.toEmail));
                    message.Subject = emailModel.title;
                    message.Body = emailModel.body;
                    message.IsBodyHtml = true;
                    using (var smtp = new SmtpClient())
                    {
                        smtp.Send(message);
                        return true;
                    }

                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            return false;
        }
        private void sendMailToInsertedUser(app_user user)
        {
            MailReaderHelper emailReader = new MailReaderHelper();
            string mailContent = emailReader.readEmailCreateAccount();
            EmailModel model = new EmailModel();
            model.toEmail = user.email;
            string hostName = "";
            try
            {
                hostName = WebConfigurationManager.AppSettings["HostName"];
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            string changePswUrl = hostName + "/Login";
            model.body = string.Format(mailContent, user.psw, changePswUrl, changePswUrl);
            model.title = "ICPC Asia-VietNam";
            sendMailAsync(model);
        }
    }
}