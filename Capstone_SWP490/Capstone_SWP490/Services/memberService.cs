using Capstone_SWP490.Repositories.Interfaces;
using Capstone_SWP490.Repositories;
using Capstone_SWP490.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Capstone_SWP490.Services
{
    public class memberService : ImemberService
    {
        private readonly ImemberRepository _imemberRepository = new memberRepository();
        public async Task<member> insert(member member)
        {
          return await _imemberRepository.Create(member);
        }

        public async Task<IEnumerable<member>> insertMany(IEnumerable<member> member)
        {
            return await _imemberRepository.CreateMany(member);
        }

        public async Task<int> update(member member, int key)
        {
            return await _imemberRepository.Update(member, key);
        }
    }
}