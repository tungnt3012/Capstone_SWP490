using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Capstone_SWP490.Models.school_memberViewModel;
using iImport = Capstone_SWP490.Common.ExcelImportPosition;
using schoolImport = Capstone_SWP490.Common.ExcelImportPosition;
using Capstone_SWP490.Services.Interfaces;
using Capstone_SWP490.Services;

namespace Capstone_SWP490.Controllers.Coach
{
    public class RegistrationController : Controller
    {
        private readonly IcontestService _icontestService = new contestService();

        // GET: Registration
        public ActionResult Index()
        {
            var isUploaded = TempData["isUploaded"];
            if(isUploaded != null)
            {
                isUploaded = isUploaded.ToString();
                if (isUploaded.Equals("true"))
                {
                    ViewBag.message = "success";
                }
                else
                {
                    ViewBag.message = "fail";
                }
                return View();
            }
            return View();
        }

        // GET: Registration
        public ActionResult detail()
        {
            return View();
        }
        public ActionResult UploadFile()
        {
            return View();
        }
        // POST: Registration/UploadFile
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (file ==null || file.ContentLength <=0)
            {
                if (file == null)
                {TempData["isUploaded"] = "You not selected file yet";}
                else { TempData["isUploaded"] = "You selected empty file"; }

                return RedirectToAction("Index", "Registration");
            }
            try
            {
                    string _FileName = createFileName(Path.GetExtension(file.FileName));
                    string _path = Path.Combine(Server.MapPath("~/resources/data/temp"), _FileName);
                    file.SaveAs(_path);
                    school_memberViewModel data = import(_path);
                    return View(data);
            }
            catch(Exception e)
            {
                TempData["isUploaded"] = "false";
            }
            finally
            {
                if(file !=null)
                file.InputStream.Close();
            }
            return RedirectToAction("Index", "Registration");
        }
        private string createFileName(string fileExtension)
        {
            return DateTime.UtcNow.Ticks + "." + fileExtension;
        }

        private school_memberViewModel import(string filepath)
        {
            Application objXL = null;
            Workbook workbook = null;
            school_memberViewModel result = new school_memberViewModel();
            try
            {
                objXL = new Application();
                workbook = objXL.Workbooks.Open(filepath);
                int numberOfWorkSheet = workbook.Worksheets.Count;
                schoolImport.SchoolImport schoolExportPosition = new schoolImport.SchoolImport();
                Worksheet sheet = (Worksheet)workbook.Worksheets[schoolExportPosition.sheetPosition];
                string[,] data = readExcelSheetCustom(sheet, schoolExportPosition);
                result.school = getSchool(data);
                result.coach = getCoach(data);
                result.vice_coach = getViceCoach(data);
                Dictionary<contest, List<team_member>> contestTeam = readTeamFromExcel(result.school, workbook, 3);
                List<team_member> teams = getTeamOfSchool(contestTeam);
                Dictionary<team, List<member>> team_member = readMemberFromExcel(teams, workbook, 4);
                result.team_member = team_member;

            }
            catch(Exception e)
            {
            }
            finally
            {
                #region Clean Up                
                if (workbook != null)
                {
                    #region Clean Up Close the workbook and release all the memory.
                    workbook.Close(false, filepath, Missing.Value);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    #endregion
                }
                deleteFileAfterImport(filepath);
                #endregion
            }

            return result;
        }
        private string[,] readExcelSheetCustom( Worksheet sheet, iImport.IExcelPosition positions)
        {
            int[,] index = positions.GetPosition();
            int dataLen = index.Length / 2;
            string[,] data = new string[dataLen, 1];
            try
            {
                for (int i = 0; i < dataLen; i++)
                {
                    Range cell = sheet.Cells[index[i, 0]][index[i, 1]];
                    var val = cell.Value;
                    data[i, 0] = val + "";
                }
            }
            catch { }
            return data;
        }

        private Dictionary<contest, List<team_member>> readTeamFromExcel(school school, Workbook workbook, int sheetIndex)
        {

            Dictionary<contest, List<team_member>> data = new Dictionary<contest, List<team_member>>();
            List<team_member> teamMembers = new List<team_member>();
            try
            {
                contest contest = new contest();
                Worksheet sheet = (Worksheet)workbook.Worksheets[sheetIndex];
                int startRowInRange = 2;
                // the entire table :
                Range range = sheet.UsedRange; // range.Rows.Count, range.Columns.Count
                                               //read from column team name
                Range currentColumn = range.Columns[1, Type.Missing];
                bool isExistContest = true;
                //read contest column
                int teamId = 0;
                for (int row = startRowInRange; row <= currentColumn.Rows.Count; row++)
                {
                    Range cellContest = range.Cells[row, 1];
                    var contestCode = cellContest.Value;
                    //check for next contest
                    if (contestCode == null && isExistContest)
                    {
                        int col = 1;
                        string teamName = range.Cells[row, ++col].Value + "";
                        if (teamName == null || teamName.Equals(""))
                        {
                            continue;
                        }
                        //team
                        team team = new team();
                        team.team_id = teamId;
                        teamId++;
                        team.school_id = school.school_id;
                        team.team_name = teamName;
                        //leader
                        member leader = new member();
                        string fullName = range.Cells[row, ++col].Value + "";
                        leader.first_name = extractFirstName(fullName);
                        leader.middle_name = extractMiddleName(fullName);
                        leader.last_name = extractLastName(fullName);
                        leader.email = range.Cells[row, ++col].Value + "";
                        leader.phone_number = range.Cells[row, ++col].Value + "";
                        leader.member_role = 3;

                        team_member teamMember = new team_member();
                        teamMember.member = leader;
                        teamMember.team = team;
                        teamMembers.Add(teamMember);
                    }
                    else
                    {   //add first team
                        if (row != startRowInRange && isExistContest)
                        {
                            data.Add(contest, teamMembers);
                        }
                        teamMembers = new List<team_member>();
                        //get contest in database
                        contest = _icontestService.getByCode(contestCode);
                        //not found in database
                        if (contest == null)
                        {
                            isExistContest = false;
                            continue;
                        }
                        isExistContest = true;
                    }

                }
                //for last team
                if (isExistContest)
                {
                    data.Add(contest, teamMembers);
                }
            }
            catch { }
            return data;
        }

        private Dictionary<team, List<member>>  readMemberFromExcel(List<team_member> teams , Workbook workbook,int sheetIndex)
        {
            member m;
            Dictionary<team, List<member>> data = new Dictionary<team, List<member>>();
            try
            {
                Worksheet sheet = (Worksheet)workbook.Worksheets[sheetIndex];
                team_member team = null;
                // the entire table :
                Range range = sheet.UsedRange; // range.Rows.Count, range.Columns.Count
                int colTeamIndex = 2;
                //read from column team name
                Range colTeam = range.Columns[colTeamIndex, Type.Missing];
                int totalTeamRow = colTeam.Rows.Count;

                for (int row = 3; row <= totalTeamRow; row++)
                {
                    Range cellTeamName = range.Cells[row, colTeamIndex];
                    var celVal = cellTeamName.Value;
                    if (celVal != null)
                    {
                        team = getTeamByTeamName(teams, celVal);
                        if (team == null)
                        {
                            continue;
                        }
                        data.Add(team.team, new List<member>());
                    }
                    m = new member();
                    int col = 2;
                    string fullName = range.Cells[row, ++col].Value + "";
                    m.first_name = extractFirstName(fullName);
                    m.middle_name = extractMiddleName(fullName);
                    m.last_name = extractLastName(fullName);
                    try
                    {
                        m.dob = DateTime.Parse(range.Cells[row, ++col].Value + "");
                    }
                    catch
                    {
                    }
                    m.email = range.Cells[row, ++col].Value + "";
                    m.phone_number = range.Cells[row, ++col].Value + "";
                    try
                    {
                        m.gender = short.Parse(range.Cells[row, ++col].Value + "");
                    }
                    catch
                    {

                    }
                    try
                    {
                        m.year = Int32.Parse(range.Cells[row, ++col].Value + "");
                    }
                    catch
                    {

                    }
                    m.award = range.Cells[row, ++col].Value + "";
                    if (isTeamLeader(m, team.member))
                    {
                        m.member_role = 3;
                    }
                    else
                    {
                        m.member_role = 4;
                    }
                    if (team != null)
                    {
                        data[team.team].Add(m);
                    }
                }
            }
            catch { }
            return data;
        }
        private bool isTeamLeader(member member, member compared)
        {
            if(compared == null)
            {
                return false;
            }
            return (member.first_name == compared.first_name && member.middle_name == compared.middle_name && member.last_name == compared.last_name
                && member.email == compared.email && member.phone_number == member.phone_number);
        }
        public team_member getTeamByTeamName(List<team_member> teams, string teamName)
        {
            if (teams == null)
                return null;
            return teams.Find(x => x.team.team_name == teamName);
        }
        private List<team_member> getTeamOfSchool(Dictionary<contest, List<team_member>> data)
        {
            List<team_member> teams = new List<team_member>();
            foreach(contest contest in data.Keys)
            {
                List<team_member> teamMmber = data[contest];
                teams.AddRange(teamMmber);
            }
            return teams.Distinct().ToList();
        }
        private member getCoach(string[,] data)
        {
            member coach = new member();
            coach.first_name = extractFirstName(data[2, 0]);
            coach.middle_name = extractMiddleName(data[2, 0]);
            coach.last_name = extractLastName(data[2, 0]);
            coach.phone_number = data[3, 0];
            coach.email = data[4, 0];
            coach.member_role = 1;
            return coach;
        }
        private member getViceCoach(string[,] data)
        {
            member viceCoach = new member();
            viceCoach.first_name = extractFirstName(data[5, 0]);
            viceCoach.first_name = extractMiddleName(data[5, 0]);
            viceCoach.first_name = extractLastName(data[5, 0]);
            viceCoach.phone_number = data[6, 0];
            viceCoach.email = data[7, 0];
            viceCoach.member_role = 2;
            return viceCoach;
        }
        private school getSchool(string[,] data)
        {
            school school = new school();
            school.school_name = data[0, 0];
            school.address = data[1, 0];
            return school;
        }
        private string extractFirstName(string name)
        {
            string[] arr = name.Split(' ');
            if(arr.Length == 0)
            {
                return "";
            }
            return arr.FirstOrDefault();
        }
        private string extractLastName(string name)
        {
            string[] arr = name.Split(' ');
            if (arr.Length == 0)
            {
                return "";
            }
            return arr.LastOrDefault();
        }

        private string extractMiddleName(string name)
        {
            string[] arr = name.Split(' ');
            if (arr.Length <= 2)
            {
                return "";
            }
            if(arr.Length == 3) { return arr[1]; }
            string result = "";
            for(int i = 1; i < arr.Length - 1; i++)
            {
                result += " ";
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
                throw ioExp;
            }
        }
    }
}
