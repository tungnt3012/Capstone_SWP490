using Capstone_SWP490.Repositories.Interfaces;
using Capstone_SWP490.Repositories;
using Capstone_SWP490.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Capstone_SWP490.ExceptionHandler;

namespace Capstone_SWP490.Services
{
    public class memberService : ImemberService
    {
        private readonly ImemberRepository _imemberRepository = new memberRepository();
        private readonly Iapp_userRepository _iapp_UserRepository = new app_userRepository();
        public async Task<member> insert(member member)
        {
            if (getByEmail(member.email) != null)
            {
                throw new MemberException("1","Email is used",null);
            }
          return await _imemberRepository.Create(member);
        }

        public async Task<IEnumerable<member>> insertMany(IEnumerable<member> member)
        {
            return await _imemberRepository.CreateMany(member);
        }

        public async Task<member> RegisterShirtSize(string username, string size)
        {
            var user = _iapp_UserRepository.FindBy(x => x.user_name.Equals(username)).FirstOrDefault();
            if (user != null)
            {
                var findMember = _imemberRepository.FindBy(x => x.user_id == user.user_id).FirstOrDefault();
                if (findMember != null)
                {
                    findMember.shirt_sizing = size;
                    if (await _imemberRepository.Update(findMember, findMember.member_id) != -1)
                    {
                        return findMember;
                    }
                }
            }
            return null;
        }

        public async Task<int> update(member member, int key)
        {
            return await _imemberRepository.Update(member, key);
        }
        public member getByEmail(string email)
        {
            return   _imemberRepository.FindBy(x => x.email == email).First();
        }
        public async Task<int> deleteAsync(member member)
        {
            return await _imemberRepository.Delete(member);
        }
    }
}