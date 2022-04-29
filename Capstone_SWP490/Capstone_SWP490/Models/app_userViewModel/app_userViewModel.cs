using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.app_userViewModel
{
    public class app_userViewModel
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string user_role { get; set; }
        public string psw { get; set; }
        public string repsw { get; set; }
        public string encrypted_psw { get; set; }
        public string full_name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public bool verified { get; set; }
        public bool active { get; set; }
        public bool send_me_event { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
        public int? confirm_password { get; set; }
    }
    public class reset_password
    {
        public string old_password { get; set; }
        public string new_password { get; set; }
        public string reNew_password { get; set; }
    }
}