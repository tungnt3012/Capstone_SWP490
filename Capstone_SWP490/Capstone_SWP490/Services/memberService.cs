﻿using Capstone_SWP490.Repositories.Interfaces;
using Capstone_SWP490.Repositories;
using Capstone_SWP490.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Capstone_SWP490.ExceptionHandler;
using log4net;
using Capstone_SWP490.Models;
using Capstone_SWP490.Models.statisticViewModel;

namespace Capstone_SWP490.Services
{
    public class memberService : ImemberService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(memberService));
        private readonly ImemberRepository _imemberRepository = new memberRepository();
        private readonly Iapp_userRepository _iapp_UserRepository = new app_userRepository();
        public async Task<member> insert(member member)
        {
            try
            {
                return await _imemberRepository.Create(member);
            }
            catch (Exception e)
            {
                if (e is MemberException)
                {
                    throw e;
                }
                member = null;
            }
            if (member == null)
            {
                throw new Exception("SYSTEM ERROR");
            }
            return member;
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

            return _imemberRepository.FindBy(x => x.email.Equals(email)).FirstOrDefault();
        }
        public async Task<int> deleteAsync(member member)
        {
            return await _imemberRepository.Delete(member);
        }

        public member GetMemberByUserId(int userId)
        {
            return _imemberRepository.FindBy(x => x.user_id == userId).FirstOrDefault();
        }

        public member GetMemberByUserId(int? userId)
        {
            return _imemberRepository.FindBy(x => x.user_id == userId).FirstOrDefault();
        }

        public async Task<AjaxResponseViewModel<bool>> JoinEvent(int id)
        {
            var output = new AjaxResponseViewModel<bool>
            {
                Status = 0,
                Data = false
            };
            var mem = _imemberRepository.FindBy(x => x.member_id == id).FirstOrDefault();
            if (mem != null)
            {
                mem.event_notify = true;
                if (await _imemberRepository.Update(mem, mem.member_id) != -1)
                {
                    output.Message = "success";
                    output.Data = true;
                    output.Status = 1;
                    return output;
                }
            }
            output.Message = "Fail";
            return output;
        }

        public member GetMemberByAvaibleUserId(int? userId)
        {
            return _imemberRepository.FindBy(x => x.user_id == userId && x.enabled == true).FirstOrDefault();
        }

        public statistic_shirtSizeViewModel statistic_ShirtSizeView()
        {
            var lstMember = _imemberRepository.FindBy(x => x.enabled == true && !String.IsNullOrEmpty(x.shirt_sizing)).ToList();
            return new statistic_shirtSizeViewModel
            {
                sizeS = _imemberRepository.FindBy(x => x.enabled == true && x.shirt_sizing.Equals("S")).Count(),
                sizeXS = _imemberRepository.FindBy(x => x.enabled == true && x.shirt_sizing.Equals("XS")).Count(),
                sizeM = _imemberRepository.FindBy(x => x.enabled == true && x.shirt_sizing.Equals("M")).Count(),
                sizeL = _imemberRepository.FindBy(x => x.enabled == true && x.shirt_sizing.Equals("L")).Count(),
                sizeXL = _imemberRepository.FindBy(x => x.enabled == true && x.shirt_sizing.Equals("XL")).Count(),
                size2XL = _imemberRepository.FindBy(x => x.enabled == true && x.shirt_sizing.Equals("2XL")).Count(),
                size3XL = _imemberRepository.FindBy(x => x.enabled == true && x.shirt_sizing.Equals("3XL")).Count(),
                lstMembers = lstMember,
                totalRegisterdSize = lstMember.Count(),
            };
        }
    }
}