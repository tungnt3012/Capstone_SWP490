﻿using System;
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
        public string encrypted_psw { get; set; }
        public string full_name { get; set; }
        public string email { get; set; }
        public bool verified { get; set; }
        public bool active { get; set; }
        public bool send_me_event { get; set; }
    }
}