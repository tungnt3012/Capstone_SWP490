using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_SWP490.Services.Interfaces
{
    interface IteamService
    {
        Task<team> insert(team enties);
         Task<IEnumerable<team>> insertMany(IEnumerable<team> enties);
        List<team> findBySchoolId(int schoolId);
    }
}
