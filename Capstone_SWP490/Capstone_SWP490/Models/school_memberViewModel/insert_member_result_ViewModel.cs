using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.school_memberViewModel
{
    public class insert_member_result_ViewModel
    {
        public string parentObject { get; set; }
        public string objectName { get; set; }
        public string occur_position { get; set;}
        public string msg;
        public Dictionary<string, string> errorList { get; set; }

    }
}