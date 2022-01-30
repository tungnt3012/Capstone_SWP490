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
    public class teamService : IteamService
    {
        private readonly IteamRepository _iteamRepository = new teamRepository();

        public async Task<team> insert(team enties)
        {
         return await _iteamRepository.Create(enties);
        }

        public async Task<IEnumerable<team>> insertMany(IEnumerable<team> enties)
        {
          return await _iteamRepository.CreateMany(enties);
        }
    }
}