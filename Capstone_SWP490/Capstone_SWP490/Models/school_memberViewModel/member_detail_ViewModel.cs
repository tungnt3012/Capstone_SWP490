using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.school_memberViewModel
{
    public class member_detail_ViewModel
    {
        public member_detail_ViewModel()
        {
            errors = new Dictionary<string, string>();
        }
        public int team_id { get; set; }
        public int member_id { get; set; }
        public bool is_leader { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public System.DateTime dob { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public short gender { get; set; }
        public int? year { get; set; }
        public string award { get; set; }
        public int? icpc_id { get; set; }

        public Dictionary<string, string> errors { get; set; }
        public List<member_contest_ViewModel> contest_Members { get; set; }
    }
}