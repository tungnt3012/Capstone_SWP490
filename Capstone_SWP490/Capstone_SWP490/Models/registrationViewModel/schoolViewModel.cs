using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.registrationViewModel
{
    public class schoolViewModel
    {
        public int schoolId { get; set; }
        public string schoolName { get; set; }
        public string insitutionName { get; set; }
        public string note { get; set; }
        private string insert_date;
        public string insertDate
        {
            get
            {
                DateTime dt = DateTime.Parse(insert_date);
                return dt.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
            }
            set { insert_date = value; }
        }
        private string update_date;
        public string updateDate
        {
            get
            {
                DateTime dt = DateTime.Parse(update_date);
                return dt.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
            }
            set { update_date = value; }
        }
        public int status { get; set; }

        public string getStatusText()
        {
            switch (status)
            {
                case 3:
                    return "Rejected";
                case 1:
                    return "Waiting for confirmation";
                case 2:
                    return "In Use";
                default:
                    return "";
            }
        }
    }
}