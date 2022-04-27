using Capstone_SWP490.Models.statisticViewModel;
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
        Task<int> update(school school);
        int count(int coach_id);
       int disableUsingStore(int schoolId);
        school getInConfirmation(int coachId);
        school findById(int schoolId);
        bool isExisted(string schoolName, string institutioName, int coachUserId);
        List<statistic_schoolViewModel> findSchoolConfirmation();
        int getRegistered();
        List<school> listRegistered();
        List<registered_school_ViewModel> listRegisteredSchool();
        int getTotalContestantInRegistered();
        Task<int> useSchool(int schoolId, int coachId);
        Task<int> processSchool(int schoolId, string type, string note);
        Task<int> removeSchool(int schoolId);

        bool isDuplicateSchool(school item);


        school findByNewRegistCoach(int coachId);

        List<team> GetTeams();
        Task<int> RemoveSchoolByCoach(int coachId, int currentInsertId);

        List<school> FindActive(int coachId);
    }
}
