using Capstone_SWP490.DAO;
using Capstone_SWP490.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Repositories
{
    public class app_userRepository : GenericRepository<app_user>, Iapp_userRepository
    {
        public List<app_user> GetAllApp_Users()
        {
            return FindBy(x => x.user_id != 0).ToList();
        }
    }
}