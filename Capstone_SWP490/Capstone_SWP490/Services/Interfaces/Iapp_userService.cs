﻿using Capstone_SWP490.Models.app_userViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_SWP490.Services.Interfaces
{
    public interface Iapp_userService
    {
        List<app_user> GetAllUser();
        Task<app_user> CreateUser(app_user userIn);
        bool CheckLogin(app_user app_User);
        Task<bool> UpdatePasswordFirst(string username, string password, bool send_event);
        Task<bool> UpdatePassword(string username, string password);
        app_userViewModel GetUserByUsername(string username);

        Task<int> delete(app_user entity);
        bool isEmailInUse(string userName, int coachId);
        app_user getByUserName(string username);
        app_user getByUserId(int id);
        Task<app_user> creatUserForImportMember(app_user user, int coachId);
        Task<int> update(app_user user);

        List<app_user> findCoach(string status, string keyword);
    }
}
