using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Helper
{
    public class MailReaderHelper
    {
        public string readEmailCreateAccount()
        {
            try
            {
                string path = HttpContext.Current.Server.MapPath("~/App_Data/CreateAccount.txt");//Path of the xml script  
                return File.ReadAllText(path); ;
            }
            catch (Exception e)
            {
                return "";
            }
        }
    }
}