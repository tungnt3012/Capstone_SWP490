﻿using Capstone_SWP490.Repositories;
using Capstone_SWP490.Repositories.Interfaces;
using Capstone_SWP490.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Capstone_SWP490.Services
{
    public class schoolService  : IschoolService
    {
        private readonly IschoolRepository _ischoolRepository = new schoolRepository();

        public async Task<school> insert(school school)
        {
         return await _ischoolRepository.Create(school);
        }
    }
}