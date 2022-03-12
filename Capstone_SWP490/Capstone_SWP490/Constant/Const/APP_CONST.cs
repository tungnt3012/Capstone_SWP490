﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Constant.Const
{
    public class APP_CONST
    {
        public class SCHOOL_ROLE
        {
            private static readonly string COACH = "COACH";
            private static readonly string VICE_COACH = "VICE-COACH";
            private static readonly string MEMBER = "MEMBER";
            private static readonly string LEADER = "LEADER";

            public static string getRole(int roleInt)
            {
                switch (roleInt)
                {
                    case 1:
                        return COACH;
                    case 2:
                        return VICE_COACH;
                    case 3:
                        return LEADER;
                    case 4:
                        return MEMBER;
                    default:
                        return "";
                }
            }
        }

        public class APP_ROLE
        {
            private static readonly string COACH = "COACH";
            private static readonly string VICE_COACH = "VICE-COACH";
            private static readonly string MEMBER = "MEMBER";
            private static readonly string LEADER = "LEADER";
            private static readonly string ORGANIZATION = "ORGANIZATION";
            private static readonly string ADMIN = "ADMIN";
            public static string getUserRole(int roleInt)
            {
                switch (roleInt)
                {
                    case 1:
                        return COACH;
                    case 2:
                        return VICE_COACH;
                    default:
                        return MEMBER;
                }
            }

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