using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.organization_ViewModel
{
    public class organization_ViewModel
    {
        public int coach_id { get; set; }
        public string coach_phone { get; set; }
        public string full_name { get; set; }
        public string school_name { get; set; }
        public string email { get; set; }
        public string institution_name { get; set; }

        public string school_phone { get; set; }
        public string school_address { get; set; }
        public bool status { get; set; }
        public bool is_duplicate_school { get; set; }

    }
}