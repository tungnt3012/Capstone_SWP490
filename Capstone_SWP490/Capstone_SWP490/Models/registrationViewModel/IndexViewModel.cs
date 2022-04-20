using Capstone_SWP490.Models.school_memberViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.registrationViewModel
{
    public class IndexViewModel
    {
        private List<school> Schools;
        public List<schoolViewModel> school { get; }
        public IndexViewModel(List<school> schools)
        {
            Schools = schools;
            school = new List<schoolViewModel>();
        }
        public IndexViewModel build()
        {
            schoolViewModel schoolViewModel;
            foreach (var item in Schools)
            {
                schoolViewModel = new schoolViewModel();
                schoolViewModel.schoolId = item.school_id;
                schoolViewModel.schoolName = item.school_name;
                schoolViewModel.insitutionName = item.institution_name;
                schoolViewModel.insertDate = item.insert_date;
                schoolViewModel.updateDate = item.update_date;
                schoolViewModel.status = (int)item.active;
                schoolViewModel.note = item.note;
                school.Add(schoolViewModel);
            }
            return this;
        }
        public List<import_error_ViewModel> insert_result { get; set; } = null;
    }
}