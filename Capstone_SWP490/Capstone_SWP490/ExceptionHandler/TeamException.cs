﻿using Capstone_SWP490.ExceptionHandler.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.ExceptionHandler
{
     class TeamException : IException
    {
        public TeamException(string code, string message, Dictionary<string, object> param) : base(code, message, param)
        {
        }
    }
}