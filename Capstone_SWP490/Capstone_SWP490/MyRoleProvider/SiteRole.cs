using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Security;

namespace Capstone_SWP490.MyRoleProvider
{
    public class SiteRole : RoleProvider
    {
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
            using (var usersContext = new gocyberx_icpcEntities())
            {
                return usersContext.app_user.Select(r => r.user_role).ToArray();
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            gocyberx_icpcEntities db = new gocyberx_icpcEntities();
            string data = db.app_user.Where(x => x.user_name == username).FirstOrDefault().user_role;
            string[] result = { data };
            return result;
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