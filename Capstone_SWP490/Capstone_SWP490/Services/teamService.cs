using Capstone_SWP490.ExceptionHandler;
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

        public List<team> findBySchoolId(int schoolId)
        {
            return _iteamRepository.FindBy(x => x.school_id == schoolId).ToList();
        }

        public async Task<team> insert(team enties)
        {
            if(_iteamRepository.checkExist(enties) != null)
            {
                throw new TeamException("0", "Team Existed !", null);
            }
         return await _iteamRepository.Create(enties);
        }

        public async Task<IEnumerable<team>> insertMany(IEnumerable<team> enties)
        {
            List<team> beforeCreate = new List<team>();
            foreach(var x in enties)
            {
                if(_iteamRepository.checkExist(x) != null)
                {
                    beforeCreate.Add(x);
                }
            }
          return await _iteamRepository.CreateMany(beforeCreate);
        }
    }
}