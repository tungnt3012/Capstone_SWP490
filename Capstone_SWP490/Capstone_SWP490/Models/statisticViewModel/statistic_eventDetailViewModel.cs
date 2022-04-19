using Capstone_SWP490.Models.events_ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.statisticViewModel
{
    public class statistic_eventDetailViewModel
    {
        public int event_id { get; set; }
        public short event_type { get; set; }
        public string title { get; set; }
        public string desctiption { get; set; }
        public System.DateTime start_date { get; set; }
        public System.DateTime end_date { get; set; }
        public string venue { get; set; }
        public string contactor_name { get; set; }
        public string contactor_email { get; set; }
        public string contactor_phone { get; set; }
        public string fan_page { get; set; }
        public Nullable<short> shirt_id { get; set; }
        public string note { get; set; }
        public string member_join { get; set; }
        public string sub_event { get; set; }
        public Nullable<int> status { get; set; }
        public List<member> lstMembers { get; set; }
    }
}