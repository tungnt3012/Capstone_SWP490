﻿using Capstone_SWP490.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_SWP490.Repositories.Interfaces
{
    public interface Iapp_userRepository : IGenericRepository<app_user>
    {
        List<app_user> GetAllApp_Users();
    }
}
