using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Capstone_SWP490.Models.post_ViewModel
{
    public class post_ViewModel
    {
        [AllowHtml]
        public post post { get; set; }
        public List<post_error> errors { get; set; }
        public string action { get; set; } = "Create";

        public bool featured { get; set; }
        public string insert_date { get; set; }
        public HttpPostedFileBase file { get; set; }
    }
}