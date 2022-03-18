using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.contestViewModel
{
    public class contestViewModel
    {
        public contestViewModel()
        {
            this.contest_member = new HashSet<contest_member>();
        }

        public int contest_id { get; set; }
        public string contest_name { get; set; }
        public string code { get; set; }
        public System.DateTime start_date { get; set; }
        public System.DateTime end_date { get; set; }
        public string venue { get; set; }
        public string contest_type { get; set; }
        public Nullable<short> max_contestant { get; set; }
        public string note { get; set; }
        public Nullable<short> shirt_id { get; set; }
        public virtual ICollection<contest_member> contest_member { get; set; }

    }
}