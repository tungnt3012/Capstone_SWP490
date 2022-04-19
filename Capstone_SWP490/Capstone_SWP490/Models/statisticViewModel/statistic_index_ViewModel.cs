using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.statisticViewModel
{
    public class statistic_index_ViewModel
    {
        public List<statistic_schoolViewModel> school_confirrmation { get; set; }
        public int total_registered_school { get; set; }
        public int total_contestant { get; set; }
        public int total_teams { get; set; }
        public statistic_eventViewModel statistic_EventViewModel { get; set; }
        public statistic_shirtSizeViewModel statistic_shirtSizeViewModel { get; set; }
    }
}