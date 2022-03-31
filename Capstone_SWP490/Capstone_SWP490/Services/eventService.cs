using Capstone_SWP490.Models;
using Capstone_SWP490.Models.events_ViewModel;
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
    public class eventService : IeventService
    {
        private readonly IeventRepository _ieventRepository = new eventRepository();
        public IEnumerable<eventsViewModel> GetAllEventsAvailale()
        {
            var events = _ieventRepository.FindBy(x=>x.event_type!=0).ToList();
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
                        event_type = x.event_type,
                        fan_page = x.fan_page,
                        note = x.note,
                        shirt_id = x.shirt_id,
                        start_date = x.start_date,
                        end_date = x.end_date,
                        start_date_str= x.start_date.ToString("dd-MM-yyyy, HH:mm"),
                        end_date_str= x.end_date.ToString("dd-MM-yyyy, HH:mm"),
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

        public AjaxResponseViewModel<IEnumerable<eventsViewModel>> GetEventsByDate(DateTime fromDateIn, DateTime toDateIn)
        {
            var output = new AjaxResponseViewModel<IEnumerable<eventsViewModel>>
            {
                Status = 0,
                Data = null
            };
            DateTime temp = Convert.ToDateTime("01/01/0001");
            var events = new List<@event>();
            if (fromDateIn == temp && toDateIn == temp)
            {
                events = _ieventRepository.FindBy(x => x.event_type != 0).ToList();
            }
            if (fromDateIn == temp)
            {
                events = _ieventRepository.FindBy(x => x.end_date <= toDateIn&& x.event_type != 0).ToList();
            }
            if (toDateIn == temp)
            {
                events = _ieventRepository.FindBy(x => x.start_date >= fromDateIn && x.event_type != 0).ToList();
            } 
            if(fromDateIn!=temp && toDateIn != temp)
            {
                events = _ieventRepository.FindBy(x => x.start_date >= fromDateIn && x.end_date <= toDateIn && x.event_type != 0).ToList();
            }
            //var events = _ieventRepository.FindBy(x => x.start_date > fromDateIn && x.end_date < toDateIn).ToList();
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
                output.Message = "success";
                output.Data = lstEventsViewModels;
                output.Status = 1;
                return output;
            }
            output.Message = "Fail";
            return output;
        }

        public AjaxResponseViewModel<IEnumerable<eventsViewModel>> SearchEventActivities(DateTime fromDateIn, DateTime toDateIn)
        {
            var output = new AjaxResponseViewModel<IEnumerable<eventsViewModel>>
            {
                Status = 0,
                Data = null
            };
            DateTime temp = Convert.ToDateTime("01/01/0001");
            var events = new List<@event>();
            if (fromDateIn == temp && toDateIn == temp)
            {
                events = _ieventRepository.FindBy(x => x.event_type == 2).ToList();
            }
            if (fromDateIn == temp)
            {
                events = _ieventRepository.FindBy(x => x.end_date <= toDateIn && x.event_type == 2).ToList();
            }
            if (toDateIn == temp)
            {
                events = _ieventRepository.FindBy(x => x.start_date >= fromDateIn && x.event_type == 2).ToList();
            }
            if (fromDateIn != temp && toDateIn != temp)
            {
                events = _ieventRepository.FindBy(x => x.start_date >= fromDateIn && x.end_date <= toDateIn && x.event_type == 2).ToList();
            }
            //var events = _ieventRepository.FindBy(x => x.start_date > fromDateIn && x.end_date < toDateIn).ToList();
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
                output.Message = "success";
                output.Data = lstEventsViewModels;
                output.Status = 1;
                return output;
            }
            output.Message = "Fail";
            return output;
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
                    start_date_str = events.start_date.ToString("dd-MM-yyyy, HH:mm"),
                    end_date_str = events.end_date.ToString("dd-MM-yyyy, HH:mm"),
                };
            }
            return null;
        }

        public async Task<eventsViewModel> UpdateEvent(eventsViewModel eventsIn)
        {
            var e = _ieventRepository.FindBy(x => x.event_id == eventsIn.event_id).FirstOrDefault();
            if (e != null)
            {
                e.title = eventsIn.title;
                e.desctiption = eventsIn.desctiption;
                e.start_date = eventsIn.start_date;
                e.end_date = eventsIn.end_date;
                e.venue = eventsIn.venue;
                e.contactor_phone = eventsIn.contactor_phone;
                e.contactor_email = eventsIn.contactor_email;
                e.contactor_name = eventsIn.contactor_name;
                e.fan_page = eventsIn.fan_page;
                e.note = eventsIn.note;
                e.event_type = eventsIn.event_type;
                if (await _ieventRepository.Update(e, e.event_id) != -1)
                {
                    return new eventsViewModel
                    {
                        event_id = e.event_id,
                        contactor_email = e.contactor_phone,
                        contactor_name = e.contactor_name,
                        contactor_phone = e.contactor_phone,
                        desctiption = e.desctiption,
                        end_date = e.end_date,
                        event_type = e.event_type,
                        fan_page = e.fan_page,
                        note = e.note,
                        start_date = e.start_date,
                        title = e.title,
                        venue = e.venue,
                    };
                }
            };
            return null;
        }

        public async Task<eventsViewModel> CreateEvent(eventsViewModel eventsIn)
        {
            //var e = _ieventRepository.FindBy(x => x.event_id == eventsIn.event_id).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(eventsIn.title)
                && !string.IsNullOrWhiteSpace(eventsIn.desctiption)
                && !string.IsNullOrWhiteSpace(eventsIn.venue)
                && !string.IsNullOrWhiteSpace(eventsIn.fan_page)
                && Convert.ToDateTime("01/01/0001")!=eventsIn.start_date
                && Convert.ToDateTime("01/01/0001")!=eventsIn.end_date)
            {
                var e = new @event
                {
                    title = eventsIn.title,
                    desctiption = eventsIn.desctiption,
                    start_date = eventsIn.start_date + eventsIn.start_time,
                    end_date = eventsIn.end_date + eventsIn.end_time,
                    venue = eventsIn.venue,
                    fan_page = eventsIn.fan_page,
                    note = eventsIn.note ?? "",
                    event_type = 1,
                };

                var newEvent = await _ieventRepository.Create(e);
                if (newEvent != null)
                {
                    return new eventsViewModel
                    {
                        event_id = newEvent.event_id,
                        contactor_email = newEvent.contactor_phone,
                        contactor_name = newEvent.contactor_name,
                        contactor_phone = newEvent.contactor_phone,
                        desctiption = newEvent.desctiption,
                        end_date = newEvent.end_date,
                        event_type = newEvent.event_type,
                        fan_page = newEvent.fan_page,
                        note = newEvent.note,
                        start_date = newEvent.start_date,
                        title = newEvent.title,
                        venue = newEvent.venue,
                    };
                }
            }
            return null;
        }

        public async Task<bool> DeleteEvent(int id)
        {
            var e = _ieventRepository.FindBy(x => x.event_id == id).FirstOrDefault();
            if (e != null)
            {
                e.event_type = 0;
                if(await _ieventRepository.Update(e, e.event_id) != -1)
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<eventsViewModel> GetAllActivitiesAvailale()
        {
            var events = _ieventRepository.FindBy(x => x.event_type == 2).ToList();
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
                        event_type = x.event_type,
                        fan_page = x.fan_page,
                        note = x.note,
                        shirt_id = x.shirt_id,
                        start_date = x.start_date,
                        end_date = x.end_date,
                        start_date_str = x.start_date.ToString("dd-MM-yyyy"),
                        end_date_str = x.end_date.ToString("dd-MM-yyyy"),
                        title = x.title,
                        venue = x.venue,
                    };
                    lstEventsViewModels.Add(e);
                }
                return lstEventsViewModels;
            }
            return null;
        }
    }
}