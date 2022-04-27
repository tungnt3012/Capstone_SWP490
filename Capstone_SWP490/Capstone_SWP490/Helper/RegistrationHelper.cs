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
        public team_member GetTeamByTeamName(List<team_member> teams, string teamName)
        {
            if (teams == null)
                return null;
            return teams.Find(x => x.team.team_name == teamName);
        }

        public string CreateFileName(string fileExtension)
        {
            return DateTime.UtcNow.Ticks + "." + fileExtension;
        }

        public member ValidImportMember(member member)
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
                if (!IsOver15YearOld(member.dob))
                {
                    throw new Exception(role + "age must greater or equal 15 year old!");
                }
            }
            return member;
        }
        public List<member_contest_ViewModel> CreateContestViewMode(List<contest_member> contestMember)
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
        public import_resultViewModel UpdateCoach(string[,] data, import_resultViewModel result)
        {
            member loginedCoach = result.Coach;
            string full_name = data[4, 0];
            loginedCoach.first_name = ExtractFirstName(full_name);
            loginedCoach.middle_name = ExtractMiddleName(full_name);
            loginedCoach.last_name = ExtractLastName(full_name);
            loginedCoach.phone_number = data[5, 0];
            loginedCoach.email = data[6, 0];
            loginedCoach.member_role = 1;
            try
            {
                loginedCoach = ValidImportMember(loginedCoach);
                result.Coach = loginedCoach;
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<import_resultViewModel> AddViceCoach(string[,] data, import_resultViewModel result)
        {
            member viceCoach = new member();
            viceCoach.first_name = ExtractFirstName(data[7, 0]);
            viceCoach.middle_name = ExtractMiddleName(data[7, 0]);
            viceCoach.last_name = ExtractLastName(data[7, 0]);
            viceCoach.phone_number = data[8, 0];
            viceCoach.email = data[9, 0];
            viceCoach.member_role = 2;
            try
            {
                viceCoach = ValidImportMember(viceCoach);
                app_user viceCoachAppUser = await CreateAppUserForMember(viceCoach, result.Coach.user_id);
                viceCoach.user_id = viceCoachAppUser.user_id;
                viceCoach.app_user = viceCoachAppUser;
                result.ViceCoach = viceCoach;
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public school GetSchool(string[,] data)
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
        public string ExtractFirstName(string name)
        {
            string[] arr = name.Split(' ');
            if (arr.Length == 0)
            {
                return "";
            }
            return StringUtils.upperCaseFirstCharacter(arr.FirstOrDefault());
        }
        public string ExtractLastName(string name)
        {
            string[] arr = name.Split(' ');
            if (arr.Length == 0)
            {
                return "";
            }
            return StringUtils.upperCaseFirstCharacter(arr.LastOrDefault());
        }
        public string ExtractMiddleName(string name)
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
        public school BuildSchool(school school)
        {
            school.teams = null;
            school.insert_date = DateTime.Now + "";
            school.update_date = DateTime.Now + "";
            school.active = 1;
            school.enabled = true;
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
        public short GetGender(string text)
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

        public import_error_ViewModel ValidateTeam(List<team> currentTeam, team team, int row)
        {
            import_error_ViewModel error = new import_error_ViewModel
            {
                objectName = "MEMBER-LEADER",
                parentObject = APP_CONST.TEAM,
                occur_position = "Row = " + row,
                type = 1
            };

            //team name cannot be black or less than 2 character
            if (StringUtils.isNullOrEmpty(team.team_name) || team.team_name.Length <= 2)
            {
                error.msg = Message.MSG017;
                return error;
            }
            team = GetTeamByTeamName(currentTeam, team.team_name);
            //skip because team existed before or used by others
            if (team != null)
            {
                string msg = Message.MSG018;
                error.msg = msg.Replace("#TEAM_NAME#", team.team_name);
                return error;
            }
            return null;
        }
        public import_resultViewModel ReadTeamSheet(import_resultViewModel result, ExcelWorksheet teamSheet, TeamImport teamObject)
        {
            //hold all registed team in imported school and other schools
            List<team> registedTeam = _iteamService.findRegistedTeam(result.Coach.user_id).ToList();
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
                        if (contest == null || contest.max_contestant == 1)
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
                    team = new team
                    {
                        team_id = teamId++,
                        school = result.School,
                        school_id = result.School.school_id,
                        team_name = teamName.Trim(),
                        contest_id = contest.contest_id,
                        contest = contest,
                        type = "NORMAL"
                    };

                    import_error_ViewModel validError = ValidateTeam(registedTeam, team, row);
                    if (validError != null)
                    {
                        result.TeamErrorList.Add(teamName);
                        result.error.Add(validError);
                        continue;
                    }

                    string leaderFullName = teamSheet.Cells[row, ++col].Value + "";
                    string leaderEmail = $"{teamSheet.Cells[row, ++col].Value}".Trim().ToLower();
                    string leaderPhone = teamSheet.Cells[row, ++col].Value + "";
                    //skip because team leader email or leader Name name is not valid
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
                    //read leader
                    leader = new member();
                    leader.member_role = 3;
                    leader.member_id = 0;
                    leader.email = leaderEmail;
                    leader.first_name = ExtractFirstName(leaderFullName);
                    leader.middle_name = ExtractMiddleName(leaderFullName);
                    leader.last_name = ExtractLastName(leaderFullName);
                    leader.phone_number = leaderPhone;
                    leader.dob = new DateTime();
                    leader.gender = -1;
                    leader.year = -1;

                    //check email in use
                    if (_iapp_UserService.isEmailInUse(leaderEmail, result.Coach.user_id))
                    {
                        error = new import_error_ViewModel
                        {
                            objectName = "MEMBER_NORMAL",
                            parentObject = APP_CONST.MEMBER,
                            occur_position = "Row = " + row,
                            msg = string.Format(Message.MSG024, cellVal),
                            type = 1
                        };
                        result.error.Add(error);
                    }
                    else
                    {
                        //add leader to team
                        teamMember = new team_member();
                        teamMember.member = leader;
                        teamMember.team = team;
                        teamMember.team_id = team.team_id;
                        teamMember.member_id = leader.member_id;
                        team.team_member.Add(teamMember);
                    }
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
            result.School.teams = schoolTeam;
            return result;
        }

        public import_resultViewModel ReadSchoolSheet(import_resultViewModel result, ExcelWorksheet schoolSheet, SchoolImport importObject)
        {
            import_error_ViewModel error;
            int col = importObject.getStartAtCol();
            int row = importObject.getStartAtRow();
            school firstRegist = _ischoolService.findByNewRegistCoach(result.Coach.app_user.user_id);
            //read school, if any field is empty then use first regist
            string school_name = (schoolSheet.Cells[row++, col].Value + "").Trim();
            if (StringUtils.isNullOrEmpty(school_name))
            {
                school_name = firstRegist.school_name;
                error = new import_error_ViewModel
                {
                    objectName = APP_CONST.SCHOOL,
                    occur_position = "ROW = " + row,
                    msg = Message.MSG006,
                    parentObject = SchoolImport.sheetName,
                    type = 2
                };
                result.error.Add(error);
            }
            else if (firstRegist != null && !school_name.Equals(firstRegist.school_name))
            {
                error = new import_error_ViewModel
                {
                    objectName = "COACH",
                    occur_position = "ROW = " + row,
                    msg = Message.MSG031,
                    parentObject = SchoolImport.sheetName,
                    type = 2
                };
                result.error.Add(error);
            }


            string insitution_name = (schoolSheet.Cells[row++, col].Value + "").Trim();
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
            else if (firstRegist != null && !insitution_name.Equals(firstRegist.institution_name))
            {
                error = new import_error_ViewModel();
                error.objectName = "COACH";
                error.occur_position = "ROW = " + row;
                error.msg = Message.MSG030;
                error.parentObject = SchoolImport.sheetName;
                error.type = 2;
                result.error.Add(error);
            }

            string rector_name = $"{schoolSheet.Cells[row++, col].Value}".Trim();
            if (StringUtils.isNullOrEmpty(rector_name))
            {
                error = new import_error_ViewModel
                {
                    objectName = "COACH",
                    occur_position = "ROW = " + row,
                    msg = Message.MSG008,
                    parentObject = SchoolImport.sheetName,
                    type = 2
                };
                result.error.Add(error);
            }

            string school_phone = $"{schoolSheet.Cells[row++, col].Value}".Trim();
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

            string address = $"{schoolSheet.Cells[row++, col].Value}".Trim();
            if (StringUtils.isNullOrEmpty(address))
            {
                school_phone = firstRegist.phone_number;
                error = new import_error_ViewModel
                {
                    objectName = "COACH",
                    occur_position = "ROW = " + row,
                    msg = Message.MSG010,
                    parentObject = SchoolImport.sheetName,
                    type = 2
                };
                result.error.Add(error);
            }

            string school_website = $"{schoolSheet.Cells[row++, col].Value}";
            school school = new school
            {
                school_name = school_name,
                rector_name = rector_name,
                institution_name = insitution_name,
                phone_number = school_phone,
                website = school_website,
                address = address,
                active = 1,
                insert_date = DateTime.Now + "",
                update_date = DateTime.Now + "",
                coach_id = result.Coach.user_id,
                enabled = true
            };
            result.School = school;
            string coach_name = $"{schoolSheet.Cells[row++, col].Value}".Trim();
            string msgValidateCoach = "";
            if (StringUtils.isNullOrEmpty(coach_name))
            {
                coach_name = result.Coach.first_name + " " + result.Coach.middle_name + " " + result.Coach.last_name;
                msgValidateCoach += Message.MSG011;
            }

            string coach_email = $"{schoolSheet.Cells[row++, col].Value}".Trim();
            if (!IsValidEmail(coach_email) || !coach_email.Equals(result.Coach.email))
            {
                msgValidateCoach += "\n" + Message.MSG012;
            }
            if (!StringUtils.isNullOrEmpty(msgValidateCoach))
            {
                error = new import_error_ViewModel();
                error.objectName = "COACH";
                error.occur_position = "ROW = 7 OR 8";
                error.msg = msgValidateCoach.StartsWith("\n") ? msgValidateCoach.Remove(0, 1) : msgValidateCoach;
                error.parentObject = SchoolImport.sheetName;
                error.type = 2;
                result.error.Add(error);
            }
            string coach_phone = schoolSheet.Cells[row++, col].Value + "";
            if (StringUtils.isNullOrEmpty(coach_phone))
            {
                coach_phone = result.Coach.phone_number;
            }
            //update coach
            result.Coach.first_name = ExtractFirstName(coach_name);
            result.Coach.middle_name = ExtractMiddleName(coach_name);
            result.Coach.last_name = ExtractLastName(coach_name);
            result.Coach.phone_number = coach_phone;

            //read vice coach
            string vice_coach_name = schoolSheet.Cells[row++, col].Value + "";
            string vice_coach_email = schoolSheet.Cells[row++, col].Value + "";
            string vice_coach_phone = schoolSheet.Cells[row++, col].Value + "";

            //start read vice coach
            if (!vice_coach_name.Equals("") && !IsValidEmail(vice_coach_email))
            {
                member vice_coach = new member
                {
                    first_name = ExtractFirstName(vice_coach_name),
                    middle_name = ExtractMiddleName(vice_coach_name),
                    last_name = ExtractLastName(vice_coach_name),
                    phone_number = vice_coach_phone,
                    email = vice_coach_email,
                    member_role = 2,
                    enabled = true
                };
                result.ViceCoach = vice_coach;
            }
            return result;
        }
        public ExcelWorksheet GetSheetByName(List<ExcelWorksheet> sheets, string sheetName)
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
        public team_member GetTeamMember(int memberId, team team)
        {
            team_member result = team.team_member.Where(x => x.member.member_id == memberId).FirstOrDefault();
            if (result != null)
            {
                return result;
            }
            return null;
        }
        public team GetTeamByTeamName(List<team> teams, string teamName)
        {
            foreach (var team in teams)
            {
                if (team.team_name.ToUpper().Equals(teamName.ToUpper()))
                {
                    return team;
                }
            }
            return null;
        }

        public bool IsExistEmail(List<string> emailList, string email)
        {
            return emailList.Where(x => x.Equals(email)).FirstOrDefault() != null;
        }
        public async Task<app_user> CreateAppUserForMember(member member, int coachId)
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

            app_user user = _iapp_UserService.getByUserName(member.email);

            if (user == null)
            {
                user = new app_user();
                string rawPassword = CommonHelper.CreatePassword(8);
                string encryptedPassword = CommonHelper.createEncryptedPassWord(rawPassword);
                user.psw = rawPassword;
                user.user_name = member.email;
                user.user_role = APP_CONST.APP_ROLE.getUserRole(member.member_role);
                user.encrypted_psw = encryptedPassword;
                user.full_name = member.first_name + " " + member.middle_name + " " + member.last_name;
                user.email = member.email;
                user.verified = false;
                user.active = false;
                user.insert_date = DateTime.Now + "";
                user.update_date = DateTime.Now + "";
                user = await _iapp_UserService.creatUserForImportMember(user);
            }
            return user;
        }

        public Dictionary<int, contest> ExtractIndividualContest(ExcelWorksheet memberSheet, int startAtRow)
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
        public import_resultViewModel ReadMemberSheet(import_resultViewModel result, ExcelWorksheet memberSheet, MemberImport memberImport)
        {
            List<string> memberEmail = new List<string>();
            import_error_ViewModel error;
            int rowCount = memberSheet.Dimension.End.Row;     //get row count
            int colCount = memberSheet.Dimension.End.Column;
            int memberId = 1;
            int col;
            team team = null;
            member member = null;
            team_member teamMember;
            Dictionary<int, contest> individualContestList = ExtractIndividualContest(memberSheet, memberImport.getStartAtRow());
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
                        team = result.School.teams.Where(x => x.team_name.ToUpper() == cellVal.ToUpper()).ToList().FirstOrDefault();
                        if (team == null && !result.TeamErrorList.Contains(cellVal))
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
                    string email = $"{memberSheet.Cells[row, ++col].Value}".Trim().ToLower();
                    if (StringUtils.isNullOrEmpty(email))
                    {
                        continue;
                    }
                    //check for email is invalid
                    if (!IsValidEmail(email))
                    {
                        error = new import_error_ViewModel
                        {
                            objectName = "MEMBER_NORMAL",
                            parentObject = APP_CONST.MEMBER,
                            occur_position = "Row = " + row,
                            msg = Message.MSG026,
                            type = 1
                        };
                        result.error.Add(error);
                        continue;
                    }
                    //check email in use
                    if (_iapp_UserService.isEmailInUse(email, result.Coach.user_id) || IsExistEmail(memberEmail, email))
                    {
                        error = new import_error_ViewModel
                        {
                            objectName = "MEMBER_NORMAL",
                            parentObject = APP_CONST.MEMBER,
                            occur_position = "Row = " + row,
                            msg = Message.MSG024,
                            type = 1
                        };
                        result.error.Add(error);
                        continue;
                    }

                    member = new member
                    {
                        member_id = memberId++,
                        member_role = 4,
                        email = email,
                        first_name = ExtractFirstName(memberName),
                        middle_name = ExtractMiddleName(memberName),
                        last_name = ExtractLastName(memberName),
                        dob = CommonHelper.toDateTime(dtString),
                        phone_number = memberSheet.Cells[row, ++col].Value + "",
                        icpc_id = CommonHelper.toInt32(memberSheet.Cells[row, ++col].Value + "", -1),
                        gender = GetGender(memberSheet.Cells[row, ++col].Value + ""),
                        year = CommonHelper.toInt32(memberSheet.Cells[row, ++col].Value + "", 0),
                        award = memberSheet.Cells[row, ++col].Value + "",
                        enabled = true
                    };

                    team_member team_Member = team.team_member.Where(x => x.member.member_role == 3).FirstOrDefault();
                    if (team_Member != null)
                    {
                        if (team_Member.member.email.Equals(email))
                        {
                            result.School.teams.Where(x => x.team_id == team.team_id).FirstOrDefault().team_member.Remove(team_Member);
                            member.member_role = 3;
                        }
                    }

                    string validateMemberMsg = ValidateMemberImport(member);

                    if (!validateMemberMsg.Equals(""))
                    {
                        error = new import_error_ViewModel
                        {
                            objectName = "MEMBER_NORMAL",
                            parentObject = APP_CONST.MEMBER,
                            occur_position = "Row = " + row,
                            msg = validateMemberMsg,
                            type = 1
                        };
                        result.error.Add(error);
                        continue;
                    }
                    //check for individual contest
                    for (int j = col + 1; j <= colCount; j++)
                    {
                        var joinedText = memberSheet.Cells[row, j].Value + "";
                        if (joinedText.Trim().ToUpper().Equals("YES"))
                        {
                            if (member.contest_member.Count >= 2)
                            {
                                error = new import_error_ViewModel
                                {
                                    objectName = "MEMBER_NORMAL",
                                    parentObject = APP_CONST.MEMBER,
                                    occur_position = "Row = " + row,
                                    msg = "Member is in a individual contest",
                                    type = 2
                                };
                                result.error.Add(error);
                                break;
                            }
                            contest_member individualMember = new contest_member();
                            individualMember.contest = individualContestList[j];
                            individualMember.member = member;
                            member.contest_member.Add(individualMember);
                            break;
                        }
                    }

                    teamMember = new team_member
                    {
                        team_id = team.team_id,
                        member_id = member.member_id,
                        member = member,
                        team = team
                    };
                    memberEmail.Add(email.Trim());
                    //add member to team
                    team current = result.School.teams.Where(x => x.team_id == team.team_id).FirstOrDefault();
                    if (current.team_member.Count <= 2)
                    {
                        result.School.teams.Where(x => x.team_id == team.team_id).FirstOrDefault().team_member.Add(teamMember);
                    }
                    else
                    {
                        error = new import_error_ViewModel
                        {
                            objectName = "MEMBER_NORMAL",
                            parentObject = APP_CONST.MEMBER,
                            occur_position = "Row = " + row,
                            msg = Message.MSG035,
                            type = 2
                        };
                        result.error.Add(error);
                    }
                }
                catch (Exception e)
                {
                    errMsg += "\n-" + Message.UNKNOWN_ERROR;
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
            //team must have more than or equal 2 member
            List<team> invalidTeam = result.School.teams.Where(x => x.team_member.Count <= 1).ToList();
            if(invalidTeam != null)
            {
                foreach (var item in invalidTeam) {
                    error = new import_error_ViewModel
                    {
                        objectName = "MEMBER_NORMAL",
                        parentObject = "MEMBER",
                        occur_position = "N/A",
                        msg = "The Team '" + item.team_name + "' have less than 2 member."
                    };
                    result.error.Add(error);
                    result.School.teams.Remove(item);
                }
            }
            return result;
        }
        public string ValidateMemberImport(member member)
        {
            string memberFullName = member.first_name + member.middle_name + member.last_name + "";
            string errmsg = "";
            if (memberFullName.Trim().Equals(""))
            {
                errmsg += Message.MSG025;
            }
            if (member.dob == null || !IsOver15YearOld(member.dob))
            {
                errmsg += Message.MSG027;
            }
            return errmsg;
        }
        public member CleanMember(member member)
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
            result.enabled = true;
            return result;
        }

        public bool IsOver15YearOld(DateTime dob)
        {
            DateTime now = DateTime.Now;
            if (DateTime.Compare(dob, new DateTime()) == 0)
            {
                return false;
            }
            dob = dob.AddYears(15);
            int compare = DateTime.Compare(dob, now);
            if (compare <= 0)
            {
                return true;
            }
            return false;

        }
        public string CreateEncryptedPassWord(string plainText)
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
        public string CheckExistSheets(List<ExcelWorksheet> sheets)
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

        public List<team> SetTeamLeaderIfNotExist(List<team> noneZeroMember)
        {
            //check
            foreach (var item in noneZeroMember)
            {
                team_member team_Member = item.team_member.Where(x => x.member.member_role == 3).FirstOrDefault();
                if (team_Member == null)
                {
                    item.team_member.FirstOrDefault().member.member_role = 3;
                }
            }
            return noneZeroMember;
        }

        public string ValidateMemberDetail(member_detail_ViewModel model)
        {
            string errorMsg = "";
            if (StringUtils.isNullOrEmpty(model.first_name))
            {
                errorMsg = Message.MSG032;
            }
            else if (StringUtils.isNullOrEmpty(model.middle_name))
            {
                errorMsg = Message.MSG033;
            }
            else if (StringUtils.isNullOrEmpty(model.last_name))
            {
                errorMsg = Message.MSG034;
            }
            else if (!IsOver15YearOld(model.dob))
            {
                errorMsg = Message.MSG027;
            }
            else if (!IsValidEmail(model.email))
            {
                errorMsg = Message.MSG003;
            }
            return errorMsg;
        }
    }
}