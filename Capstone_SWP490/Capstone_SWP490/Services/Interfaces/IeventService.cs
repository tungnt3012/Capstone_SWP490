using Capstone_SWP490.Models;
using Capstone_SWP490.Models.app_userViewModel;
using Capstone_SWP490.Models.events_ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_SWP490.Services.Interfaces
{
    public interface IeventService
    {
        AjaxResponseViewModel<IEnumerable<eventsMainViewModel>>  GetEventsByDate(DateTime fromDateIn, DateTime toDateIn);
        AjaxResponseViewModel<IEnumerable<eventsViewModel>> SearchEventActivities(DateTime fromDateIn, DateTime toDateIn);
        IEnumerable<string> GetAllSectionEvent();
        IEnumerable<eventsMainViewModel> GetAllEventsAvailale();
        IEnumerable<eventsViewModel> GetAllActivitiesAvailale();
        eventsViewModel GetEventsById(int id);
        List<eventsViewModel> GetSubEventsByEventId(int id);
        List<eventsViewModel> GetSubEventsByEventIdAndUser(int id, int userId);
        Task<eventsViewModel> UpdateEvent(eventsViewModel eventsIn);
        Task<eventsMainCreateViewModel> CreateEvent(eventsMainCreateViewModel eventsIn);
        Task<eventsViewModel> CreateSubEvent(eventsViewModel eventsIn);
        Task<bool> DeleteEvent(int id);
        Task<AjaxResponseViewModel<bool>> JoinSubEvent(int eventId, int userId);
        bool IsUserJoinEvent(int eventId, int userId);
        int CountMemberJoinEvent(int eventId);
        List<eventsViewModel> GetTop8Event();
        List<eventsViewModel> EventStatic();
        List<app_userViewModel> AllUsersInEvent(int eventId);
        AjaxResponseViewModel<bool> SendNotiNewEvent(int eventId);

    }
}
