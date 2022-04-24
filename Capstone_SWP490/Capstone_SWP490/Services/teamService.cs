using Capstone_SWP490.Constant.Const;
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

        public Task<int> delete(team team)
        {
            return _iteamRepository.Delete(team);
        }

        public List<team> findBySchoolId(int schoolId)
        {
            return _iteamRepository.FindBy(x => x.school_id == schoolId).ToList();
        }

        public async Task<team> insert(team enties)
        {
            if (!enties.type.Equals(APP_CONST.TEAM_ROLE.COACH_TEAM))
            {
                try
                {
                    team existed = _iteamRepository.checkExist(enties);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            return await _iteamRepository.Create(enties);
        }

        public async Task<IEnumerable<team>> insertMany(IEnumerable<team> enties)
        {
            List<team> beforeCreate = new List<team>();
            foreach (var x in enties)
            {
                if (_iteamRepository.checkExist(x) != null)
                {
                    beforeCreate.Add(x);
                }
            }
            return await _iteamRepository.CreateMany(beforeCreate);
        }

        public Task<int> update(team team)
        {
            return _iteamRepository.Update(team, team.team_id);
        }

        public IEnumerable<team> findRegistedTeam(int coachId)
        {
            return _iteamRepository.FindBy(x => x.school.active == 2 && x.school.coach_id != coachId).ToList();
        }

        public IEnumerable<team> getAllTeams()
        {
            return _iteamRepository.FindBy(x => x.enabled == true).ToList();
        }

        public team FindByTeamName(string teamName)
        {
            return _iteamRepository.FindBy(x => x.team_name.ToUpper().Equals(teamName.ToUpper())).FirstOrDefault();
        }
    }
}