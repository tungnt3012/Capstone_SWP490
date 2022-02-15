using Capstone_SWP490.DAO;
using Capstone_SWP490.ExceptionHandler;
using Capstone_SWP490.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Repositories
{
    public class teamRepository : GenericRepository<team>, IteamRepository
    {
        private readonly IschoolRepository _ischoolRepository = new schoolRepository();
        public team checkExist(team team)
        {
            team existedTeam = FindBy(x => x.team_name.Equals(team.team_name)).FirstOrDefault();
            if(existedTeam != null && existedTeam.school_id != team.school_id)
            {
                school existedSchool =_ischoolRepository.FindBy(x => x.school_id == existedTeam.school_id).FirstOrDefault();
                if(existedSchool != null)
                {
                    DateTime insertSchoolDate = DateTime.Parse(existedSchool.update_date);
                    DateTime now = DateTime.Now;
                    if(insertSchoolDate.Year != now.Year)
                    {
                        throw new TeamException("0", "Team '"+team.team_name+"' đã được đăng ký bởi trường "+ existedSchool.school_name + "(" + existedSchool.short_name +") " +
                            "vui lòng chọn tên khác" , null);
                    }
                }
            }
            return null;
        }
    }
}