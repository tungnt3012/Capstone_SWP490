using Capstone_SWP490.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Repositories.Interfaces
{
    public interface IteamRepository : IGenericRepository<team>
    {
        team checkExist(team team);
    }
}