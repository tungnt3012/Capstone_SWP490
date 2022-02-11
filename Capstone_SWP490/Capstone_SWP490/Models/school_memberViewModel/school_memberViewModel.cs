using System.Collections.Generic;

namespace Capstone_SWP490.Models.school_memberViewModel
{
    public class school_memberViewModel
    {
        public school_memberViewModel()
        {
            this.school = new school();
            this.coach = new member();
            this.vice_coach = new member();
            this.error = new List<insert_member_result_ViewModel>();
        }
        public List<insert_member_result_ViewModel> error { get; set; }
        public school school { get; set; }
        public member coach { get; set; }
        public member vice_coach { get; set; }
        public team displayTeam { get; set; }
        public contest displayContest { get; set; }
        public void setDisplayTeam(int teamId)
        {
            if(school.teams.Count == 0)
            {
                displayTeam = new team();
                return;
            }

            foreach (var team in school.teams)
            {
                if (team.team_id == teamId)
                {
                   displayTeam = team;
                }
            }
        }
    }
}