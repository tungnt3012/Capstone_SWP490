using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.memberViewModel
{
    public class memberViewModel
    {
        public int member_id { get; set; }
        public int user_id { get; set; }
        public short member_role { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public DateTime dob { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public short gender { get; set; }
        public int year { get; set; }
        public string award { get; set; }
        public string shirt_sizing { get; set; }
        public bool event_notify { get; set; }
    }
}