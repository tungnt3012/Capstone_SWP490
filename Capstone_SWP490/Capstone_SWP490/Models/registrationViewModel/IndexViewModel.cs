using Capstone_SWP490.Models.school_memberViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.registrationViewModel
{
    public class IndexViewModel
    {
        public List<school> school { get; set; }
        public List<import_error_ViewModel> insert_result { get; set; } = null;
    }
}