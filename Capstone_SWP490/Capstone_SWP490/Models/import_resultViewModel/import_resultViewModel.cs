using Capstone_SWP490.Constant.Const;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Capstone_SWP490.Models.school_memberViewModel
{
    public class import_resultViewModel
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(import_resultViewModel));
        public import_resultViewModel()
        {
            this.error = new List<import_error_ViewModel>();
        }
        public bool RootError { get; set; } = false;
        public List<import_error_ViewModel> error { get; set; }
        public school School { get; set; }
        public member Coach { get; set; }
        public member ViceCoach { get; set; }
        public team DisplayTeam { get; set; }
        public contest DisplayContest { get; set; }
        public string Source { get; set; } = "IMPORT";
        private void SortError()
        {
            if (error != null)
                error = error.OrderBy(o => o.type).ToList();

        }
        public void SetDisplayTeam(int teamId)
        {
            SortError();
            if (School.teams.Count == 0)
            {
                DisplayTeam = new team();
                return;
            }

            try
            {
                if (teamId == 0)
                {
                    DisplayTeam = School.teams.FirstOrDefault();
                }
                else
                {
                    foreach (var team in School.teams)
                    {
                        if (team.team_id == teamId)
                        {
                            DisplayTeam = team;
                            break;
                        }
                    }
                }
                List<team_member> team_member = DisplayTeam.team_member.Where(x => x.member.member_role == 3).ToList();
                if (team_member != null && team_member.Count > 0)
                {
                    member leader = team_member.FirstOrDefault().member;
                    if (leader.contest_member != null && leader.contest_member.Count > 0)
                        DisplayContest = leader.contest_member.FirstOrDefault().contest;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
        public school GetCleanSchool()
        {
            school result = new school();
            result.school_name = School.school_name;
            result.institution_name = School.institution_name;
            result.address = School.address;
            result.insert_date = DateTime.Now + "";
            result.active = School.active;
            result.update_date = School.update_date;
            result.coach_id = School.coach_id;
            result.enabled = true;
            result.active = 1;
            result.rector_name = School.rector_name;
            result.website = School.website;
            result.phone_number = School.phone_number;
            return result;
        }

        public member GetCleanCoach()
        {
            member result = new member
            {
                member_id = Coach.member_id,
                user_id = Coach.user_id,
                member_role = Coach.member_role,
                first_name = Coach.first_name,
                middle_name = Coach.middle_name,
                last_name = Coach.last_name,
                dob = Coach.dob,
                email = Coach.email,
                phone_number = Coach.phone_number,
                gender = 0,
                year = -1,
                enabled = Coach.enabled
            };
            return result;
        }

        public member GetCleanViceCoach()
        {
            member result = new member
            {
                member_role = ViceCoach.member_role,
                first_name = ViceCoach.first_name,
                middle_name = ViceCoach.middle_name,
                last_name = ViceCoach.last_name,
                dob = ViceCoach.dob,
                email = ViceCoach.email,
                phone_number = ViceCoach.phone_number,
                gender = 0,
                year = -1,
                enabled = Coach.enabled
            };
            return result;
        }

        public team GetCleanTeam(int teamId, int insertedSchoolId)
        {
            team currentTeam = School.teams.Where(x => x.team_id == teamId).FirstOrDefault();
            if (currentTeam != null)
            {
                team team = new team
                {
                    team_name = currentTeam.team_name,
                    school_id = insertedSchoolId,
                    type = APP_CONST.TEAM_ROLE.NORMAL_TEAM,
                    enabled = true,
                    contest_id = currentTeam.contest_id
                };
                return team;
            }
            return null;
        }
    }
}