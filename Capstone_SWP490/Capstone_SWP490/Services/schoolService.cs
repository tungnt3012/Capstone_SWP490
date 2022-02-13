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
    public class schoolService  : IschoolService
    {
        private readonly IschoolRepository _ischoolRepository = new schoolRepository();

        public async Task<int> deleteAsync(school school)
        {
            return await _ischoolRepository.Delete(school);
        }

        public List<school> findByCoachId(int coachId)
        {
            return _ischoolRepository.FindBy(x => x.coach_id == coachId).OrderByDescending(x=>x.update_date).ToList();
        }

        public school findById(int id)
        {
            return _ischoolRepository.FindBy(x => x.school_id == id).FirstOrDefault();
        }

        public async Task<school> insert(school school)
        {
            school existActive = _ischoolRepository.checkExist(school);
            if(existActive == null)
            {
                school.active = true;
            }
         return await _ischoolRepository.Create(school);
        }

    }
}