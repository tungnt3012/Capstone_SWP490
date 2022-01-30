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
    }
}
