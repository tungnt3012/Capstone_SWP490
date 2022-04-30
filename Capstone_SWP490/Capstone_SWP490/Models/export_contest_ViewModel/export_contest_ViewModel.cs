using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.export_contest_ViewModel
{
    public class export_contest_ViewModel
    {
        public export_contest_ViewModel()
        {
            Members = new List<export_contest_memberViewModel>();
        }
        public string Type { get; set; }
        public int ContestId { get; set; }
        public string ContestName { get; set; }
        public string ContestCode { get; set; }
        public List<export_contest_memberViewModel> Members { get; set; }
    }
}