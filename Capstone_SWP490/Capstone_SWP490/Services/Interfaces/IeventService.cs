﻿using Capstone_SWP490.Models;
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
        AjaxResponseViewModel<IEnumerable<eventsViewModel>>  GetEventsByDate(DateTime fromDateIn, DateTime toDateIn);
        AjaxResponseViewModel<IEnumerable<eventsViewModel>> SearchEventActivities(DateTime fromDateIn, DateTime toDateIn);
        IEnumerable<string> GetAllSectionEvent();
        IEnumerable<eventsViewModel> GetAllEventsAvailale();
        IEnumerable<eventsViewModel> GetAllActivitiesAvailale();
        eventsViewModel GetEventsById(int id);
        List<eventsViewModel> GetSubEventsByEventId(int id);
        Task<eventsViewModel> UpdateEvent(eventsViewModel eventsIn);
        Task<eventsViewModel> CreateEvent(eventsViewModel eventsIn);
        Task<bool> DeleteEvent(int id);
    }
}
