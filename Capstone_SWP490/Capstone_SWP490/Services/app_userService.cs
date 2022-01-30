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
        public List<app_user> GetAllUser()
        {
            return _iapp_UserRepository.GetAllApp_Users();
        }
        public Task<app_user> CreateUser(app_user userIn)
        {
            var userOut = _iapp_UserRepository.Create(userIn);
            if (userOut != null)
            {
                return userOut;
            }
            return null;
        }
    }
}