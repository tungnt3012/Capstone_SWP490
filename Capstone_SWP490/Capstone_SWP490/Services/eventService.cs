using Capstone_SWP490.Models.events_ViewModel;
using Capstone_SWP490.Repositories;
using Capstone_SWP490.Repositories.Interfaces;
using Capstone_SWP490.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Services
{
    public class eventService : IeventService
    {
        private readonly IeventRepository _ieventRepository = new eventRepository();
        public IEnumerable<eventsViewModel> GetAllEvents()
        {
            var events = _ieventRepository.GetAll();
            var lstEventsViewModels = new List<eventsViewModel>();
            if (events != null)
            {
                foreach (var x in events)
                {
                    var e = new eventsViewModel
                    {
                        event_id = x.event_id,
                        contactor_email = x.contactor_phone,
                        contactor_name = x.contactor_name,
                        contactor_phone = x.contactor_phone,
                        desctiption = x.desctiption,
                        end_date = x.end_date,
                        event_type = x.event_type,
                        fan_page = x.fan_page,
                        note = x.note,
                        shirt_id = x.shirt_id,
                        start_date = x.start_date,
                        title = x.title,
                        venue = x.venue,
                    };
                    lstEventsViewModels.Add(e);
                }
                return lstEventsViewModels;
            }
            return null;
        }

        public IEnumerable<string> GetAllSectionEvent()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<eventsViewModel> GetEventsByDate(DateTime fromDateIn, DateTime toDateIn)
        {
            throw new NotImplementedException();
        }

        public eventsViewModel GetEventsById(int id)
        {
            var events = _ieventRepository.FindBy(x => x.event_id == id).FirstOrDefault();
            if (events != null)
            {
                return new eventsViewModel
                {
                    event_id = events.event_id,
                    contactor_email = events.contactor_phone,
                    contactor_name = events.contactor_name,
                    contactor_phone = events.contactor_phone,
                    desctiption = events.desctiption,
                    end_date = events.end_date,
                    event_type = events.event_type,
                    fan_page = events.fan_page,
                    note = events.note,
                    shirt_id = events.shirt_id,
                    start_date = events.start_date,
                    title = events.title,
                    venue = events.venue,
                };
            }
            return null;
        }
    }
}