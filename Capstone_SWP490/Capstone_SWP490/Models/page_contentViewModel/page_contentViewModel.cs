using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.page_contentViewModel
{
    public class page_contentViewModel
    {
        public int content_id { get; set; }
        public string page_id { get; set; }
        public string title { get; set; }
        public string title_menu_name { get; set; }
        public string title_menu_link { get; set; }
        public string content { get; set; }
        public int position { get; set; }
        public string user_role { get; set; }
        public Nullable<int> status { get; set; }
    }
}