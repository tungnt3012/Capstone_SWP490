using Capstone_SWP490.ExceptionHandler;
using Capstone_SWP490.Models.school_memberViewModel;
using log4net;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using iImport = Capstone_SWP490.Common.ExcelImportPosition;
using Capstone_SWP490.Common.ExcelImportPosition;
using interfaces = Capstone_SWP490.Services.Interfaces;
using services = Capstone_SWP490.Services;
using System.Threading.Tasks;

namespace Capstone_SWP490.Helper
{
    public class RegistrationHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(RegistrationHelper));
        private readonly interfaces.IcontestService _icontestService = new services.contestService();
        private readonly interfaces.Iapp_userService _iapp_UserService = new services.app_userService();
        private readonly interfaces.ImemberService _imemberService = new services.memberService();
        public team_member getTeamByTeamName(List<team_member> teams, string teamName)
        {
            if (teams == null)
                return null;
            return teams.Find(x => x.team.team_name == teamName);
        }
        public bool isTeamLeader(member member, member compared)
        {
            if (compared == null)
            {
                return false;
            }
            return (member.email.Equals(compared.email));
        }
        public string createFileName(string fileExtension)
        {
            return DateTime.UtcNow.Ticks + "." + fileExtension;
        }

        public member validImportMember(member member)
        {
            string role = member.member_role == 1 ? "COACH" : member.member_role == 2 ? "VICE-COACH" : member.member_role == 3 ? "LEADER" : "MEMBER";
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
        public school_memberViewModel updateCoach(string[,] data, school_memberViewModel result)
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
        public async Task<school_memberViewModel> addViceCoach(string[,] data, school_memberViewModel result)
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
            string name = upperCaseFirstCharacter(data[0, 0]);
            string shortName = data[1, 0].ToUpper();
            string address = data[2, 0];
            string type = upperCaseFirstCharacter(data[3, 0]);
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
            return upperCaseFirstCharacter(arr.FirstOrDefault());
        }
        public string extractLastName(string name)
        {
            string[] arr = name.Split(' ');
            if (arr.Length == 0)
            {
                return "";
            }
            return upperCaseFirstCharacter(arr.LastOrDefault());
        }
        public string extractMiddleName(string name)
        {
            string[] arr = name.Split(' ');
            if (arr.Length <= 2)
            {
                return "";
            }
            if (arr.Length == 3) { return upperCaseFirstCharacter(arr[1]); }
            string result = "";
            for (int i = 1; i < arr.Length - 1; i++)
            {
                result += " ";
            }
            return upperCaseFirstCharacter(result);
        }
        public school cleanSchool(school school)
        {
            school.teams = null;
            return school;
        }

        public bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
        public int toInt32(string s, int valueIfAbsent)
        {
            try
            {
                return Int32.Parse(s);
            }
            catch
            {
                return valueIfAbsent;
            }
        }
        public short toShort(string s)
        {
            try
            {
                return short.Parse(s);
            }
            catch
            {
                return -1;
            }
        }

        public DateTime toDateTime(string s)
        {
            try
            {
                return DateTime.Parse(s);
            }
            catch
            {
                return new DateTime();
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

        public string upperCaseFirstCharacter(string text)
        {
            if (text.Length == 0)
            {
                return text;
            }

            string result = "";
            string[] arr = text.Trim().Split(' ');
            for (int i = 0; i < arr.Length; i++)
            {
                if (!arr[i].Equals(" ") && !arr[i].Equals(""))
                {
                    string upper = arr[i].ToUpper();
                    result += arr[i].Replace(arr[i].ElementAt(0), upper.ElementAt(0)) + " ";
                }
            }
            return result.Trim();
        }

        public async Task<school_memberViewModel> readTeamSheet(school_memberViewModel result, ExcelWorksheet teamSheet, TeamImport teamObject)
        {
            int coachUserId = result.coach.app_user.user_id;
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

            for (int row = teamObject.getStartAtRow(); row <= rowCount; row++)
            {
                try
                {
                    col = teamObject.getStartAtCol();
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
                            continue;
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
                    if (StringUtils.isNullOrEmpty(teamName) || !IsValidEmail(leaderEmail))
                    {
                        error = new insert_member_result_ViewModel();
                        error.objectName = "MEMBER-LEADER";
                        error.parentObject = "TEAM";
                        error.occur_position = "Row = " + row;
                        error.msg = "Team leader email and Team Name cannot be blank";
                        result.error.Add(error);
                        continue;
                    }

                    team = getTeamByTeamName(teamList, teamName);

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
                    leader.first_name = extractFirstName(leaderFullName);
                    leader.middle_name = extractMiddleName(leaderFullName);
                    leader.last_name = extractLastName(leaderFullName);
                    leader.phone_number = leaderPhone;
                    // leader.app_user = createAppUserFromMember(leader);
                    leader.dob = new DateTime();
                    leader.gender = -1;
                    leader.year = -1;
                    try
                    {
                        app_user leaderLoginUser = await createAppUserForMember(leader, coachUserId);
                        leader.app_user = leaderLoginUser;
                    }
                    catch (Exception e)
                    {
                        error = new insert_member_result_ViewModel();
                        error.objectName = "MEMBER-LEADER";
                        error.parentObject = "TEAM";
                        error.occur_position = "Row = " + row;
                        error.msg = "Team leader " + e.Message;
                        result.error.Add(error);
                        continue;
                    }

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

        public string[,] readExcelSheetCustom(ExcelWorksheet sheet, int[,] positionList)
        {
            int dataLen = positionList.Length / 2;
            string[,] data = new string[dataLen, 1];
            try
            {
                for (int i = 0; i < dataLen; i++)
                {
                    ExcelRange cell = sheet.Cells[positionList[i, 0], positionList[i, 1]];
                    var val = cell.Value;

                    data[i, 0] = (val == null) ? "" : val + "";
                }
                return data;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<school_memberViewModel> readSchoolSheet(school_memberViewModel result, ExcelWorksheet schoolSheet, SchoolImport importObject)
        {

            int col = importObject.getStartAtCol();
            int row = importObject.getStartAtRow();
            //read school
            string school_name = schoolSheet.Cells[row++, col].Value + "";
            if (school_name.Equals(""))
            {
                throw new Exception("School name cannot be empty");
            }
            string insitution_name = schoolSheet.Cells[row++, col].Value + "";
            if (insitution_name.Equals(""))
            {
                throw new Exception("Institution name cannot be empty");
            }
            string rector_name = schoolSheet.Cells[row++, col].Value + "";
            if (rector_name.Equals(""))
            {
                throw new Exception("Rector name cannot be empty");
            }

            string school_phone = schoolSheet.Cells[row++, col].Value + "";
            if (school_phone.Equals(""))
            {
                throw new Exception("School phone cannot be empty");
            }
            string address = schoolSheet.Cells[row++, col].Value + "";
            if (address.Equals(""))
            {
                throw new Exception("School address cannot be empty");
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
            string msgCoach = "";
            if (coach_name.Equals(""))
            {
                insert_member_result_ViewModel error = new insert_member_result_ViewModel();
                error.objectName = "COACH";
                msgCoach += "Coach name is empty, we uses information of logined user";
            }

            string coach_email = schoolSheet.Cells[row++, col].Value + "";
            if (IsValidEmail(coach_email))
            {
                msgCoach += "\nCoach email is empty, we uses information of logined user";
            }
            if (!msgCoach.Equals(""))
            {
                insert_member_result_ViewModel error = new insert_member_result_ViewModel();
                error.objectName = "SCHOOL";
                error.occur_position = "ROW = 6 OR 7";
                error.msg = msgCoach;
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
            if (!vice_coach_name.Equals(""))
            {
                member vice_coach = new member();
                vice_coach.first_name = extractFirstName(vice_coach_name);
                vice_coach.middle_name = extractMiddleName(vice_coach_name);
                vice_coach.last_name = extractLastName(vice_coach_name);
                vice_coach.phone_number = vice_coach_phone;
                vice_coach.email = vice_coach_email;
                if (!IsValidEmail(vice_coach_email))
                {
                    insert_member_result_ViewModel error = new insert_member_result_ViewModel();
                    error.objectName = "SCHOOl";
                    error.occur_position = "ROW = " + 10;
                    error.msg = "Cannot read Vice Coach because Vice Coach email is empty";
                    result.error.Add(error);
                }
                else
                {
                    vice_coach.member_role = 2;
                    vice_coach.app_user = await createAppUserForMember(vice_coach, loginedId);
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
                if (nameUpper.Equals(sheetName.ToUpper()))
                {
                    return sheet;
                }
            }
            return null;
        }
        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
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

        public bool checkExistEmail(List<string> emailList, string email)
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
            user.psw = CreatePassword(8);
            user.user_name = member.email;
            user.user_role = member.member_role == 1 ? "COACH" : member.member_role == 2 ? "VICE-COACH" : member.member_role == 3 ? "LEADER" : "MEMBER";
            user.encrypted_psw = user.psw;
            user.full_name = member.first_name + " " + member.middle_name + " " + member.last_name;
            user.email = member.email;
            user.verified = false;
            user.active = false;
            user = await _iapp_UserService.creatUserForImportMember(user, coachId);
            return user;
        }
        public async Task<school_memberViewModel> readMemberSheet(school_memberViewModel result, ExcelWorksheet memberSheet, MemberImport memberImport)
        {
            int coachUserId = result.coach.app_user.user_id;
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
                    member.first_name = extractFirstName(memberName);
                    member.middle_name = extractMiddleName(memberName);
                    member.last_name = extractLastName(memberName);
                    string dtString = memberSheet.Cells[row, ++col].Value + "";
                    DateTime memberDob = toDateTime(dtString);
                    member.dob = toDateTime(dtString);
                    string email = memberSheet.Cells[row, ++col].Value + "";
                    member.phone_number = memberSheet.Cells[row, ++col].Value + "";
                    member.icpc_id = toInt32(memberSheet.Cells[row, ++col].Value + "", -1);
                    member.gender = getGender(memberSheet.Cells[row, ++col].Value + "");
                    member.year = toInt32(memberSheet.Cells[row, ++col].Value + "", 0);
                    member.award = memberSheet.Cells[row, ++col].Value + "";

                    if (IsValidEmail(email))
                    {
                        member.email = "";
                    }
                    else
                    {
                        member.email = email;
                    }
                    //skip if both email and name is blank
                    if (StringUtils.isNullOrEmpty(memberName) || StringUtils.isNullOrEmpty(member.email))
                    {
                        errMsg = "- Member name or email cannot blank ";
                        error = new insert_member_result_ViewModel();
                        error.objectName = "MEMBER_NORMAL";
                        error.parentObject = "MEMBER";
                        error.occur_position = "Row = " + row;
                        error.msg = errMsg;
                        result.error.Add(error);
                        continue;
                    }

                    if (!isOver15YearOld(memberDob))
                    {
                        errMsg += "- Member age must greater than or equal 15 year old ";
                        member.dob = new DateTime();
                    }
                    team_member leader = team.team_member.Where(x => x.member.member_role == 3).FirstOrDefault();
                    contest_member contestMemmber = new contest_member();
                    contestMemmber.contest = leader.member.contest_member.FirstOrDefault().contest;
                    contestMemmber.member = member;
                    member.contest_member.Add(contestMemmber);
                    //because of leader added before therefore will be skipped
                    if (!isTeamLeader(member, leader.member))
                    {
                        teamMember = new team_member();
                        member.app_user = await createAppUserForMember(member, coachUserId);
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
    }

}