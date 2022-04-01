using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.coachViewModel
{
    public class coachSignUpViewModel
    {
        public int id { get; set; }
        public string full_name { get; set; }

        public string email {get;set; }
        public string phone_numer { get; set; }
        public string school_phone { get; set; }
        public string school_name { get; set; }
        public string institution_name  { get; set; }

        public string school_address { get; set; }
        public string type { get; set; }
    }
}