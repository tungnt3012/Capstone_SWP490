using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.post_ViewModel
{
    public class post_TopViewModel
    {
        public int post_id { get; set; }
        public int post_by { get; set; }
        public string insert_date { get; set; }
        public string update_date { get; set; }
        public string title { get; set; }
        public string short_description { get; set; }
        public string content { get; set; }
        public string html_content { get; set; }
        public Nullable<bool> featured { get; set; }
        public Nullable<bool> enabled { get; set; }
        public string schedule_date { get; set; }
        public string post_to { get; set; }
        public string title_image { get; set; }

    }
}