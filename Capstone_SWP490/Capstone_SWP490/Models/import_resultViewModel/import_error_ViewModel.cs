using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.school_memberViewModel
{
    public class import_error_ViewModel
    {
        public string parentObject { get; set; }
        public string objectName { get; set; }
        public string occur_position { get; set; }
        public string msg;
        //1: error, 2 warning
        public int type { get; set; } = 1;

    }
}