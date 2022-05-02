using Capstone_SWP490.Models.events_ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.statisticViewModel
{
    public class statistic_eventViewModel
    {
        public statistic_eventViewModel()
        {
            lstEvents = new List<eventsViewModel>();
        }
        public int totalEvent { get; set; }
        public List<eventsViewModel> lstEvents { get; set; }
    }
}