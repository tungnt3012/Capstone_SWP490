using Capstone_SWP490.Helper;
using Capstone_SWP490.Models;
using Capstone_SWP490.Models.app_userViewModel;
using Capstone_SWP490.Models.events_ViewModel;
using Capstone_SWP490.Models.statisticViewModel;
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
        private readonly ImemberRepository _imemberRepository = new memberRepository();
        private readonly Iapp_userRepository _iapp_userRepository = new app_userRepository();

        public IEnumerable<eventsMainViewModel> GetAllEventsAvailale()
        {
            var events = _ieventRepository.FindBy(x => x.event_type == 1 && x.status != -1).ToList();
            var lstEventsMainViewModels = new List<eventsMainViewModel>();
            if (events != null)
            {
                foreach (var x in events)
                {
                    var e = new eventsMainViewModel
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
                        subEvent = GetSubEventsByEventId(x.event_id)
                    };
                    lstEventsMainViewModels.Add(e);
                }
                return lstEventsMainViewModels;
            }
            return null;
        }

        public IEnumerable<string> GetAllSectionEvent()
        {
            throw new NotImplementedException();
        }

        public AjaxResponseViewModel<IEnumerable<eventsMainViewModel>> GetEventsByDate(DateTime fromDateIn, DateTime toDateIn)
        {
            var output = new AjaxResponseViewModel<IEnumerable<eventsMainViewModel>>
            {
                Status = 0,
                Data = null
            };
            DateTime temp = Convert.ToDateTime("01/01/0001");
            if (toDateIn != temp)
            {
                toDateIn += new TimeSpan(23, 59, 59);
            }
            var events = new List<@event>();
            if (fromDateIn == temp && toDateIn == temp)
            {
                events = _ieventRepository.FindBy(x => x.event_type == 1 && x.status != -1).ToList();
            }
            if (fromDateIn == temp)
            {
                events = _ieventRepository.FindBy(x => x.end_date <= toDateIn && x.event_type == 1 && x.status != -1).ToList();
            }
            if (toDateIn == temp)
            {
                events = _ieventRepository.FindBy(x => x.start_date >= fromDateIn && x.event_type == 1 && x.status != -1).ToList();
            }
            if (fromDateIn != temp && toDateIn != temp)
            {
                events = _ieventRepository.FindBy(x => x.start_date >= fromDateIn && x.end_date <= toDateIn && x.event_type == 1 && x.status != -1).ToList();
            }
            //ONLY Search Main-Event
            var lstEventsMainViewModels = new List<eventsMainViewModel>();
            if (events != null)
            {
                foreach (var x in events)
                {
                    var e = new eventsMainViewModel
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
                        subEvent = GetSubEventsByEventId(x.event_id)
                    };
                    lstEventsMainViewModels.Add(e);
                }
                output.Message = "success";
                output.Data = lstEventsMainViewModels;
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
            //ONLY Search Sub-Event (Schedule of day)
            var lstEventsViewModels = new List<eventsViewModel>();
            if (events != null)
            {
                foreach (var x in events)
                {
                    var e = new eventsViewModel
                    {
                        event_id = x.event_id,
                        desctiption = x.desctiption,
                        end_date = x.end_date,
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
                var eOutput = new eventsViewModel
                {
                    event_id = events.event_id,
                    event_type = events.event_type,
                    desctiption = events.desctiption,
                    end_date = events.end_date,
                    fan_page = events.fan_page,
                    note = events.note,
                    shirt_id = events.shirt_id,
                    start_date = events.start_date,
                    title = events.title,
                    venue = events.venue,
                    status = events.status,
                    contactor_phone = events.contactor_phone,
                    contactor_email = events.contactor_email,
                    contactor_name = events.contactor_name,
                    start_date_str = events.start_date.ToString("dd-MM-yyyy, HH:mm"),
                    end_date_str = events.end_date.ToString("dd-MM-yyyy, HH:mm"),
                };
                var mainOfE = _ieventRepository.FindBy(x => x.sub_event.Contains(eOutput.event_id.ToString())).FirstOrDefault();
                if (mainOfE != null)
                {
                    eOutput.main_event_id = mainOfE.event_id;
                }
                return eOutput;
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
                e.start_date = eventsIn.start_date + eventsIn.start_time;
                e.end_date = eventsIn.end_date + eventsIn.end_time;
                e.venue = eventsIn.venue;
                e.fan_page = eventsIn.fan_page;
                e.note = eventsIn.note;
                e.status = eventsIn.status;
                e.contactor_name = eventsIn.contactor_name;
                e.contactor_email = eventsIn.contactor_email;
                e.contactor_phone = eventsIn.contactor_phone;
                if (await _ieventRepository.Update(e, e.event_id) != -1)
                {
                    var eOutput = new eventsViewModel
                    {
                        event_id = e.event_id,
                        desctiption = e.desctiption,
                        event_type = e.event_type,
                        end_date = e.end_date,
                        fan_page = e.fan_page,
                        note = e.note,
                        start_date = e.start_date,
                        title = e.title,
                        venue = e.venue,
                        status = e.status,
                        contactor_phone = e.contactor_phone,
                        contactor_email = e.contactor_email,
                        contactor_name = e.contactor_name
                    };

                    var mainOfE = _ieventRepository.FindBy(x => x.sub_event.Contains(eOutput.event_id.ToString())).FirstOrDefault();
                    if (mainOfE != null)
                    {
                        eOutput.main_event_id = mainOfE.event_id;
                    }
                    return eOutput;
                }
            };
            return null;
        }

        public async Task<eventsMainCreateViewModel> CreateEvent(eventsMainCreateViewModel eventsIn)
        {
            //var e = _ieventRepository.FindBy(x => x.event_id == eventsIn.event_id).FirstOrDefault();

            if ((!string.IsNullOrWhiteSpace(eventsIn.title)
                && !string.IsNullOrWhiteSpace(eventsIn.desctiption)
                && !string.IsNullOrWhiteSpace(eventsIn.venue)
                && !string.IsNullOrWhiteSpace(eventsIn.fan_page)
                && Convert.ToDateTime("01/01/0001") != eventsIn.start_date)
                && (!string.IsNullOrWhiteSpace(eventsIn.subEvent.title)
                && !string.IsNullOrWhiteSpace(eventsIn.subEvent.desctiption)
                && !string.IsNullOrWhiteSpace(eventsIn.subEvent.venue)
                && !string.IsNullOrWhiteSpace(eventsIn.subEvent.fan_page)
                && Convert.ToDateTime("01/01/0001") != eventsIn.subEvent.start_date
                && Convert.ToDateTime("01/01/0001") != eventsIn.subEvent.end_date))
            {

                var se = new @event
                {
                    title = eventsIn.subEvent.title,
                    desctiption = eventsIn.subEvent.desctiption,
                    start_date = eventsIn.subEvent.start_date + eventsIn.subEvent.start_time,
                    end_date = eventsIn.subEvent.end_date + eventsIn.subEvent.end_time,
                    venue = eventsIn.subEvent.venue,
                    fan_page = eventsIn.subEvent.fan_page,
                    contactor_name = eventsIn.subEvent.contactor_name,
                    contactor_email = eventsIn.subEvent.contactor_email,
                    contactor_phone = eventsIn.subEvent.contactor_phone,
                    note = eventsIn.note ?? "",
                    event_type = 2,
                    status = eventsIn.subEvent.status
                };
                var subEvent = await _ieventRepository.Create(se);
                if (subEvent != null)
                {
                    var e = new @event
                    {
                        title = eventsIn.title,
                        desctiption = eventsIn.desctiption,
                        start_date = eventsIn.start_date.Add(new TimeSpan(00, 00, 01)),
                        end_date = eventsIn.start_date.Add(new TimeSpan(23, 59, 59)),
                        venue = eventsIn.venue,
                        fan_page = eventsIn.fan_page,
                        contactor_name = eventsIn.contactor_name,
                        contactor_email = eventsIn.contactor_email,
                        contactor_phone = eventsIn.contactor_phone,
                        note = eventsIn.note ?? "",
                        event_type = 1,
                        status = 0,
                        sub_event = subEvent.event_id + ","
                    };

                    var newEvent = await _ieventRepository.Create(e);
                    if (newEvent != null)
                    {
                        return new eventsMainCreateViewModel
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
                            status = newEvent.status,
                            subEvent = new eventsViewModel
                            {
                                event_id = se.event_id,
                                contactor_email = se.contactor_phone,
                                contactor_name = se.contactor_name,
                                contactor_phone = se.contactor_phone,
                                desctiption = se.desctiption,
                                end_date = se.end_date,
                                event_type = se.event_type,
                                fan_page = se.fan_page,
                                note = se.note,
                                start_date = se.start_date,
                                title = se.title,
                                venue = se.venue,
                                status = se.status
                            }
                        };
                    }
                }
            }
            return null;
        }

        public async Task<eventsViewModel> CreateSubEvent(eventsViewModel eventsIn)
        {
            //var e = _ieventRepository.FindBy(x => x.event_id == eventsIn.event_id).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(eventsIn.title)
                && !string.IsNullOrWhiteSpace(eventsIn.desctiption)
                && !string.IsNullOrWhiteSpace(eventsIn.venue)
                && !string.IsNullOrWhiteSpace(eventsIn.fan_page)
                && Convert.ToDateTime("01/01/0001") != eventsIn.start_date
                && Convert.ToDateTime("01/01/0001") != eventsIn.end_date)
            {
                var mainEvent = _ieventRepository.FindBy(x => x.event_id == eventsIn.main_event_id).FirstOrDefault();
                if (mainEvent != null)
                {
                    var e = new @event
                    {
                        title = eventsIn.title,
                        desctiption = eventsIn.desctiption,
                        start_date = eventsIn.start_date + eventsIn.start_time,
                        end_date = eventsIn.end_date + eventsIn.end_time,
                        venue = eventsIn.venue,
                        fan_page = eventsIn.fan_page,
                        contactor_name = eventsIn.contactor_name,
                        contactor_email = eventsIn.contactor_email,
                        contactor_phone = eventsIn.contactor_phone,
                        note = eventsIn.note ?? "",
                        event_type = 2,
                        status = eventsIn.status
                    };

                    var newEvent = await _ieventRepository.Create(e);
                    if (newEvent != null)
                    {
                        if (String.IsNullOrWhiteSpace(mainEvent.sub_event))
                        {
                            mainEvent.sub_event = newEvent.event_id + ",";
                        }
                        else
                        {
                            mainEvent.sub_event += newEvent.event_id + ",";
                        }

                        if (await _ieventRepository.Update(mainEvent, mainEvent.event_id) != -1)
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
                                main_event_id = mainEvent.event_id,
                                status = newEvent.status,
                                total_joined = CountMemberJoinEvent(newEvent.event_id)
                            };
                        }
                    }
                }
            }
            return null;
        }


        public async Task<bool> DeleteEvent(int id)
        {
            var e = _ieventRepository.FindBy(x => x.event_id == id).FirstOrDefault();
            if (e != null)
            {
                e.status = -1;
                if (await _ieventRepository.Update(e, e.event_id) != -1)
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
                        desctiption = x.desctiption,
                        fan_page = x.fan_page,
                        note = x.note,
                        shirt_id = x.shirt_id,
                        start_date = x.start_date,
                        end_date = x.end_date,
                        start_date_str = x.start_date.ToString("dd-MM-yyyy"),
                        end_date_str = x.end_date.ToString("dd-MM-yyyy"),
                        title = x.title,
                        venue = x.venue,
                        contactor_name = x.contactor_name,
                        contactor_email = x.contactor_email,
                        contactor_phone = x.contactor_phone,
                        total_joined = CountMemberJoinEvent(x.event_id)
                    };
                    lstEventsViewModels.Add(e);
                }
                return lstEventsViewModels;
            }
            return null;
        }

        public List<eventsViewModel> GetSubEventsByEventId(int id)
        {
            var mainEvent = _ieventRepository.FindBy(x => x.event_id == id).FirstOrDefault();
            if (mainEvent != null)
            {
                var lstSubEvent = new List<eventsViewModel>();
                if (mainEvent.sub_event != null || !String.IsNullOrWhiteSpace(mainEvent.sub_event))
                {
                    string[] subEventId = mainEvent.sub_event.Split(',');
                    var lstSubEventTemp = new List<@event>();
                    foreach (var item in subEventId)
                    {
                        if (!String.IsNullOrWhiteSpace(item))
                        {
                            int subId = Convert.ToInt32(item.ToString());
                            var sub = _ieventRepository.FindBy(x => x.event_id == subId && x.event_type == 2).FirstOrDefault();
                            if (sub != null)
                            {
                                lstSubEventTemp.Add(sub);
                            }
                        }
                    }
                    var sortSubTemp = (from s in lstSubEventTemp
                                       orderby s.start_date ascending
                                       select s).ToList();
                    foreach (var x in sortSubTemp)
                    {
                        var subViewModel = new eventsViewModel
                        {
                            event_id = x.event_id,
                            title = x.title,
                            event_type = x.event_type,
                            desctiption = x.desctiption,
                            start_date = x.start_date,
                            end_date = x.end_date,
                            start_date_str = x.start_date.ToString("dd-MM-yyyy, HH:mm"),
                            end_date_str = x.end_date.ToString("dd-MM-yyyy, HH:mm"),
                            venue = x.venue,
                            note = x.note,
                            status = x.status,
                            contactor_name = x.contactor_name,
                            contactor_email = x.contactor_email,
                            contactor_phone = x.contactor_phone,
                            total_joined = CountMemberJoinEvent(x.event_id)
                        };
                        lstSubEvent.Add(subViewModel);
                    }
                }
                return lstSubEvent;
            }
            return null;
        }
        public async Task<AjaxResponseViewModel<bool>> JoinSubEvent(int eventId, int userId)
        {
            var output = new AjaxResponseViewModel<bool>
            {
                Status = 0,
                Data = false
            };
            var e = _ieventRepository.FindBy(x => x.event_id == eventId).FirstOrDefault();
            var user = _iapp_userRepository.FindBy(x => x.user_id == userId).FirstOrDefault();
            if (e != null && user != null)
            {
                if (String.IsNullOrWhiteSpace(e.member_join))
                {
                    e.member_join = user.user_id + ",";
                }
                else
                {
                    e.member_join += user.user_id + ",";
                }

                if (await _ieventRepository.Update(e, e.event_id) != -1)
                {
                    output.Data = true;
                    output.Status = 1;
                    return output;
                }
            }
            output.Data = false;
            return output;
        }

        public bool IsUserJoinEvent(int eventId, int userId)
        {
            var e = _ieventRepository.FindBy(x => x.event_id == eventId).FirstOrDefault();
            var user = _iapp_userRepository.FindBy(x => x.user_id == userId).FirstOrDefault();
            if (e != null && user != null)
            {
                if (!String.IsNullOrWhiteSpace(e.member_join))
                {
                    if (e.member_join.Contains(user.user_id.ToString()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public List<eventsViewModel> GetSubEventsByEventIdAndUser(int eventId, int userId)
        {
            var mainEvent = _ieventRepository.FindBy(x => x.event_id == eventId).FirstOrDefault();
            var user = _iapp_userRepository.FindBy(x => x.user_id == userId).FirstOrDefault();

            if (mainEvent != null && user != null)
            {
                var lstSubEvent = new List<eventsViewModel>();
                if (mainEvent.sub_event != null || !String.IsNullOrWhiteSpace(mainEvent.sub_event))
                {
                    string[] subEventId = mainEvent.sub_event.Split(',');
                    var lstSubEventTemp = new List<@event>();

                    foreach (var item in subEventId)
                    {
                        if (!String.IsNullOrWhiteSpace(item))
                        {
                            int subId = Convert.ToInt32(item.ToString());
                            var sub = _ieventRepository.FindBy(x => x.event_id == subId && x.event_type == 2).FirstOrDefault();
                            if (sub != null)
                            {
                                lstSubEventTemp.Add(sub);
                            }
                        }
                    }
                    var sortSubTemp = (from s in lstSubEventTemp
                                       orderby s.start_date ascending
                                       select s).ToList();
                    foreach (var x in sortSubTemp)
                    {
                        var subViewModel = new eventsViewModel
                        {
                            event_id = x.event_id,
                            title = x.title,
                            event_type = x.event_type,
                            desctiption = x.desctiption,
                            start_date = x.start_date,
                            end_date = x.end_date,
                            start_date_str = x.start_date.ToString("dd-MM-yyyy, HH:mm"),
                            end_date_str = x.end_date.ToString("dd-MM-yyyy, HH:mm"),
                            venue = x.venue,
                            note = x.note,
                            status = x.status,
                            is_user_joined = IsUserJoinEvent(x.event_id, user.user_id),
                            total_joined = CountMemberJoinEvent(x.event_id)
                        };
                        lstSubEvent.Add(subViewModel);
                    }
                }
                return lstSubEvent;
            }
            return null;
        }

        public int CountMemberJoinEvent(int eventId)
        {
            var subEvent = _ieventRepository.FindBy(x => x.event_id == eventId && x.status == 1).FirstOrDefault();
            if (subEvent != null)
            {
                if (!String.IsNullOrWhiteSpace(subEvent.member_join))
                {
                    return subEvent.member_join.Split(',').Length - 1;
                }
            }
            return 0;
        }

        public List<eventsViewModel> GetTop8Event()
        {
            var lstEvent = new List<eventsViewModel>();
            var events = _ieventRepository.FindBy(x => x.event_type == 1 && x.status != -1).ToList();
            var lstE = (from es in events
                        orderby es.start_date descending
                        select es).Take(8).ToList();
            if (events.Count() > 0)
            {
                foreach (var x in lstE)
                {
                    var e = new eventsViewModel
                    {
                        event_id = x.event_id,
                        title = x.title,
                        event_type = x.event_type,
                        desctiption = x.desctiption,
                        start_date = x.start_date,
                        end_date = x.end_date,
                        start_date_str = x.start_date.ToString("dd-MM-yyyy"),
                        end_date_str = x.end_date.ToString("dd-MM-yyyy"),
                        venue = x.venue,
                        note = x.note,
                        status = x.status,
                        total_joined = CountMemberJoinEvent(x.event_id)
                    };
                    lstEvent.Add(e);
                }
            }
            return lstEvent;
        }

        public AjaxResponseViewModel<bool> SendNotiNewEvent(int eventId)
        {
            var output = new AjaxResponseViewModel<bool>();
            var eventIn = _ieventRepository.FindBy(x => x.event_id == eventId).FirstOrDefault();
            if (eventIn != null)
            {
                var member = _imemberRepository.FindBy(x => x.event_notify == true && x.enabled == true).ToList();
                if (member != null && member.Count > 0)
                {
                    foreach (var x in member)
                    {
                        new MailHelper().sendMailEvent(x, eventIn);
                    }
                    output.Data = true;
                    return output;
                }
            }
            output.Data = false;
            return output;
        }

        public statistic_eventViewModel EventStatic()
        {
            var sortSubTemp = (from x in _ieventRepository.FindBy(x => x.event_type == 1 && x.status != 0)
                               orderby x.start_date descending
                               select x).ToList();
            var lstSubEvent = new List<eventsViewModel>();
            if (sortSubTemp.Count > 0)
            {
                var lstSubEventTemp = new List<@event>();

                foreach (var x in sortSubTemp)
                {
                    if (x.sub_event != null || !String.IsNullOrWhiteSpace(x.sub_event))
                    {
                        string[] subEventId = x.sub_event.Split(',');
                        foreach (var item in subEventId)
                        {
                            if (!String.IsNullOrWhiteSpace(item))
                            {
                                int subId = Convert.ToInt32(item.ToString());
                                var sub = _ieventRepository.FindBy(s => s.event_id == subId && s.event_type == 2 && s.status > 0).FirstOrDefault();
                                if (sub != null)
                                {
                                    lstSubEventTemp.Add(sub);
                                }
                            }
                        }
                    }
                }

                if (lstSubEventTemp.Count > 0)
                {
                    foreach (var x in lstSubEventTemp)
                    {
                        var subViewModel = new eventsViewModel
                        {
                            event_id = x.event_id,
                            title = x.title,
                            event_type = x.event_type,
                            desctiption = x.desctiption,
                            start_date = x.start_date,
                            end_date = x.end_date,
                            start_date_str = x.start_date.ToString("dd-MM-yyyy, HH:mm"),
                            end_date_str = x.end_date.ToString("dd-MM-yyyy, HH:mm"),
                            venue = x.venue,
                            note = x.note,
                            status = x.status,
                            total_joined = CountMemberJoinEvent(x.event_id)
                        };
                        lstSubEvent.Add(subViewModel);
                    }
                }
                return new statistic_eventViewModel { totalEvent = lstSubEvent.Count, lstEvents = lstSubEvent };
            }
            return null;
        }

        public statistic_eventDetailViewModel AllUsersInEvent(int eventId)
        {
            var subEvent = _ieventRepository.FindBy(x => x.event_id == eventId && x.status > 0).FirstOrDefault();
            if (subEvent != null)
            {
                var lstMemberJoin = new List<member>();
                var staticEvent = new statistic_eventDetailViewModel();
                if (!String.IsNullOrWhiteSpace(subEvent.member_join))
                {
                    string[] lsUsers = subEvent.member_join.Split(',');
                    foreach (var item in lsUsers)
                    {
                        if (!String.IsNullOrWhiteSpace(item))
                        {
                            int uId = Convert.ToInt32(item.ToString());
                            var user = _imemberRepository.FindBy(x => x.user_id == uId).FirstOrDefault();
                            if (user != null)
                            {
                                lstMemberJoin.Add(user);
                            }
                        }
                    }
                }

                return new statistic_eventDetailViewModel
                {
                    event_id = subEvent.event_id,
                    title = subEvent.title,
                    event_type = subEvent.event_type,
                    desctiption = subEvent.desctiption,
                    start_date = subEvent.start_date,
                    end_date = subEvent.end_date,
                    venue = subEvent.venue,
                    note = subEvent.note,
                    status = subEvent.status,
                    contactor_phone = subEvent.contactor_phone,
                    contactor_email = subEvent.contactor_email,
                    contactor_name = subEvent.contactor_name,
                    fan_page = subEvent.fan_page,
                    lstMembers = lstMemberJoin
                };
            }
            return null;
        }
    }

}