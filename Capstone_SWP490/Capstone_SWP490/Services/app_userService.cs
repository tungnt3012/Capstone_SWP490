using Capstone_SWP490.ExceptionHandler;
using Capstone_SWP490.Models.app_userViewModel;
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
    public class app_userService : Iapp_userService
    {

        private readonly Iapp_userRepository _iapp_UserRepository = new app_userRepository();
        private readonly ImemberRepository _imemberRepository = new memberRepository();

        public List<app_user> GetAllUser()
        {
            return _iapp_UserRepository.GetAllApp_Users();
        }
        public async Task<app_user> CreateUser(app_user userIn)
        {
            //if(_iapp_UserRepository.FindBy(x => x.user_name.Equals(userIn.user_name)) != null){
             //   throw new UserException("1","Email existed", null);
            //}
          return await _iapp_UserRepository.Create(userIn);
        }

        public bool CheckLogin(app_user app_User)
        {
            var u = _iapp_UserRepository.FindBy(x => x.user_name == app_User.user_name
                && x.psw == app_User.psw).FirstOrDefault();
            if (u != null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UpdatePasswordFirst(string username, string password, bool send_event)
        {
            var findUser = _iapp_UserRepository.FindBy(x => x.user_name == username && x.verified == false).FirstOrDefault();
            if (findUser != null)
            {
                var findMemberInfo = _imemberRepository.FindBy(x => x.user_id == findUser.user_id).FirstOrDefault();
                if (findMemberInfo == null)
                {
                    return false;
                }
                findMemberInfo.event_notify = send_event;
                if (await _imemberRepository.Update(findMemberInfo, findMemberInfo.member_id) != -1)
                {
                    findUser.psw = password;
                    findUser.verified = true;
                    var rsUpdate = await _iapp_UserRepository.Update(findUser, findUser.user_id);
                    if (rsUpdate != -1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<bool> UpdatePassword(string username, string password)
        {
            var findUser = _iapp_UserRepository.FindBy(x => x.user_name == username).FirstOrDefault();
            if (findUser != null)
            {
                findUser.psw = password;
                var rsUpdate = await _iapp_UserRepository.Update(findUser, findUser.user_id);
                if (rsUpdate != -1)
                {
                    return true;
                }
            }
            return false;
        }

        public app_userViewModel GetUserByUsername(string username)
        {
            var u = _iapp_UserRepository.FindBy(x => x.user_name == username).FirstOrDefault();
            if (u != null)
            {
                app_userViewModel viewModel = new app_userViewModel
                {
                    user_id = u.user_id,
                    user_name = u.user_name,
                    user_role = u.user_role,
                    verified = u.verified,
                    full_name = u.full_name,
                    email = u.email,
                    active = u.active
                };
                return viewModel;
            }
            return null;
        }

        public app_user getByUserId(int userId)
        {
            return  _iapp_UserRepository.FindBy(x => x.user_id == userId).FirstOrDefault();
        }

        public app_user getByUserId(int? userId)
        {
            throw new NotImplementedException();
        }

        public Task<int> delete(app_user entity)
        {
            return  _iapp_UserRepository.Delete(entity);
        }

        public Task<app_user> CreateuserForImportMember(app_user userIn, int coachId)
        {
            throw new NotImplementedException();
        }
    }
}