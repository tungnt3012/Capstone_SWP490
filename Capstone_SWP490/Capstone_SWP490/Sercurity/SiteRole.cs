using Capstone_SWP490.Services;
using Capstone_SWP490.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Security;

namespace Capstone_SWP490.Sercurity
{
    public class SiteRole : RoleProvider
    {
        private readonly Iapp_userService _iapp_UserService = new app_userService();
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
#pragma warning disable CS0414 // The field 'SiteRole._cacheTimeoutInMinute' is assigned but its value is never used
        private int _cacheTimeoutInMinute = 20;
#pragma warning restore CS0414 // The field 'SiteRole._cacheTimeoutInMinute' is assigned but its value is never used

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            string[] AppRole = new string[] { "ADMIN", "COACH", "MEMBER", "ORGANIZER", "GUEST", "VICE-COACH" };
            return AppRole;
        }

        public override string[] GetRolesForUser(string username)
        {
            return new string[] { _iapp_UserService.getByUserName(username).user_role };
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            var userRoles = GetRolesForUser(username);
            return userRoles.Contains(roleName);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}