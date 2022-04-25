﻿using Capstone_SWP490.DAO;
using Capstone_SWP490.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Repositories
{
    public class contestRepository : GenericRepository<contest>, IcontestRepository
    {
        public contest getByCode(string code)
        {
           return FindBy(x => x.code.Trim().ToUpper().Equals(code.Trim())).First();
        }
    }
}