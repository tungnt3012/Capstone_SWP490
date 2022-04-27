using Capstone_SWP490.ExceptionHandler;
using Capstone_SWP490.Helper;
using Capstone_SWP490.Models;
using Capstone_SWP490.Models.app_userViewModel;
using Capstone_SWP490.Repositories;
using Capstone_SWP490.Repositories.Interfaces;
using Capstone_SWP490.Services.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Capstone_SWP490.Services
{
    public class app_userService : Iapp_userService
    {

        private readonly Iapp_userRepository _iapp_UserRepository = new app_userRepository();
        private readonly ImemberRepository _imemberRepository = new memberRepository();
        private static readonly ILog Log = LogManager.GetLogger(typeof(app_userService));
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
            string pass = app_User.psw;
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] data = md5.ComputeHash(utf8.GetBytes(pass));
            var passConverted = Convert.ToBase64String(data);

            var u = _iapp_UserRepository.FindBy(x => x.user_name == app_User.user_name
                && x.encrypted_psw == passConverted).FirstOrDefault();
            if (u != null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UpdatePasswordFirst(string username, string password, string passwordEncrypted, bool send_event)
        {
            var findUser = _iapp_UserRepository.FindBy(x => x.user_name == username && x.verified == false).FirstOrDefault();
            if (findUser != null)
            {
                if (findUser.psw.Equals(password))
                {
                    return false;
                }
                var findMemberInfo = _imemberRepository.FindBy(x => x.user_id == findUser.user_id && x.enabled == true).FirstOrDefault();
                if (findMemberInfo == null)
                {
                    return false;
                }
                findMemberInfo.event_notify = send_event;
                if (await _imemberRepository.Update(findMemberInfo, findMemberInfo.member_id) != -1)
                {
                    findUser.psw = password;
                    findUser.encrypted_psw = passwordEncrypted;
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

        public async Task<bool> UpdatePassword(string username, string password, string passwordEncrypted)
        {
            var findUser = _iapp_UserRepository.FindBy(x => x.user_name == username).FirstOrDefault();
            if (findUser != null)
            {
                if (findUser.psw.Equals(password))
                {
                    return false;
                }
                findUser.psw = password;
                findUser.encrypted_psw = passwordEncrypted;
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
                    active = u.active,
                    psw = u.psw,
                    encrypted_psw = u.encrypted_psw,
                    confirm_password = u.confirm_password
                };
                return viewModel;
            }
            return null;
        }

        public app_user getByUserId(int userId)
        {
            return _iapp_UserRepository.FindBy(x => x.user_id == userId).FirstOrDefault();
        }

        public app_user getByUserName(string username)
        {
            return _iapp_UserRepository.FindBy(x => x.user_name == username).FirstOrDefault();
        }

        public Task<int> delete(app_user entity)
        {
            return _iapp_UserRepository.Delete(entity);
        }

        public bool isEmailInUse(string userName, int coachId)
        {
            List<int?> existed = _iapp_UserRepository.getContext().Check_Mail_In_Use(userName, coachId).ToList();
            if (existed.FirstOrDefault() >= 1)
            {
                return true;
            }
            return false;
        }

        public Task<app_user> creatUserForImportMember(app_user user)
        {
            return CreateUser(user);
        }

        public async Task<int> update(app_user user)
        {
            user.update_date = DateTime.Now + "";
            return await _iapp_UserRepository.Update(user, user.user_id);
        }

        public List<app_user> findCoach(string status, string keyword)
        {
            if (keyword == null)
            {
                keyword = "";
            }
            List<app_user> result;
            status = status.ToUpper();
            if (status != null && status.Equals("ALL"))
            {
                result = _iapp_UserRepository.FindBy(x => x.user_role.Equals("COACH") && x.update_date != null && (x.email.Contains(keyword) || x.full_name.Contains(keyword))).ToList();
            }
            else if (status.Equals("ENABLED"))
            {
                result = _iapp_UserRepository.FindBy(x => x.user_role.Equals("COACH") && x.update_date != null && x.active == true && (x.email.Contains(keyword) || x.full_name.Contains(keyword))).ToList();
            }
            else
            {
                result = _iapp_UserRepository.FindBy(x => x.user_role.Equals("COACH") && x.update_date != null && x.active == false && (x.email.Contains(keyword) || x.full_name.Contains(keyword))).OrderBy(x => x.active).ToList();
            }

            var lstTemp = new List<app_userViewModel>();
            var app_userTemp = new List<app_user>();
            if (result.Count >= 1)
            {
                foreach (var x in result)
                {
                    var newU = new app_userViewModel
                    {
                        active = x.active,
                        email = x.email,
                        full_name = x.full_name,
                        user_id = x.user_id,
                        user_name = x.user_name,
                        user_role = x.user_role,
                        verified = x.verified,
                        insert_date = Convert.ToDateTime(x.insert_date),
                        update_date = Convert.ToDateTime(x.update_date)
                    };
                    lstTemp.Add(newU);
                }

                var tmp = (from u in lstTemp
                           orderby u.update_date descending
                           select u).ToList();

                foreach (var s in tmp)
                {
                    var newUTemp = new app_user
                    {
                        active = s.active,
                        email = s.email,
                        full_name = s.full_name,
                        user_id = s.user_id,
                        user_name = s.user_name,
                        user_role = s.user_role,
                        verified = s.verified,
                        insert_date = Convert.ToString(s.insert_date),
                        update_date = Convert.ToString(s.update_date)
                    };
                    app_userTemp.Add(newUTemp);
                }
            }

            return app_userTemp;
            //return result.OrderBy(x => x.update_date).ToList();
        }

        public PagingOutput<List<app_userViewModel>> GetListUsersManager(int userCrr)
        {
            var allUser = _iapp_UserRepository.FindBy(x => x.user_id != userCrr).ToList();
            if (allUser != null)
            {
                var users = allUser.Select(x => new app_userViewModel()
                {
                    user_id = x.user_id,
                    active = x.active,
                    email = x.email,
                    encrypted_psw = x.encrypted_psw,
                    full_name = x.full_name,
                    psw = x.psw,
                    repsw = x.psw,
                    user_name = x.user_name,
                    user_role = x.user_role,
                    verified = x.verified
                }).ToList();
                //int totalPage = allUser.Count / pageSize;
                //if (allUser.Count % pageSize > 0)
                //{
                //    totalPage = (allUser.Count / pageSize) + 1;
                //}
                var paging = new PagingOutput<List<app_userViewModel>>
                {
                    Data = users,
                    //Index = pageIndex,
                    //PageSize = pageSize,
                    TotalItem = allUser.Count,
                    //TotalPage = totalPage
                };
                return paging;
            }
            return null;
        }

        public async Task<bool> SwitchableUsers(int user_id, bool status)
        {
            var user = _iapp_UserRepository.FindBy(x => x.user_id == user_id).FirstOrDefault();
            if (user != null)
            {
                user.active = status;
                if (await _iapp_UserRepository.Update(user, user.user_id) != -1)
                {
                    return true;
                }
            }
            return false;
        }

        public List<app_user> findNewRegistCoach(string keyword)
        {
            if (keyword == null)
            {
                keyword = "";
            }
            List<app_user> result = _iapp_UserRepository.FindBy(x => x.user_role.Equals("COACH") && x.update_date == null && (x.email.Contains(keyword) || x.full_name.Contains(keyword))).ToList();

            var lstTemp = new List<app_userViewModel>();
            var app_userTemp = new List<app_user>();
            if (result.Count >= 1)
            {
                foreach (var x in result)
                {
                    var newU = new app_userViewModel
                    {
                        active = x.active,
                        email = x.email,
                        full_name = x.full_name,
                        user_id = x.user_id,
                        user_name = x.user_name,
                        user_role = x.user_role,
                        verified = x.verified,
                        insert_date = Convert.ToDateTime(x.insert_date),
                        update_date = Convert.ToDateTime(x.update_date)
                    };
                    lstTemp.Add(newU);
                }

                var tmp = (from u in lstTemp
                           orderby u.update_date descending
                           select u).ToList();

                foreach (var s in tmp)
                {
                    var newUTemp = new app_user
                    {
                        active = s.active,
                        email = s.email,
                        full_name = s.full_name,
                        user_id = s.user_id,
                        user_name = s.user_name,
                        user_role = s.user_role,
                        verified = s.verified,
                        insert_date = Convert.ToString(s.insert_date),
                        update_date = Convert.ToString(s.update_date)
                    };
                    app_userTemp.Add(newUTemp);
                }
            }

            return app_userTemp;
        }

        public async Task<bool> ForgotPassword(string email)
        {
            var u = _iapp_UserRepository.FindBy(x => x.email.Equals(email)).FirstOrDefault();
            if (u != null)
            {
                u.psw = Helper.CommonHelper.CreatePassword(8);
                u.encrypted_psw = Helper.CommonHelper.createEncryptedPassWord(u.psw);
                u.confirm_password = 0;
                if (await _iapp_UserRepository.Update(u, u.user_id) != -1)
                {
                    new MailHelper().sendMailForgotPassword(u);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> ResetPassword(string username, string newPass)
        {
            var u = _iapp_UserRepository.FindBy(x => x.user_name.Equals(username)).FirstOrDefault();
            if (u != null)
            {
                if (u.psw.Equals(newPass))
                {
                    return false;
                }
                u.psw = newPass;
                u.encrypted_psw = CommonHelper.createEncryptedPassWord(u.psw);
                u.confirm_password = 1;
                u.verified = true;
                if (await _iapp_UserRepository.Update(u, u.user_id) != -1)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<app_user> CreateOrganizer(app_user userIn)
        {
            if (!String.IsNullOrWhiteSpace(userIn.full_name) && !String.IsNullOrWhiteSpace(userIn.email))
            {
                var u = _iapp_UserRepository.FindBy(x => x.email == userIn.email).FirstOrDefault();
                if (u == null)
                {
                    string pwRandom = CommonHelper.CreatePassword(8);
                    var userTemp = new app_user
                    {
                        user_name = userIn.email,
                        full_name = userIn.full_name,
                        email = userIn.email,
                        user_role = "ORGANIZER",
                        psw = pwRandom,
                        encrypted_psw = CommonHelper.createEncryptedPassWord(pwRandom),
                        verified = false,
                        active = true,
                        insert_date = DateTime.Now.ToString(),
                        update_date = DateTime.Now.ToString(),
                        confirm_password = 0
                    };
                    var uChecker = await _iapp_UserRepository.Create(userTemp);
                    if (uChecker != null)
                    {
                        new MailHelper().sendMailNewOrganizerAccount(uChecker);
                        return uChecker;
                    }
                }
            }
            return null;
        }
    }
}