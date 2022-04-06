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
        public List<import_error_ViewModel> error { get; set; }
        public school school { get; set; }
        public member coach { get; set; }
        public member vice_coach { get; set; }
        public team displayTeam { get; set; }
        public contest displayContest { get; set; }
        public string source { get; set; } = "IMPORT";
        private void sortError()
        {
            if(error !=null)
            error = error.OrderBy(o => o.type).ToList();

        }
        public void setDisplayTeam(int teamId)
        {
            sortError();
            if (school.teams.Count == 0)
            {
                displayTeam = new team();
                return;
            }

            try
            {
                if (teamId == 0)
                {
                    displayTeam = school.teams.FirstOrDefault();
                }
                else
                {
                    foreach (var team in school.teams)
                    {
                        if (team.team_id == teamId)
                        {
                            displayTeam = team;
                            break;
                        }
                    }
                }
                List<team_member> team_member = displayTeam.team_member.Where(x => x.member.member_role == 3).ToList();
                if (team_member != null && team_member.Count > 0)
                {
                    member leader = team_member.FirstOrDefault().member;
                    if (leader.contest_member != null && leader.contest_member.Count > 0)
                        displayContest = leader.contest_member.FirstOrDefault().contest;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
    }
}