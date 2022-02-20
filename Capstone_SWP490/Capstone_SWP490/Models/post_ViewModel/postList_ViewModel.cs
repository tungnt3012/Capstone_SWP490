using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.post_ViewModel
{
    public class postList_ViewModel
    {
        public List<post> posts { get; set; }
        public List<post_error> errors { get; set; }
        public string calDate(string scheduleDate)
        {
            DateTime now = DateTime.Now;
            int totalSecond = (int)DateTime.Parse(scheduleDate).Subtract(now).TotalSeconds;
            int day = totalSecond / (3600 * 24);
            int hours = (totalSecond - (day * 3600 * 24)) / 3600;
            int minues = (totalSecond - day * 3600 * 24 - hours * 3600)/60;
            int second = (totalSecond - day * 3600 * 24 - hours * 3600 - minues * 60);
            return day + "d" + hours + "h" + minues + "m" +second+"s";
        }
    }
}