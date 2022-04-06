using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.registration_statisticModel
{
    public class registration_statisticModel
    {
        public int School_id { get; set; }
        public string School_name { get; set; }
        public string School_short_name { get; set; }
        public string School_phone { get; set; }
        public string Coach_name { get; set; }

        public string Coach_phone { get; set; }
        public string School_address { get; set; }

        public string Confirm_note { get; set; }

        public int Status { get; set; }

        public int Total_member { get; set; }
        public int Total_team { get; set; }


    }
}