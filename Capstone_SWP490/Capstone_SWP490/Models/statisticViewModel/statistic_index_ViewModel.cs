using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.statisticViewModel
{
    public class statistic_index_ViewModel
    {
        public statistic_index_ViewModel()
        {
            school_confirrmation = new List<statistic_schoolViewModel>();
            list_registered_school = new List<registered_school_ViewModel>();
            list_registered_contest = new registered_contest_ViewModel();
            list_registered_team = new List<team>();
            statistic_EventViewModel = new statistic_eventViewModel();
            statistic_shirtSizeViewModel = new statistic_shirtSizeViewModel();
        }
        public List<statistic_schoolViewModel> school_confirrmation { get; set; }
        public int total_registered_school { get; set; }
        public List<registered_school_ViewModel> list_registered_school { get; set; }
        public school detail_school { get; set; }
        public int total_contestant { get; set; }
        public registered_contest_ViewModel list_registered_contest { get; set; }
        public int total_teams { get; set; }
        public List<team> list_registered_team { get; set; }
        public statistic_eventViewModel statistic_EventViewModel { get; set; }
        public statistic_shirtSizeViewModel statistic_shirtSizeViewModel { get; set; }
    }
}