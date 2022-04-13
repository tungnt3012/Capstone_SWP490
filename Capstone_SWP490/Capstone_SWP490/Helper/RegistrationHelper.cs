using Capstone_SWP490.ExceptionHandler;
using Capstone_SWP490.Models.school_memberViewModel;
using log4net;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Capstone_SWP490.Common.ExcelImportPosition;
using interfaces = Capstone_SWP490.Services.Interfaces;
using services = Capstone_SWP490.Services;
using System.Threading.Tasks;
using Capstone_SWP490.Constant.Const;
using System.Security.Cryptography;
using Resources;

namespace Capstone_SWP490.Helper
{
    public class RegistrationHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(RegistrationHelper));
        private readonly interfaces.IcontestService _icontestService = new services.contestService();
        private readonly interfaces.Iapp_userService _iapp_UserService = new services.app_userService();
        private readonly interfaces.IteamService _iteamService = new services.teamService();
        private readonly interfaces.IschoolService _ischoolService = new services.schoolService();
        public team_member getTeamByTeamName(List<team_member> teams, string teamName)
        {
            if (teams == null)
                return null;
            return teams.Find(x => x.team.team_name == teamName);
        }
        public bool isTeamLeader(member leader, member compared)
        {
            if (compared == null)
            {
                return false;
            }
            return (leader.email.Equals(compared.email));
        }
        public string createFileName(string fileExtension)
        {
            return DateTime.UtcNow.Ticks + "." + fileExtension;
        }

        public member validImportMember(member member)
        {
            string role = APP_CONST.SCHOOL_ROLE.getRole(member.member_role);
            if (member.first_name.Equals("") && member.middle_name.Equals("") && member.last_name.Equals(""))
            {
                throw new Exception(role + " name cannot be empty !");
            }
            if (member.email.Equals(""))
            {
                throw new Exception(role + " email cannot be empty !");
            }
            if (member.member_role == 3 || member.member_role == 4)
            {
                if (!isOver15YearOld(member.dob))
                {
                    throw new Exception(role + "age must greater or equal 15 year old!");
                }
            }
            return member;
        }
        public List<member_contest_ViewModel> createContestViewMode(List<contest_member> contestMember)
        {
            List<member_contest_ViewModel> listContestModel = new List<member_contest_ViewModel>();
            member_contest_ViewModel contestModel;
            List<contest> individualContest = _icontestService.getIndividualContest();
            foreach (var item in individualContest)
            {
                contestModel = new member_contest_ViewModel();
                contestModel.code = item.code;
                contestModel.name = item.contest_name;
                contest_member joined = contestMember.Where(x => x.contest.max_contestant == -1 && x.contest.code.Equals(item.code)).FirstOrDefault();
                if (joined != null)
                {
                    contestModel.selected = true;
                }
                else
                {
                    contestModel.selected = false;
                }
                    listContestModel.Add(contestModel);
            }
            return listContestModel;
        }
        public import_resultViewModel updateCoach(string[,] data, import_resultViewModel result)
        {
            member loginedCoach = result.coach;
            string full_name = data[4, 0];
            loginedCoach.first_name = extractFirstName(full_name);
            loginedCoach.middle_name = extractMiddleName(full_name);
            loginedCoach.last_name = extractLastName(full_name);
            loginedCoach.phone_number = data[5, 0];
            loginedCoach.email = data[6, 0];
            loginedCoach.member_role = 1;
            try
            {
                loginedCoach = validImportMember(loginedCoach);
                result.coach = loginedCoach;
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<import_resultViewModel> addViceCoach(string[,] data, import_resultViewModel result)
        {
            member viceCoach = new member();
            viceCoach.first_name = extractFirstName(data[7, 0]);
            viceCoach.middle_name = extractMiddleName(data[7, 0]);
            viceCoach.last_name = extractLastName(data[7, 0]);
            viceCoach.phone_number = data[8, 0];
            viceCoach.email = data[9, 0];
            viceCoach.member_role = 2;
            try
            {
                viceCoach = validImportMember(viceCoach);
                app_user viceCoachAppUser = await createAppUserForMember(viceCoach, result.coach.user_id);
                viceCoach.user_id = viceCoachAppUser.user_id;
                viceCoach.app_user = viceCoachAppUser;
                result.vice_coach = viceCoach;
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public school getSchool(string[,] data)
        {
            string name = StringUtils.upperCaseFirstCharacter(data[0, 0]);
            string shortName = data[1, 0].ToUpper();
            string address = data[2, 0];
            if (StringUtils.isNullOrEmpty(name) || StringUtils.isNullOrEmpty(shortName))
            {
                throw new SchoolException("1", "School name or short name cannot be null", null);
            }
            school school = new school();
            school.school_name = name;
            school.institution_name = shortName;
            school.address = address;
            return school;
        }
        public string extractFirstName(string name)
        {
            string[] arr = name.Split(' ');
            if (arr.Length == 0)
            {
                return "";
            }
            return StringUtils.upperCaseFirstCharacter(arr.FirstOrDefault());
        }
        public string extractLastName(string name)
        {
            string[] arr = name.Split(' ');
            if (arr.Length == 0)
            {
                return "";
            }
            return StringUtils.upperCaseFirstCharacter(arr.LastOrDefault());
        }
        public string extractMiddleName(string name)
        {
            string[] arr = name.Split(' ');
            if (arr.Length <= 2)
            {
                return "";
            }
            if (arr.Length == 3) { return StringUtils.upperCaseFirstCharacter(arr[1]); }
            string result = "";
            for (int i = 1; i < arr.Length - 1; i++)
            {
                result += " " + arr[i];
            }
            return StringUtils.upperCaseFirstCharacter(result);
        }
        public school cleanSchool(school school)
        {
            school.teams = null;
            school.insert_date = DateTime.Now + "";
            school.update_date = DateTime.Now + "";
            return school;
        }

        public bool IsValidEmail(string email)
        {
            try
            {
                var trimmedEmail = email.Trim();

                if (trimmedEmail.EndsWith("."))
                {
                    return false;
                }
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
        public short getGender(string text)
        {
            if (text.ToUpper().Equals("NAM") || text.ToUpper().Equals("MALE"))
            {
                return 0;
            }
            else if (text.ToUpper().Equals("NỮ") || text.ToUpper().Equals("FEMALE"))
            {
                return 1;
            }
            return 2;
        }

        public import_resultViewModel readTeamSheet(import_resultViewModel result, ExcelWorksheet teamSheet, TeamImport teamObject)
        {
            //hold all registed team in imported school and other schools
            List<team> registedTeam = _iteamService.findRegistedTeam().ToList();
            List<team> schoolTeam = new List<team>();
            import_error_ViewModel error;
            contest contest = null;
            team team;
            member leader;
            team_member teamMember;
            int rowCount = teamSheet.Dimension.End.Row;     //get row count
            int teamId = 0;

            for (int row = teamObject.getStartAtRow(); row <= rowCount; row++)
            {
                try
                {
                    int col = teamObject.getStartAtCol();
                    var cellVal = teamSheet.Cells[row, col].Value + "";
                    cellVal = cellVal.Trim();
                    //check for contest column value
                    if (!cellVal.Equals(""))
                    {
                        contest = _icontestService.getByCode(cellVal.ToUpper());
                        if (contest == null || contest.max_contestant == -1)
                        {
                            error = new import_error_ViewModel();
                            error.objectName = "CONTEST";
                            error.parentObject = APP_CONST.TEAM;
                            error.occur_position = "Row = " + row;
                            error.msg = string.Format(Message.MSG016, cellVal);
                            error.type = 1;
                            result.error.Add(error);
                            continue;
                        }
                    }
                    //skip because contest not exist in next row
                    if (contest == null)
                    {
                        continue;
                    }

                    //read team
                    string teamName = teamSheet.Cells[row, ++col].Value + "";
                    teamName = teamName.Trim();
                    //team name cannot be black or less than 2 character
                    if (StringUtils.isNullOrEmpty(teamName) || teamName.Length <= 2)
                    {
                        error = new import_error_ViewModel();
                        error.objectName = "MEMBER-LEADER";
                        error.parentObject = APP_CONST.TEAM;
                        error.occur_position = "Row = " + row;
                        error.msg = Message.MSG017;
                        error.type = 1;
                        result.error.Add(error);
                        continue;
                    }

                    team = getTeamByTeamName(registedTeam, teamName);
                    //skip because team existed before or used by others
                    if (!StringUtils.isNullOrEmpty(team.team_name))
                    {
                        error = new import_error_ViewModel();
                        error.objectName = "TEAM";
                        error.parentObject = APP_CONST.TEAM;
                        error.occur_position = "Row = " + row;
                        string msg = Message.MSG018;
                        error.msg = msg.Replace("#TEAM_NAME#", team.team_name);
                        error.type = 1;
                        result.error.Add(error);
                        continue;
                    }

                    string leaderFullName = teamSheet.Cells[row, ++col].Value + "";
                    string leaderEmail = teamSheet.Cells[row, ++col].Value + "";
                    string leaderPhone = teamSheet.Cells[row, ++col].Value + "";
                    //skip because team leader email or team name is not valid
                    if (StringUtils.isNullOrEmpty(leaderFullName) || !IsValidEmail(leaderEmail))
                    {
                        error = new import_error_ViewModel();
                        error.objectName = "MEMBER-LEADER";
                        error.parentObject = APP_CONST.TEAM;
                        error.occur_position = "Row = " + row;
                        error.msg = Message.MSG019;
                        error.type = 1;
                        result.error.Add(error);
                        continue;
                    }

                    team.team_id = teamId++;
                    team.school = result.school;
                    team.school_id = result.school.school_id;
                    team.team_name = teamName;
                    team.contest_id = contest.contest_id;
                    team.contest = contest;
                    team.type = "NORMAL";
                    //read leader
                    leader = new member();
                    leader.member_role = 3;
                    leader.member_id = 0;
                    leader.email = leaderEmail;
                    leader.first_name = extractFirstName(leaderFullName);
                    leader.middle_name = extractMiddleName(leaderFullName);
                    leader.last_name = extractLastName(leaderFullName);
                    leader.phone_number = leaderPhone;
                    leader.dob = new DateTime();
                    leader.gender = -1;
                    leader.year = -1;

                    //add leader to team
                    teamMember = new team_member();
                    teamMember.member = leader;
                    teamMember.team = team;
                    teamMember.team_id = team.team_id;
                    teamMember.member_id = leader.member_id;
                    team.team_member.Add(teamMember);
                    schoolTeam.Add(team);
                    registedTeam.Add(team);
                }
                catch (Exception e)
                {
                    error = new import_error_ViewModel();
                    error.objectName = "TEAM";
                    error.parentObject = APP_CONST.TEAM;
                    error.occur_position = "Row = " + row;
                    error.msg = Message.UNKNOWN_ERROR;
                    error.type = 1;
                    result.error.Add(error);
                    Log.Error(e.Message);
                }
            }
            result.school.teams = schoolTeam;
            return result;
        }

        public import_resultViewModel readSchoolSheet(import_resultViewModel result, ExcelWorksheet schoolSheet, SchoolImport importObject)
        {
            import_error_ViewModel error;
            int col = importObject.getStartAtCol();
            int row = importObject.getStartAtRow();
            school firstRegist = _ischoolService.getFirstRegistSchool(result.coach.app_user.user_id);
            //read school, if any field is empty then use first regist
            string school_name = schoolSheet.Cells[row++, col].Value + "";
            if (StringUtils.isNullOrEmpty(school_name))
            {
                school_name = firstRegist.school_name;
                error = new import_error_ViewModel();
                error.objectName = "COACH";
                error.occur_position = "ROW = " + row;
                error.msg = Message.MSG006;
                error.parentObject = SchoolImport.sheetName;
                error.type = 2;
                result.error.Add(error);
            }

            string insitution_name = schoolSheet.Cells[row++, col].Value + "";
            if (StringUtils.isNullOrEmpty(insitution_name))
            {
                insitution_name = firstRegist.institution_name;
                error = new import_error_ViewModel();
                error.objectName = "COACH";
                error.occur_position = "ROW = " + row;
                error.msg = Message.MSG007;
                error.parentObject = SchoolImport.sheetName;
                error.type = 2;
                result.error.Add(error);
            }
            bool existed = _ischoolService.isExisted(school_name, insitution_name, result.coach.app_user.user_id);
            //in case of school name and institution name inserted by other coach
            if (existed)
            {
                string msg = string.Format(Message.MSG021, school_name, insitution_name);
                throw new SchoolException("1", msg, null);
            }

            string rector_name = schoolSheet.Cells[row++, col].Value + "";
            if (StringUtils.isNullOrEmpty(rector_name))
            {
                error = new import_error_ViewModel();
                error.objectName = "COACH";
                error.occur_position = "ROW = " + row;
                error.msg = Message.MSG008;
                error.parentObject = SchoolImport.sheetName;
                error.type = 2;
                result.error.Add(error);
            }

            string school_phone = schoolSheet.Cells[row++, col].Value + "";
            if (StringUtils.isNullOrEmpty(school_phone))
            {
                school_phone = firstRegist.phone_number;
                error = new import_error_ViewModel();
                error.objectName = "COACH";
                error.occur_position = "ROW = " + row;
                error.msg = Message.MSG009;
                error.parentObject = SchoolImport.sheetName;
                error.type = 2;
                result.error.Add(error);
            }

            string address = schoolSheet.Cells[row++, col].Value + "";
            if (StringUtils.isNullOrEmpty(address))
            {
                school_phone = firstRegist.phone_number;
                error = new import_error_ViewModel();
                error.objectName = "COACH";
                error.occur_position = "ROW = " + row;
                error.msg = Message.MSG010;
                error.parentObject = SchoolImport.sheetName;
                error.type = 2;
                result.error.Add(error);
            }

            string school_website = schoolSheet.Cells[row++, col].Value + "";
            school school = new school();
            school.school_name = school_name;
            school.rector_name = rector_name;
            school.institution_name = insitution_name;
            school.phone_number = school_phone;
            school.website = school_website;
            school.address = address;
            result.school = school;

            int loginedId = result.coach.user_id;
            string coach_name = schoolSheet.Cells[row++, col].Value + "";
            string msgValidateCoach = "";
            if (coach_name.Trim().Equals(""))
            {
                msgValidateCoach += Message.MSG011;
            }

            string coach_email = schoolSheet.Cells[row++, col].Value + "";
            if (!IsValidEmail(coach_email) || !coach_email.Equals(result.coach.email))
            {
                msgValidateCoach += "\n" + Message.MSG012;
            }
            if (!msgValidateCoach.Equals(""))
            {
                error = new import_error_ViewModel();
                error.objectName = "COACH";
                error.occur_position = "ROW = 7 OR 8";
                error.msg = msgValidateCoach.StartsWith("\n") ? msgValidateCoach.Remove(0,1) : msgValidateCoach;
                error.parentObject = SchoolImport.sheetName;
                error.type = 2;
                result.error.Add(error);
            }
            string coach_phone = schoolSheet.Cells[row++, col].Value + "";

            //read coach
            member coach = new member();
            coach.first_name = extractFirstName(coach_name);
            coach.middle_name = extractMiddleName(coach_name);
            coach.last_name = extractLastName(coach_name);
            coach.phone_number = coach_phone;
            coach.user_id = loginedId;
            coach.app_user = result.coach.app_user;
            result.coach = coach;

            //read vice coach
            string vice_coach_name = schoolSheet.Cells[row++, col].Value + "";
            string vice_coach_email = schoolSheet.Cells[row++, col].Value + "";
            string vice_coach_phone = schoolSheet.Cells[row++, col].Value + "";
            //start read vice coach
            if (!vice_coach_name.Equals(""))
            {
                member vice_coach = new member();
                vice_coach.first_name = extractFirstName(vice_coach_name);
                vice_coach.middle_name = extractMiddleName(vice_coach_name);
                vice_coach.last_name = extractLastName(vice_coach_name);
                vice_coach.phone_number = vice_coach_phone;
                vice_coach.email = vice_coach_email;
                //cannot read vice coach because no email recognized
                if (!IsValidEmail(vice_coach_email))
                {
                    error = new import_error_ViewModel();
                    error.objectName = APP_CONST.SCHOOL;
                    error.parentObject = APP_CONST.SCHOOL;
                    error.occur_position = "ROW = " + 10;
                    error.msg = Message.MSG014;
                    error.type = 2;
                    result.error.Add(error);
                }
                else
                {
                    result.vice_coach = vice_coach;
                }
            }

            return result;
        }
        public ExcelWorksheet getSheetByName(List<ExcelWorksheet> sheets, string sheetName)
        {
            if (sheets == null || sheets.Count == 0)
            {
                return null;
            }
            foreach (var sheet in sheets)
            {
                string nameUpper = sheet.Name.Trim().ToUpper();
                if (nameUpper.Equals(sheetName.Trim().ToUpper()))
                {
                    return sheet;
                }
            }
            return null;
        }
        public team_member getTeamMember(int memberId, team team)
        {
            team_member result = team.team_member.Where(x => x.member.member_id == memberId).FirstOrDefault();
            if (result != null)
            {
                return result;
            }
            return null;
        }
        public team getTeamByTeamName(List<team> teams, string teamName)
        {
            foreach (var team in teams)
            {
                if (team.team_name.ToUpper().Equals(teamName.ToUpper()))
                {
                    return team;
                }
            }
            return new team();
        }

        public bool isExistEmail(List<string> emailList, string email)
        {
            return emailList.Where(x => x.Equals(email)).FirstOrDefault() != null;
        }
        public async Task<app_user> createAppUserForMember(member member, int coachId)
        {
            if (member == null)
            {
                return null;
            }
            bool isUsed = _iapp_UserService.isEmailInUse(member.email, coachId);
            if (isUsed)
            {
                throw new Exception("Email '" + member.email + "' already used, please input new email!");
            }

            app_user user = new app_user();
            string rawPassword = CommonHelper.CreatePassword(8);
            string encryptedPassword = CommonHelper.createEncryptedPassWord(rawPassword);
            user.psw = rawPassword;
            user.user_name = member.email;
            user.user_role = APP_CONST.APP_ROLE.getUserRole(member.member_role);
            user.encrypted_psw = encryptedPassword;
            user.full_name = member.first_name + " " + member.middle_name + " " + member.last_name;
            user.email = member.email;
            user.verified = false;
            user.active = true;
            user.insert_date = DateTime.Now + "";
            user.update_date = DateTime.Now + "";
            user = await _iapp_UserService.creatUserForImportMember(user, coachId);
            return user;
        }

        public Dictionary<int, contest> extractIndividualContest(ExcelWorksheet memberSheet, int startAtRow)
        {
            int colCount = memberSheet.Dimension.End.Column;
            //read individual contest
            Dictionary<int, contest> individualContestList = new Dictionary<int, contest>();
            //get all contest from excel sheet team member
            for (int j = 12; j <= colCount; j++)
            {
                var contestName = memberSheet.Cells[startAtRow - 1, j].Value + "";
                if (!StringUtils.isNullOrEmpty(contestName))
                {
                    string code = StringUtils.extractFirstWordCharacter(contestName);
                    contest individual = _icontestService.getByCodeOrName(code, contestName);
                    if (individual != null && !individualContestList.ContainsValue(individual))
                    {
                        individualContestList.Add(j, individual);
                    }
                }

            }
            return individualContestList;
        }
        public import_resultViewModel readMemberSheet(import_resultViewModel result, ExcelWorksheet memberSheet, MemberImport memberImport)
        {
            List<string> memberEmail = new List<string>();
            int coachUserId = result.coach.app_user.user_id;
            import_error_ViewModel error;
            int rowCount = memberSheet.Dimension.End.Row;     //get row count
            int colCount = memberSheet.Dimension.End.Column;
            List<team> teamList = new List<team>();
            int memberId = 1;
            int col;
            team team = null;
            member member = null;
            team_member teamMember;
            Dictionary<int, contest> individualContestList = extractIndividualContest(memberSheet, memberImport.getStartAtRow());
            //read member information
            for (int row = memberImport.getStartAtRow(); row <= rowCount; row++)
            {
                string errMsg = "";
                try
                {
                    col = memberImport.getStartAtCol();
                    var cellVal = memberSheet.Cells[row, col].Value + "";
                    //check for team name column value
                    if (!StringUtils.isNullOrEmpty(cellVal))
                    {
                        //read team from sheet team
                        team = result.school.teams.Where(x => x.team_name.ToUpper() == cellVal.ToUpper()).ToList().FirstOrDefault();
                        if (team == null)
                        {
                            error = new import_error_ViewModel();
                            error.objectName = "MEMBER_NORMAL";
                            error.parentObject = APP_CONST.MEMBER;
                            error.occur_position = "Row = " + row;
                            error.type = 1;
                            error.msg = string.Format(Message.MSG023, cellVal);
                            result.error.Add(error);
                        }

                    }
                    //skip for next row
                    if (team == null)
                    {
                        continue;
                    }
                    //member
                    string memberName = memberSheet.Cells[row, ++col].Value + "";
                    string dtString = memberSheet.Cells[row, ++col].Value + "";
                    string email = memberSheet.Cells[row, ++col].Value + "";
                    //check for email is invalid
                    if (!IsValidEmail(email))
                    {
                        error = new import_error_ViewModel();
                        error.objectName = "MEMBER_NORMAL";
                        error.parentObject = APP_CONST.MEMBER;
                        error.occur_position = "Row = " + row;
                        error.msg = Message.MSG026;
                        error.type = 1;
                        result.error.Add(error);
                        continue;
                    }

                    team_member leaderTeamMember = team.team_member.Where(x => x.member.member_role == 3).FirstOrDefault();
                    if (leaderTeamMember != null)
                    {
                        string leaderEmail = leaderTeamMember.member.email; ;
                        //check for current member is leader or not
                        if (leaderEmail.ToUpper().Trim().Equals(email.ToUpper().Trim()))
                        {
                            member = leaderTeamMember.member;
                            //remove leader (add before)
                            result.school.teams.Where(x => x.team_id == team.team_id).FirstOrDefault().team_member.Remove(leaderTeamMember);
                        }
                        else
                        {
                            member = new member();
                            member.member_id = memberId++;
                            member.member_role = 4;
                            //email used by another
                            if (isExistEmail(memberEmail, member.email))
                            {
                                error = new import_error_ViewModel();
                                error.objectName = "MEMBER_NORMAL";
                                error.parentObject = APP_CONST.MEMBER;
                                error.occur_position = "Row = " + row;
                                error.msg = Message.MSG024;
                                error.type = 1;
                                result.error.Add(error);
                                continue;
                            }
                            else
                            {
                                member.email = email.Trim();
                                memberEmail.Add(email.Trim());
                            }
                        }
                    }
                    member.first_name = extractFirstName(memberName);
                    member.middle_name = extractMiddleName(memberName);
                    member.last_name = extractLastName(memberName);
                    member.dob = CommonHelper.toDateTime(dtString);
                    member.phone_number = memberSheet.Cells[row, ++col].Value + "";
                    member.icpc_id = CommonHelper.toInt32(memberSheet.Cells[row, ++col].Value + "", -1);
                    member.gender = getGender(memberSheet.Cells[row, ++col].Value + "");
                    member.year = CommonHelper.toInt32(memberSheet.Cells[row, ++col].Value + "", 0);
                    member.award = memberSheet.Cells[row, ++col].Value + "";
                    member.enabled = true;
                    string validateMemberMsg = validateMemberImport(member);

                    if (!validateMemberMsg.Equals(""))
                    {
                        error = new import_error_ViewModel();
                        error.objectName = "MEMBER_NORMAL";
                        error.parentObject = APP_CONST.MEMBER;
                        error.occur_position = "Row = " + row;
                        error.msg = validateMemberMsg;
                        error.type = 1;
                        result.error.Add(error);
                        continue;
                    }
                    //check for individual contest
                    for (int j = col + 1; j <= colCount; j++)
                    {
                        var joinedText = memberSheet.Cells[row, j].Value + "";
                        if (joinedText.Trim().ToUpper().Equals("YES"))
                        {
                            contest_member individualMember = new contest_member();
                            individualMember.contest = individualContestList[j];
                            individualMember.member = member;
                            member.contest_member.Add(individualMember);
                        }
                    }
                    teamMember = new team_member();
                    teamMember.team_id = team.team_id;
                    teamMember.member_id = member.member_id;
                    teamMember.member = member;
                    //add member to team
                    result.school.teams.Where(x => x.team_id == team.team_id).FirstOrDefault().team_member.Add(teamMember);
                }
                catch (Exception e)
                {
                    errMsg += "\n- UNKOW ERROR";
                    Log.Error(e.Message);
                }

                if (!errMsg.Equals(""))
                {
                    error = new import_error_ViewModel();
                    error.objectName = "MEMBER_NORMAL";
                    error.parentObject = "MEMBER";
                    error.occur_position = "Row = " + row;
                    error.msg = errMsg;
                    result.error.Add(error);
                }
            }
            return result;
        }
        public string validateMemberImport(member member)
        {
            string memberFullName = member.first_name + member.middle_name + member.last_name + "";
            string errmsg = "";
            if (memberFullName.Trim().Equals(""))
            {
                errmsg += Message.MSG025;
            }
            if (member.dob == null || !isOver15YearOld(member.dob))
            {
                errmsg += Message.MSG027;
            }
            return errmsg;
        }
        public member cleanMember(member member)
        {
            member result = new member();
            if (member == null)
            {
                return member;
            }

            result.first_name = member.first_name;
            result.middle_name = member.middle_name;
            result.last_name = member.last_name;
            result.member_role = member.member_role;
            result.dob = member.dob;
            result.email = member.email;
            result.phone_number = member.phone_number;
            result.gender = member.gender;
            result.year = member.year;
            result.award = member.award;
            result.shirt_sizing = member.shirt_sizing;
            result.event_notify = member.event_notify;
            result.icpc_id = member.icpc_id;
            return result;
        }

        public bool isOver15YearOld(DateTime dob)
        {
            DateTime now = DateTime.Now;
            dob = dob.AddYears(15);
            int compare = DateTime.Compare(dob, now);
            if (compare <= 0)
            {
                return true;
            }
            return false;

        }
        public string createEncryptedPassWord(string plainText)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] data = md5.ComputeHash(utf8.GetBytes(plainText));

            var passToData = Convert.ToBase64String(data);

            //MD5 md5 = new MD5CryptoServiceProvider();
            //// Compute hash from the bytes of text
            //md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(plainText));
            //// Get hash result after compute it
            //byte[] result = md5.Hash;
            //StringBuilder strBuilder = new StringBuilder();
            //for (int i = 0; i < result.Length; i++)
            //{
            //    strBuilder.Append(result[i].ToString("x2"));
            //}
            return passToData;
        }
        public string checkExistSheets(List<ExcelWorksheet> sheets)
        {
            List<string> sheetNames = new List<string>();
            foreach (ExcelWorksheet sheet in sheets)
            {
                sheetNames.Add(sheet.Name.Trim().ToUpper());
            }
            if (!sheetNames.Contains(SchoolImport.sheetName.Trim().ToUpper()))
            {
                return SchoolImport.sheetName.Trim();
            }
            if (!sheetNames.Contains(MemberImport.sheetName.Trim().ToUpper()))
            {
                return SchoolImport.sheetName.Trim();
            }
            if (!sheetNames.Contains(TeamImport.sheetName.Trim().ToUpper()))
            {
                return SchoolImport.sheetName.Trim();
            }
            return "";
        }
    }
}