﻿using Capstone_SWP490.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_SWP490.Repositories.Interfaces
{
   public interface IschoolRepository : IGenericRepository<school>
    {
        school checkActive(school school);
    }
}
