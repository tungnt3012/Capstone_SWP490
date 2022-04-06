using Capstone_SWP490.ExceptionHandler;
using Capstone_SWP490.Repositories;
using Capstone_SWP490.Repositories.Interfaces;
using Capstone_SWP490.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Capstone_SWP490.Services
{
    public class schoolService : IschoolService
    {
        private readonly IschoolRepository _ischoolRepository = new schoolRepository();

        public int count(int coach_id)
        {
            List<school> schools = _ischoolRepository.FindBy(x => x.coach_id == coach_id).ToList();
            if (schools != null)
            {
                return schools.Count;
            }
            return 0;
        }

        public async Task<int> deleteAsync(school school)
        {
            return await _ischoolRepository.Delete(school);
        }

        public void disable(school school)
        {
            _ischoolRepository.Update(school, school.school_id);
        }

        public int disableUsingStore(int schoolId)
        {
            return _ischoolRepository.getContext().Disable_School_Data(schoolId);
        }

        public List<school> findByCoachId(int coachId)
        {
            return _ischoolRepository.FindBy(x => x.coach_id == coachId && x.enabled == true).OrderByDescending(x => x.update_date).ToList();
        }

        public school findActiveById(int id)
        {
            return _ischoolRepository.FindBy(x => x.school_id == id && x.enabled == true).FirstOrDefault();
        }
        public school findInUsing(int coachId)
        {
            return _ischoolRepository.FindBy(x => x.coach_id == coachId && x.active == 3).FirstOrDefault();
        }
        public async Task<school> insert(school school)
        {
            school existActive = _ischoolRepository.checkActive(school);
            school.active = (existActive == null) ? 2 : 1;
            try
            {
                return await _ischoolRepository.Create(school);
            }
            catch (Exception e)
            {
                if (e is SchoolException)
                {
                    throw e;
                }
                school = null;
            }
            if (school == null)
            {
                throw new Exception("SYSTEM ERROR");
            }
            return school;
        }

        public Task<int> update(school school)
        {
            return _ischoolRepository.Update(school, school.school_id);
        }

        public school findById(int schoolId)
        {
            return _ischoolRepository.FindBy(x => x.coach_id == schoolId).FirstOrDefault();
        }

        public bool isExisted(string schoolName, string institutioName, int coachUserId)
        {
            return _ischoolRepository.FindBy(x => x.coach_id != coachUserId && x.active == 3
            && x.school_name.Equals(schoolName)
            && x.institution_name.Equals(institutioName)).FirstOrDefault() != null;
        }
        public school getFirstRegistSchool(int coachUserId)
        {
            return _ischoolRepository.FindBy(x => x.coach_id == coachUserId && x.active == 0).FirstOrDefault();
        }
    }
}