using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Common.Const
{
    public class APP_CONST
    {
        public class ROLE
        {
            public static readonly string COACH = "COACH";
            public static readonly string MEMBER = "MEMBER";
            public static readonly string LEADER = "LEADER";
        }
        public class TEAM_ROLE
        {
            public static readonly string COACH_TEAM = "COACH-TEAM";
            public static readonly string NORMAL_TEAM = "NORMAL";
        }
        public static string ROOT = "ROOT";
        public static string MEMBER = "MEMBER";
        public static string SCHOOL = "SCHOOL";
        public static string TEAM = "TEAM";
    }
}