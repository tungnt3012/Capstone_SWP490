﻿using Capstone_SWP490.Models.events_ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_SWP490.Services.Interfaces
{
    public interface IeventService
    {
        IEnumerable<eventsViewModel> GetEventsByDate(DateTime fromDateIn, DateTime toDateIn);
        IEnumerable<string> GetAllSectionEvent();
        IEnumerable<eventsViewModel> GetAllEvents();
        eventsViewModel GetEventsById(int id);
    }
}
