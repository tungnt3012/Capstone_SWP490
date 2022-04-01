using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Capstone_SWP490.Services.Interfaces
{
    public interface Iteam_memberService
    {
        Task<team_member> insert(team_member team_Member);
        List<team_member> getCoachTeamMember(int? teamId);
        Task<int> delete(team_member entity);
    }
}