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
    public class teamMemberService : Iteam_memberService
    {
        private readonly Iteam_memberRepository _iteam_memberRepository = new teamMemberRepository();

        public async Task<team_member> insert(team_member team_Member)
        {
            return await _iteam_memberRepository.Create(team_Member);
        }
    }
}