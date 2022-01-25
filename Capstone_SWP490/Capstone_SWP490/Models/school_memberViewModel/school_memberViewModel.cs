using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.school_memberViewModel
{
    public class school_memberViewModel
    {
        public school school { get; set; }
        public member coach { get; set; }
        public member vice_coach { get; set; }
        public Dictionary<contest, List<team>> contest_team { get; set; }
        public Dictionary<team, List<member>> team_member { get; set; }
    }
}