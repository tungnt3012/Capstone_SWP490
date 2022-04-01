using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.mailModel
{
    public class EmailModel
    {
        public string toEmail { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public Dictionary<string, string> targetUrls { get; set; }
    }
}