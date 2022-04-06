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
        school findActiveById(int id);

        void disable(school school);
        int count(int coach_id);
       int disableUsingStore(int schoolId);
        school findInUsing(int coachId);
        Task<int> update(school school);
        school findById(int schoolId);
        bool isExisted(string schoolName, string institutioName, int coachUserId);
        school getFirstRegistSchool(int coachUserId);
    }
}
