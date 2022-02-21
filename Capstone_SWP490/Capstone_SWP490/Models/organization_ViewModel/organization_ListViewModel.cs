using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.organization_ViewModel
{
    public class organization_ListViewModel
    {
        public List<organization_ViewModel> data { get; set; }

        public organization_ListViewModel()
        {
            comboxStatus = new List<string>();
            comboxStatus.Add("All");
            comboxStatus.Add("Enabled");
            comboxStatus.Add("Disabled");
        }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public string status { get; set; }

        public int total_data { get; set; }
        public int total_page { get; set; }

        public List<string> comboxStatus { get; }
        public string keyword { get; set; }
    }
}