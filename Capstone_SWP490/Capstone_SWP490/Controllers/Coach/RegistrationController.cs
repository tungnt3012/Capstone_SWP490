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
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Capstone_SWP490.Models.insert_member_result_ViewModel;

namespace Capstone_SWP490.Controllers.Coach
{
    public class RegistrationController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(RegistrationController));
        private readonly interfaces.IcontestService _icontestService = new services.contestService();
        private readonly interfaces.IschoolService _ischoolService = new services.schoolService();
        private readonly interfaces.IteamService _iteamService = new services.teamService();
        private readonly interfaces.ImemberService _imemberService = new services.memberService();
        private readonly interfaces.Iapp_userService _iapp_UserService = new services.app_userService();
        private readonly interfaces.Iteam_memberService _iteam_memberService = new services.teamMemberService();
        private readonly RegistrationHelper registrationHelper = new RegistrationHelper();
        // GET: Registration
        public ActionResult Index()
        {
            Log.Info("Hi I am log4net Info Level");
            return View();
        }

        public ActionResult Guide()
        {
            return View();
        }

        public ActionResult MemberDetail(string id, string teamId)
        {
            school_memberViewModel data = (school_memberViewModel)HttpContext.Session["school"];
            if (data == null)
            {
                return RedirectToAction("Result", "Registration");
            }
            if (id == null || id.Equals(""))
            {
                return RedirectToAction("Result", "Registration", new { team = 0});
            }
            int memberId;
            int teamIdInt;
            try
            {
                 memberId = Int32.Parse(id);
                teamIdInt = Int32.Parse(teamId);
                team_member teamMember = getTeamMember(memberId,data.school.teams.Where( x=> x.team_id == teamIdInt).First());
                if(teamMember != null)
                {
                    return View(teamMember);
                }
            }
            catch { }
            return View();
        }

        private team_member getTeamMember(int memberId, team team)
        {
                team_member result = team.team_member.Where(x => x.member.member_id == memberId).FirstOrDefault();
                if(result != null)
                {
                    return result;
                }
            return null;
        }
        
        public async Task<ActionResult> InsertMember()
        {
            school_memberViewModel data = (school_memberViewModel)HttpContext.Session["school"];
            List<insert_member_result_ViewModel> result = new List<insert_member_result_ViewModel>();
            if (data != null)
            {
                school school = data.school;
                List<team> teams = (List<team>)data.school.teams;
                List<app_user> users = new List<app_user>();
                school = registrationHelper.cleanSchool(school);
                member insertedMember;
                try
                {
                    await _ischoolService.insert(school);
                }
                catch
                {
                    return RedirectToAction("Result", "Registration", new { error = true });
                }
                foreach (team item in teams)
                {
                    HashSet<team_member> teamMemberList = (HashSet<team_member>)item.team_member;
                    item.team_member = null;
                    item.school_id = school.school_id;
                    item.school = school;
                    team insertedTeam = await _iteamService.insert(item);
                    //insert team success
                    if(insertedTeam != null)
                    {
                        foreach (team_member i in teamMemberList)
                        {
                            member member = i.member;
                            member.contest_member = null;
                            member.team_member = null;
                            member.user_id = 4;

                            //insert member
                            insertedMember = await _imemberService.insert(member);
                            if (insertedMember != null)
                            {
                                app_user user = new app_user();
                                user.psw = CreatePassword(8);
                                user.user_name = member.email;
                                user.user_role = "MEMBER";
                                user.encrypted_psw = CreatePassword(8);
                                user.full_name = member.first_name + " " + member.middle_name + " " + member.last_name;
                                user.email = member.email;
                                user.verified = false;
                                user.active = true;

                                app_user insertedUser = await _iapp_UserService.CreateUser(user);
                                if (insertedUser != null)
                                {
                                    users.Add(insertedUser);

                                    insertedMember.user_id = insertedUser.user_id;
                                    await _imemberService.update(insertedMember, insertedMember.member_id);
                                    team_member teamMember = new team_member();
                                    teamMember.member_id = insertedMember.member_id;
                                    teamMember.team_id = insertedTeam.team_id;

                                   await _iteam_memberService.insert(teamMember);
                                }
                            }
                        }
                    }
                }
                sendMailToInsertedUser(users);
            }
            return RedirectToAction("Index", "Registration", new { team = 0 });
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
        [HttpPost]
        public ActionResult UpdateMember(int teamId, int id, string firstName, string middleName,
            string lastName, string dob, string email, string phone, string icpc, int year,
            string gender, string award)
        {
            school_memberViewModel data = (school_memberViewModel)HttpContext.Session["school"];
            if (data == null)
            {
                return RedirectToAction("Result", "Registration");
            }
            try{

                member member = data.school.teams.Where(x => x.team_id == teamId).First().
                     team_member.Where(y => y.member.member_id == id).First().member;
                member.first_name = firstName;
                member.middle_name = middleName;
                member.last_name = lastName;
                member.dob = DateTime.Parse(dob);
                //member.dateStr = member.dob.Year + "-" + member.dob.Month + "-" + member.dob.Day;
                member.email = email;
                member.phone_number = phone;
                member.year = year;
                member.gender = short.Parse(gender);
                member.award = award;

                data.school.teams.Where(x => x.team_id == teamId).First().
                     team_member.Where(y => y.member.member_id == id).First().member = member;
                Session.Add("school", data);
            }
            catch { }
            return RedirectToAction("Result", "Registration", new { team = teamId });
        }
        public ActionResult Result(HttpPostedFileBase file, string team)
        {
            if(team != null && !team.Equals(""))
            {
                school_memberViewModel data = (school_memberViewModel)HttpContext.Session["school"];
                if(data != null)
                {
                    int teamId = 0;
                    try
                    {
                        teamId = Int32.Parse(team);
                    }
                    catch
                    {

                    }
                    data.setDisplayTeam(teamId);
                }
                return View(data);
            }

            if (file is null)
            {
                Log.Error("File imported is null");
                return RedirectToAction("Index", "Registration");
            }
            try
            {
                string _FileName = registrationHelper.createFileName(Path.GetExtension(file.FileName));
                string _path = Path.Combine(Server.MapPath("/App_Data/temp"), _FileName);
                _path = _path.Replace(".xlsx", ".txt");
                file.SaveAs(_path);
                school_memberViewModel data = import(_path);
                Session.Add("school", data);
   
               return View(data);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                TempData["isUploaded"] = "false";
            }
            finally
            {
                if (file != null)
                    file.InputStream.Close();
            }
            return RedirectToAction("Index", "Registration");
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
                    school school = registrationHelper.getSchool(data);
                    //read coach
                    result.coach = registrationHelper.getCoach(data);
                    //read vice coach
                    result.vice_coach = registrationHelper.getViceCoach(data);
                    //read teams
                    iImport.TeamImport teamImport = new iImport.TeamImport();
                    ExcelWorksheet teamSheet = package.Workbook.Worksheets[teamImport.sheetPosition];
                    school = readTeam(school, teamSheet, teamImport);
                    //read team member
                    iImport.MemberImport memberImport = new iImport.MemberImport();
                    ExcelWorksheet memberSheet = package.Workbook.Worksheets[memberImport.sheetPosition];
                    school = readMember(school, memberSheet, memberImport);
                    result.school = school;
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
                    ExcelRange cell = sheet.Cells[index[i, 0],index[i, 1]];
                    var val = cell.Value;
                    
                    data[i, 0] = (val == null) ? "": val+"";
                }
            }
            catch(Exception e) {
                Log.Error(e.Message);
            }
            return data;
        }

        private school readTeam(school school, ExcelWorksheet teamSheet, iImport.IExcelPosition excelImport)
        {

            List<team> teamList = new List<team>();
            contest contest = new contest();
            team team = null;
            member member;
            contest_member contestMember;
            team_member teamMember;
            try
            {
                int rowCount = teamSheet.Dimension.End.Row;     //get row count
                int teamId = 0;
                int col;
                bool existContest = true;
                for (int row = excelImport.getStartAtRow(); row <= rowCount; row++)
                {
                    col = excelImport.getStartAtCol();
                    var cellVal = teamSheet.Cells[row, col].Value + "";
                    //check for contest column value
                    if (!cellVal.Equals(""))
                    {
                        contest = _icontestService.getByCode(cellVal);
                        if(contest == null)
                        {
                            existContest = false;
                        }
                        else
                        {
                            existContest = true;
                        }
                        continue;
                    }
                    //skip because contest not exist
                    if (!existContest)
                    {
                        continue;
                    }
                    //read team
                    team = new team();
                    team.team_id = teamId++;
                    team.school = school;
                    string teamName = teamSheet.Cells[row, ++col].Value + "";
                    if (teamName.Equals(""))
                    {
                        continue;
                    }
                    team.team_name = teamName;
                    //read leader
                    member = new member();
                    member.member_role = 3;
                    member.member_id = 0;
                    string fullName = teamSheet.Cells[row, ++col].Value + "";
                    member.first_name = registrationHelper.extractFirstName(fullName);
                    member.middle_name = registrationHelper.extractMiddleName(fullName);
                    member.last_name = registrationHelper.extractLastName(fullName);
                    member.email = teamSheet.Cells[row, ++col].Value + "";
                    member.phone_number = teamSheet.Cells[row, ++col].Value + "";
                    contestMember = new contest_member();
                    contestMember.contest = contest;
                    member.contest_member.Add(contestMember);
                    //add leader to member of team
                    teamMember = new team_member();
                    teamMember.member = member;
                    teamMember.team = team;
                    team.team_member.Add(teamMember);
                    if (team != null)
                    {
                        teamList.Add(team);
                    }
                }
            }
            catch (Exception e) {
                Log.Error(e.Message);
            }
            school.teams = teamList;
            return school;
        }

        private school readMember(school school, ExcelWorksheet memberSheet, iImport.IExcelPosition memberImport)
        {
            try
            {
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
                    col = memberImport.getStartAtCol();
                    var cellVal = memberSheet.Cells[row, col].Value + "";
                    //check for team name column value
                    if (!cellVal.Equals(""))
                    {
                        //read new team
                        team = school.teams.Where(x => x.team_name == cellVal).ToList().FirstOrDefault();
                    }
                    //skip because team not exist
                    if (team == null)
                    {
                        continue;
                    }
                    string memberName = memberSheet.Cells[row, ++col].Value + "";
                    if (memberName.Equals(""))
                    {
                        continue;
                    }
                    member = new member();
                    member.member_id = memberId++;
                    //normal member
                    member.member_role = 4;
                    member.first_name = registrationHelper.extractFirstName(memberName);
                    member.middle_name = registrationHelper.extractMiddleName(memberName);
                    member.last_name = registrationHelper.extractLastName(memberName);
                    string dtString = memberSheet.Cells[row, ++col].Value + "";
                    try
                    {
                        member.dob = DateTime.Parse(dtString);
                        //member.dateStr = member.dob.Year + "-" + member.dob.Month +"-"+ member.dob.Day;
                    }
                    catch
                    {
                    }
                    member.email = memberSheet.Cells[row, ++col].Value + "";
                    member.phone_number = memberSheet.Cells[row, ++col].Value + "";
                    //skip icpc
                    ++col;

                    try
                    {
                        member.gender = short.Parse(memberSheet.Cells[row, ++col].Value + "");
                    }
                    catch { }
                    try
                    {
                        member.year = Int32.Parse(memberSheet.Cells[row, ++col].Value + "");
                    }
                    catch { }
                    member.award = memberSheet.Cells[row, ++col].Value + "";
                    team_member leader = team.team_member.Where(x => x.member.member_role == 3).FirstOrDefault();
                   //because of leader added before so won't add any more
                    if (!registrationHelper.isTeamLeader(member, leader.member))
                    {
                        teamMember = new team_member();
                        teamMember.member = member;
                        teamMember.team = team;
                        //add to team
                        school.teams.Where(x => x.team_id == team.team_id).FirstOrDefault().team_member.Add(teamMember);
                    }
                    else
                    {
                        //update leader data
                        leader.member.year = member.year;
                        leader.member.award = member.award;
                        leader.member.gender = member.gender;
                        //leader.member.dateStr = member.dateStr;
                        team.team_member.Where(x => x.member.member_role == 3).FirstOrDefault().member = leader.member;
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(e.Message);
            }
                return school;
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
                    //please format body before send
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
                Console.WriteLine(e.Message);
            }
            return false;
        }
        private void sendMailToInsertedUser(List<app_user> users)
        {
            MailReaderHelper emailReader = new MailReaderHelper();
            string mailContent = emailReader.readEmailCreateAccount();
            foreach (app_user item in users)
            {
                EmailModel model = new EmailModel();
                model.toEmail = item.email;
                model.body = string.Format(mailContent, item.psw, "https://www.icpc.online/Login/ChangePasswordFirst", "https://www.icpc.online/Login/ChangePasswordFirst");
                model.title = "ICPC Asia-VietNam";
                sendMailAsync(model);
            }
        }
    }
}