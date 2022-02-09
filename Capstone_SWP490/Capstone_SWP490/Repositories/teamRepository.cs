using Capstone_SWP490.DAO;
using Capstone_SWP490.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Repositories
{
    public class teamRepository : GenericRepository<team>, IteamRepository
    {
        public team checkExist(team team)
        {
            return FindBy(x => x.team_name.Equals(team.team_name)).FirstOrDefault();
        }
    }
}