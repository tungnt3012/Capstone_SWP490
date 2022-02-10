using Capstone_SWP490.Repositories;
using Capstone_SWP490.Repositories.Interfaces;
using Capstone_SWP490.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Capstone_SWP490.Services
{
    public class contest_memberService : Icontest_memberService
    {
        private readonly Icontest_memberRepository _icontest_memberRepository = new contest_memberRepository();
        public async Task<contest_member> insert(contest_member enties)
        {
         return await _icontest_memberRepository.Create(enties);
        }
    }
}