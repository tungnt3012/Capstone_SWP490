using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.statisticViewModel
{
    public class statistic_schoolViewModel
    {
        public int school_id { get; set; }
        public string school_name { get; set; }
        public string school_phone { get; set; }
        public string coach_name { get; set; }
        public string coach_email { get; set; }

        public string coach_phone { get; set; }

        public int total_team { get; set; }
        public int total_member { get; set; }
    }
}