using Capstone_SWP490.Models.events_ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.statisticViewModel
{
    public class statistic_shirtSizeViewModel
    {
        public statistic_shirtSizeViewModel()
        {
            lstMembers = new List<member>();
        }
        public int totalRegisterdSize { get; set; }
        public int sizeS { get; set; }
        public int sizeXS { get; set; }
        public int sizeM { get; set; }
        public int sizeL { get; set; }
        public int sizeXL { get; set; }
        public int size2XL { get; set; }
        public int size3XL { get; set; }
        public List<member> lstMembers { get; set; }
    }
}