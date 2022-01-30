using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.school_memberViewModel
{
    public class school_memberViewModel
    {
        public school_memberViewModel()
        {
            this.school = new school();
            this.coach = new member();
            this.vice_coach = new member();
        }
        public school school { get; set; }
        public member coach { get; set; }
        public member vice_coach { get; set; }
        public team displayTeam { get; set; }
        public void setDisplayTeam(int teamId)
        {
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