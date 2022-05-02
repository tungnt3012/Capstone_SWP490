using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.statisticViewModel
{
    public class registered_contest_ViewModel
    {
        public registered_contest_ViewModel()
        {
            TeamContest = new Dictionary<contest, List<team>>();
            IndividualContest = new Dictionary<contest, List<member>>();
        }
        public contest contest { get; set; }
        public List<member> lstMember { get; set; }
        public int contestant_number { get; set; }

        public Dictionary<contest,List<team>> TeamContest { get; set; }

        public Dictionary<contest, List<member>> IndividualContest { get; set; }
    }
}