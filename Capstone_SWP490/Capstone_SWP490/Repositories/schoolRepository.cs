using Capstone_SWP490.DAO;
using Capstone_SWP490.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Repositories
{
    public class schoolRepository : GenericRepository<school>, IschoolRepository
    {
        public school checkActive(school school)
        {
            if(school == null)
            {
                return null;
            }
            return FindBy(x => x.coach_id == school.coach_id && x.active == 3).FirstOrDefault();
        }
    }
}