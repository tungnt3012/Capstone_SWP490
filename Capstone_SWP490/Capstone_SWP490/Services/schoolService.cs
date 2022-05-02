using Capstone_SWP490.Constant.Const;
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
        private readonly IteamRepository _iteamRepository = new teamRepository();
        private readonly Iapp_userRepository _iappUserRepository = new app_userRepository();
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
                app_user coach = _iappUserRepository.FindBy(x => x.user_id == item.coach_id).FirstOrDefault();
                if (coach != null)
                {
                    statistic_SchoolViewModel.coach_name = coach.full_name;
                    statistic_SchoolViewModel.coach_email = coach.email;
                    statistic_SchoolViewModel.coach_phone = coach.members.FirstOrDefault().phone_number;
                }
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
                    int contestant = 0;
                    var schoolsTempt = new registered_school_ViewModel
                    {
                        school = x,
                    };
                    if (x.teams.Count > 0)
                    {
                        foreach (var m in x.teams)
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

            //accept
            if (type.Equals("1"))
            {
                List<school> current = _ischoolRepository.FindBy(x => x.school_id != data.school_id && x.coach_id == data.coach_id && x.enabled == true && x.active >= 1).ToList();
                MailHelper mailHelper = new MailHelper();
                foreach (var item in current)
                {
                    item.enabled = false;
                    item.active = -2;
                    await update(item);

                    foreach (var team in item.teams)
                    {
                        foreach (var member in team.team_member)
                        {
                            member.member.app_user.active = false;
                            await _iappUserService.update(member.member.app_user);
                        }
                    }

                }

                school stored = _ischoolRepository.FindBy(x => x.active == -1 && x.coach_id == data.coach_id).FirstOrDefault();
                team coachTeam = stored.teams.Where(x => x.type.Equals(APP_CONST.TEAM_ROLE.COACH_TEAM)).FirstOrDefault();
                app_user viceCoachUser = coachTeam.team_member.FirstOrDefault().member.app_user;
                viceCoachUser.active = true;
                await _iappUserService.update(viceCoachUser);
                mailHelper.sendMailToInsertedUser(viceCoachUser);

                foreach (var item in data.teams)
                {
                    foreach (var member in item.team_member)
                    {
                        if(member.member.member_role == 1)
                        {
                            continue;
                        }
                        if (!member.member.app_user.active)
                        {
                            member.member.app_user.active = true;
                            await _iappUserService.update(member.member.app_user);
                            try
                            {
                                mailHelper.sendMailToInsertedUser(member.member.app_user);
                            }
                            catch
                            {

                            }

                        }
                    }
                }
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

        public bool isDuplicateSchool(school item)
        {
            if (item == null || StringUtils.isNullOrEmpty(item.school_name) || StringUtils.isNullOrEmpty(item.institution_name))
            {
                return false;
            }
            school check = _ischoolRepository.FindBy(x => x.institution_name.Equals(item.institution_name) && x.active != 3).FirstOrDefault();
            return check != null;
        }

        public school findByNewRegistCoach(int coachId)
        {
            return _ischoolRepository.FindBy(x => x.coach_id == coachId && x.active != -1).FirstOrDefault();
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
            app_user user = _iappUserService.getByUserId(coachId);
            //case user is vice coach then get coach Id to get school
            if (user != null && user.user_role.Equals("VICE-COACH"))
            {
                member viceCoachMember = _imemberRepository.FindBy(x => x.user_id == user.user_id && x.team_member.Count >= 1).FirstOrDefault();
                if (viceCoachMember != null)
                {
                    team_member coachTeamMember = viceCoachMember.team_member.FirstOrDefault();
                    if (coachTeamMember != null)
                    {
                        team coachTeam = _iteamRepository.FindBy(x => x.team_id == coachTeamMember.team_id).FirstOrDefault();

                        if (coachTeam != null)
                        {
                            coachId = coachTeam.team_member.Where(x => x.member.member_role == 1).FirstOrDefault().member.user_id;
                        }
                    }
                }
            }
            return _ischoolRepository.FindBy(x => x.coach_id == coachId && x.active >= 1 && x.enabled == true).ToList();
        }

        public bool CheckExist(string name, string insitutionName, int coachId)
        {
            return _ischoolRepository.FindBy(x => x.school_name.ToUpper().Trim().Equals(name.Trim().ToUpper())
            && x.institution_name.ToUpper().Trim().Equals(insitutionName.Trim().ToUpper()) && x.coach_id != coachId).FirstOrDefault() != null;
        }
    }
}