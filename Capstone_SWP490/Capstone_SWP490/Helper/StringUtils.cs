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

        public static string upperCaseFirstCharacter(string text)
        {
            if (text.Length == 0)
            {
                return text;
            }

            string result = "";
            string[] arr = text.Trim().Split(' ');
            for (int i = 0; i < arr.Length; i++)
            {
                if (!arr[i].Equals(" ") && !arr[i].Equals(""))
                {
                    string upper = arr[i].ToUpper();
                    result += arr[i].Replace(arr[i].ElementAt(0), upper.ElementAt(0)) + " ";
                }
            }
            return result.Trim();
        }

        public static string extractFirstWordCharacter(string s)
        {
            if (s == null)
            {
                return "";
            }
            string result = "";
            string[] arr = s.Split(' ');
            foreach (string item in arr)
            {
                result += item.ElementAt(0);
            }
            return result.ToUpper();
        }
    }
}