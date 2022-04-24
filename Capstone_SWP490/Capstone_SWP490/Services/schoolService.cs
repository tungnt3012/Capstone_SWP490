using Capstone_SWP490.ExceptionHandler;
using Capstone_SWP490.Helper;
using Capstone_SWP490.Models.statisticViewModel;
using Capstone_SWP490.Repositories;
using Capstone_SWP490.Repositories.Interfaces;
using Capstone_SWP490.Services.Interfaces;
using log4net;
using Resources;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Capstone_SWP490.Services
{
    public class schoolService : IschoolService
    {
        private readonly IschoolRepository _ischoolRepository = new schoolRepository();
        private readonly ImemberRepository _imemberRepository = new memberRepository();
        private static readonly ILog Log = LogManager.GetLogger(typeof(schoolService));
        private readonly Iapp_userService _iappUserService = new app_userService();
        public int count(int coach_id)
        {
            List<school> schools = _ischoolRepository.FindBy(x => x.coach_id == coach_id && x.enabled == true).ToList();
            if (schools != null)
            {
                return schools.Count;
            }
            return 0;
        }

        public async Task<int> deleteAsync(school school)
        {
            return await _ischoolRepository.Delete(school);
        }

        public void disable(school school)
        {
            _ischoolRepository.Update(school, school.school_id);
        }
        public async Task<int> update(school school)
        {
            return await _ischoolRepository.Update(school, school.school_id);
        }
        public int disableUsingStore(int schoolId)
        {
            return _ischoolRepository.getContext().Disable_School_Data(schoolId);
        }

        //public List<school> findByCoachId(int coachId)
        //{
        //    return _ischoolRepository.FindBy(x => x.coach_id == coachId && x.enabled == true).OrderByDescending(x => x.update_date).ToList();
        //}
        public List<school> findByCoachId(int coachId)
        {
            return _ischoolRepository.FindBy(x => x.coach_id == coachId).OrderByDescending(x => x.update_date).ToList();
        }

        public school findActiveById(int id)
        {
            return _ischoolRepository.FindBy(x => x.school_id == id && x.enabled == true).FirstOrDefault();
        }
        public school getInConfirmation(int coachId)
        {
            return _ischoolRepository.FindBy(x => x.coach_id == coachId && x.active == 2).FirstOrDefault();
        }
        public async Task<school> insert(school school)
        {
            try
            {
                return await _ischoolRepository.Create(school);
            }
            catch (Exception e)
            {
                if (e is SchoolException)
                {
                    throw e;
                }
                school = null;
            }
            if (school == null)
            {
                throw new Exception("SYSTEM ERROR");
            }
            return school;
        }

        public school findById(int schoolId)
        {
            return _ischoolRepository.FindBy(x => x.school_id == schoolId).FirstOrDefault();
        }

        public bool isExisted(string schoolName, string institutioName, int coachUserId)
        {
            return _ischoolRepository.FindBy(x => x.coach_id != coachUserId && x.active == 3
            && x.school_name.Equals(schoolName)
            && x.institution_name.Equals(institutioName)).FirstOrDefault() != null;
        }

        public List<statistic_schoolViewModel> findSchoolConfirmation()
        {
            List<statistic_schoolViewModel> result = new List<statistic_schoolViewModel>();
            List<school> schools = _ischoolRepository.FindBy(x => x.active == 1 && x.enabled == true).ToList();
            foreach (school item in schools)
            {
                statistic_schoolViewModel statistic_SchoolViewModel = new statistic_schoolViewModel();
                statistic_SchoolViewModel.school_id = item.school_id;
                statistic_SchoolViewModel.school_name = item.school_name;
                statistic_SchoolViewModel.school_phone = item.phone_number;
                statistic_SchoolViewModel.total_team = item.teams.Count;
                statistic_SchoolViewModel.total_member = (int)_ischoolRepository.getContext().Count_Member_In_School(item.school_id).FirstOrDefault();
                result.Add(statistic_SchoolViewModel);
            }
            return result;
        }

        public List<team> GetTeams()
        {
            List<school> schools = _ischoolRepository.FindBy(x => x.active == 2).ToList();
            List<team> teamRs = new List<team>();
            if (schools != null)
            {
                foreach (var x in schools)
                {
                    if (x.teams.Count > 0)
                    {
                        foreach (var t in x.teams)
                        {
                            //if (t.enabled == true)
                            //{
                            //    teamRs.Add(t);
                            //}
                            teamRs.Add(t);
                        }
                    }
                }
                return teamRs;
            }
            return null;
        }

        public int getRegistered()
        {
            List<school> schools = _ischoolRepository.FindBy(x => x.active == 2 && x.enabled == true).ToList();
            if (schools == null)
            {
                return 0;
            }
            return schools.Count;
        }

        public List<school> listRegistered()
        {
            return _ischoolRepository.FindBy(x => x.active == 3).ToList();
        }
        public List<registered_school_ViewModel> listRegisteredSchool()
        {
            var schools = _ischoolRepository.FindBy(x => x.active == 2 && x.enabled == true).ToList();
            if (schools.Count > 0)
            {
                var schoolsOut = new List<registered_school_ViewModel>();
                foreach (var x in schools)
                {
                    int contestant=0;
                    var schoolsTempt = new registered_school_ViewModel
                    {
                        school = x,
                    };
                    if (x.teams.Count > 0)
                    {
                        foreach(var m in x.teams)
                        {
                            contestant += m.team_member.Count();
                        }
                    }
                    var coach = _imemberRepository.FindBy(u => u.user_id == x.coach_id).FirstOrDefault();
                    schoolsTempt.coach_name = coach.first_name + " " + coach.middle_name + " " + coach.last_name;
                    schoolsTempt.coach_phone = coach.phone_number;
                    schoolsTempt.contestant = contestant;
                    schoolsOut.Add(schoolsTempt);
                }
                return schoolsOut;
            }
            return null;
        }
        public int getTotalContestantInRegistered()
        {
            return (int)_ischoolRepository.getContext().Count_Contestant().FirstOrDefault();
        }

        public async Task<int> useSchool(int schoolId, int coachId)
        {
            try
            {   //find school to update
                school data = _ischoolRepository.FindBy(x => x.school_id == schoolId).FirstOrDefault();
                if (data == null)
                {
                    throw new Exception(Message.MSG028);
                }
                school currentInUse = _ischoolRepository.FindBy(x => x.active == 2 && x.coach_id == coachId).FirstOrDefault();
                //exist data in waiting for confirmation
                if (currentInUse != null)
                {
                    currentInUse.active = 1;
                    await update(currentInUse);
                }
                data.active = 2;
                return await update(data);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw new Exception(Message.SYSTEM_ERROR);
            }
        }

        public Task<int> acceptSchool(int schoolId)
        {
            throw new NotImplementedException();
        }

        public Task<int> rejectSchool(int schoolId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> processSchool(int schoolId, string type, string note)
        {
            school data = _ischoolRepository.FindBy(x => x.school_id == schoolId).FirstOrDefault();
            school coachRegistSchool = findByNewRegistCoach((int)data.coach_id);

            if (data == null)
            {
                throw new Exception(Message.MSG028);
            }
            if (type.Equals("1"))
            {
                coachRegistSchool.school_name = data.school_name;
                coachRegistSchool.institution_name = data.institution_name;
                //if orgnaizier accept then update school record defined when coach create account
                await update(coachRegistSchool);
                data.active = 2;
            }
            else if (type.Equals("2"))
            {
                data.active = 3;
            }
            data.note = note;
            List<school> current = _ischoolRepository.FindBy(x => x.school_id != data.school_id && x.coach_id == data.coach_id && x.enabled == true && x.active >= 1).ToList();
            foreach (var item in current)
            {
                item.enabled = false;
                item.active = -2;
                await update(item);

                foreach (var team in data.teams)
                {
                    foreach (var member in team.team_member)
                    {
                        member.member.app_user.active = false;
                       await _iappUserService.update(member.member.app_user);
                    }
                }

            }

            List<app_user> users = new List<app_user>();
            foreach (var item in data.teams)
            {
                foreach (var member in item.team_member)
                {
                    users.Add(member.member.app_user);
                }
            }
            _ischoolRepository.getContext().Enable_App_User(data.school_id);
            foreach (var item in users)
            {
                new MailHelper().sendMailToInsertedUser(item);
            }
            return await update(data);
        }

        public async Task<int> removeSchool(int schoolId)
        {
            school data = _ischoolRepository.FindBy(x => x.school_id == schoolId).FirstOrDefault();
            if (data == null)
            {
                throw new Exception(Message.MSG028);
            }
            if (data.active == 1 || data.active == -1)
            {
                data.enabled = false;
                return await update(data);
            }
            return -1;
        }

        public bool checkDuplicate(string schoolName, string insitutionName)
        {
            if (StringUtils.isNullOrEmpty(schoolName) || StringUtils.isNullOrEmpty(insitutionName))
            {
                return false;
            }
            school check = _ischoolRepository.FindBy(x => x.school_name.Equals(schoolName) && x.institution_name.Equals(insitutionName)).FirstOrDefault();
            return check == null;
        }

        public school findByNewRegistCoach(int coachId)
        {
            return _ischoolRepository.FindBy(x => x.coach_id == coachId && x.active == -1).FirstOrDefault();
        }

        public async Task<int> RemoveSchoolByCoach(int coachId, int currentInsertId)
        {
            List<school> schools = _ischoolRepository.FindBy(x => x.coach_id == coachId && x.active != -1 && x.active != 2 && x.enabled == true && x.school_id != currentInsertId).ToList();
            int count = 0;
            foreach (var item in schools)
            {
                item.enabled = false;
                item.active = -2;
                await update(item);
                count++;
            }
            return count;
        }

        public List<school> FindActive(int coachId)
        {
            return _ischoolRepository.FindBy(x => x.coach_id == coachId && x.active >= 1 && x.enabled == true).ToList();
        }
    }
}