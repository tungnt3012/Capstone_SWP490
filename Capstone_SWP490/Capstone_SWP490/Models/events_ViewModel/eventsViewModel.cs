using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.events_ViewModel
{
    public class eventsViewModel
    {
        public int event_id { get; set; }
        public short event_type { get; set; }
        public string title { get; set; }
        public string desctiption { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public TimeSpan start_time { get; set; }
        public TimeSpan end_time { get; set; }
        public string start_date_str { get; set; }
        public string end_date_str { get; set; }
        public string venue { get; set; }
        public string contactor_name { get; set; }
        public string contactor_email { get; set; }
        public string contactor_phone { get; set; }
        public string fan_page { get; set; }
        public Nullable<short> shirt_id { get; set; }
        public string note { get; set; }
    }
}