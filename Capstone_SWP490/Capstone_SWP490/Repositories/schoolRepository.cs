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
        public school checkExist(school school)
        {
            if(school == null)
            {
                return null;
            }
            return FindBy(x => x.short_name == school.short_name && x.type == school.type && x.city == school.city).FirstOrDefault();
        }
    }
}