using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Helper
{
    public class StringUtils
    {
        public static bool isNullOrEmpty(string text)
        {
            if(text == null)
            {
                return true;
            }
            if(text.Trim().Length == 0)
            {
                return true;
            }
            return false;
        }
    }
}