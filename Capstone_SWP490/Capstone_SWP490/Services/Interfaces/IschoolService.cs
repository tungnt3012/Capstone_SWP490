using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_SWP490.Services.Interfaces
{
   public interface IschoolService
    {
        Task<school> insert(school school);
        Task<int> deleteAsync(school school);
        List<school> findByCoachId(int coachId);
        school findById(int id);

    }
}
