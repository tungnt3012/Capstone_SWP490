﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_SWP490.Services.Interfaces
{
   public interface Icontest_memberService
    {
        Task<contest_member> insert(contest_member enties);
        Task<int> delete(contest_member entity);
        Task<int> deleteMany(IEnumerable<contest_member> entities);
    }
}
