using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Constant.Enums
{
    public class SESSION_CONST
    {
      public class Registration {

            public static string REGISTRATION_ERROR_SESSION = "REGISTRATION_ERROR";
            public static string INSERT_ERROR = "IMPORT_ERROR";
            public static string READ_FILE_ERROR = "READ_FILE_ERROR";
            public static string SCHOOL_SESSION = "SCHOOL";
            public static string INSERT_RESULT = "INSERT_RESULT";
        }

        public class Global
        {
            public static readonly string LOGIN_SESSION = "profile";
        }
    }
}