using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.statisticViewModel
{
    public class registered_contest_ViewModel
    {
        public contest contest { get; set; }
        public List<member> lstMember { get; set; }
        public int contestant_number { get; set; }
    }
}