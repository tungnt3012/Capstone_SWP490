using Capstone_SWP490.Helper;
using Capstone_SWP490.Models;
using Capstone_SWP490.Models.contestViewModel;
using Capstone_SWP490.Models.events_ViewModel;
using Capstone_SWP490.Repositories;
using Capstone_SWP490.Repositories.Interfaces;
using Capstone_SWP490.Services;
using Capstone_SWP490.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Capstone_SWP490.Controllers
{
    public class HomeController : Controller
    {
        private readonly Ipage_contentRepository _ipage_contentRepository = new page_contentRepository();
        private readonly Ipage_contentService _ipage_contentService = new page_contentService();
        private readonly IeventService _ieventService = new eventService();
        private readonly Iapp_userService _iapp_UserService = new app_userService();
        private readonly ImemberService _imemberService = new memberService();
        private readonly IcontestService _icontestService = new contestService();
        private readonly IpostService _ipostService = new postService();


        public ActionResult ShirtSizing()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Scoreboard()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult ListHomePageContent()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult HomePageContent()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult ScoreboardUpload()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult ScoreboardManagement()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ContestStatistic()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult EventEdit(int id)
        {
            ViewBag.Message = "Your contact page.";
            var events = _ieventService.GetEventsById(id);
            return View(events);
        }
        [HttpPost]
        public async Task<ActionResult> EventEdit(eventsViewModel events)
        {
            var rsUpdate = await _ieventService.UpdateEvent(events);
            if (rsUpdate != null)
            {
                ViewData["success"] = "*Edit Event Successfully !!!";
                return View(rsUpdate);
            }
            ViewData["error"] = "*Edit Event Failed !!!";
            return View(events);
        }

        public ActionResult EventUpload()
        {
            ViewBag.Message = "Your Event Upload page.";
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> EventUpload(eventsViewModel events)
        {
            var rsCreate = await _ieventService.CreateEvent(events);
            if (rsCreate != null)
            {
                ViewData["success"] = "*Add Event Successfully !!!";
                return View(rsCreate);
            }
            ViewData["error"] = "*Add Event Failed !!!";
            return View(events);
        }
        public async Task<ActionResult> EventDelete(int id)
        {
            var rsCreate = await _ieventService.DeleteEvent(id);
            ViewData["Events"] = _ieventService.GetAllEventsAvailale();
            if (rsCreate == true)
            {
                ViewData["success"] = "*Delete Event Successfully !!!";
                return View("Event");
            }
            ViewData["error"] = "*Delete Event Failed !!!";
            return View("Event");
        }

        public ActionResult Downloads()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Accommodations()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult GetContentAccommodations()
        {
            return Json(_ipage_contentRepository.GetPage_ContentByPageId("Accommodations"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LocalInformation()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult GetContentLocalInformation()
        {
            return Json(_ipage_contentRepository.GetPage_ContentByPageId("LocalInformation"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Rules()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult GetContentRule()
        {
            return Json(_ipage_contentRepository.GetPage_ContentByPageId("RULE"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetContentDownloads()
        {
            return Json(_ipage_contentRepository.GetPage_ContentByPageId("Downloads"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGuestMenu()
        {
            return Json(_ipage_contentService.GetMenuContents("GUEST"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAdminMenu()
        {
            return Json(_ipage_contentService.GetMenuContents("ADMIN"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOrganizationMenu()
        {
            return Json(_ipage_contentService.GetMenuContents("ORGANIZER"), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateListContent(List<page_content> page_Contents)
        {
            AjaxResponseViewModel<IEnumerable<page_content>> ajaxResponse = await _ipage_contentService.UpdateListPageContent(page_Contents);
            return Json(ajaxResponse);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateSingleContent(page_content page_Contents)
        {
            AjaxResponseViewModel<page_content> ajaxResponse = await _ipage_contentService.UpdateSingleContent(page_Contents);
            return Json(ajaxResponse);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteSingleContent(page_content page_Contents)
        {
            AjaxResponseViewModel<bool> ajaxResponse = await _ipage_contentService.DeleteSingleContent(page_Contents);
            return Json(ajaxResponse);
        }

        [HttpPost]
        public async Task<ActionResult> CreatePageContent(page_content page_Contents)
        {
            AjaxResponseViewModel<page_content> ajaxResponse = await _ipage_contentService.CreatePageContent(page_Contents);
            return Json(ajaxResponse);
        }

        [HttpPost]
        public async Task<ActionResult> PinPageContent(page_content page_Contents)
        {
            AjaxResponseViewModel<bool> ajaxResponse = await _ipage_contentService.PinPageContent(page_Contents);
            return Json(ajaxResponse);
        }

        public ActionResult EventDetail(int id)
        {
            if (HttpContext.Session["username"] != null)
            {
                var u = _iapp_UserService.GetUserByUsername(HttpContext.Session["username"].ToString());
                if (u != null)
                {
                    ViewData["Events"] = _ieventService.GetEventsById(id);
                    ViewData["SubEvents"] = _ieventService.GetSubEventsByEventIdAndUser(id, u.user_id);
                    ViewData["Members"] = _imemberService.GetMemberByUserId(u.user_id);
                    return View();
                }
            }
            ViewBag.Message = "Your contact page.";
            ViewData["Events"] = _ieventService.GetEventsById(id);
            ViewData["SubEvents"] = _ieventService.GetSubEventsByEventId(id);
            return View();
        }

        public ActionResult SubEventDetail(int id)
        {
            ViewData["Events"] = _ieventService.GetEventsById(id);
            return View();
        }
        public ActionResult SubEventUpload(int id)
        {
            ViewBag.Message = "Your Sub-Event Upload page.";
            ViewData["MainEvent"] = _ieventService.GetEventsById(id);
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SubEventUpload(eventsViewModel events)
        {
            var rsCreate = await _ieventService.CreateSubEvent(events);
            if (rsCreate != null)
            {
                ViewData["success"] = "*Add Event Successfully !!!";
                ViewData["MainEvent"] = _ieventService.GetEventsById(events.main_event_id);
                return View(rsCreate);
            }
            ViewData["error"] = "*Add Event Failed !!!";
            ViewData["MainEvent"] = _ieventService.GetEventsById(events.main_event_id);
            return View(events);
        }


        [HttpPost]
        public async Task<ActionResult> JoinSubEvent(int subEvent, int userCrr)
        {
            AjaxResponseViewModel<bool> ajaxResponse = await _ieventService.JoinSubEvent(subEvent, userCrr);
            return Json(ajaxResponse);
        }


        [HttpPost]
        public async Task<ActionResult> JoinEvent(int userId)
        {
            AjaxResponseViewModel<bool> ajaxResponse = await _imemberService.JoinEvent(userId);
            return Json(ajaxResponse);
        }
        public ActionResult Event()
        {
            if (HttpContext.Session["username"] != null)
            {
                var u = _iapp_UserService.GetUserByUsername(HttpContext.Session["username"].ToString());
                if (u != null)
                {
                    var member = _imemberService.GetMemberByAvaibleUserId(u.user_id);
                    ViewData["Events"] = _ieventService.GetAllEventsAvailale();
                    ViewData["Members"] = member;
                    return View();
                }
            }
            ViewBag.Message = "Your contact page.";
            ViewData["Events"] = _ieventService.GetAllEventsAvailale();
            return View();
        }
        [HttpPost]
        public ActionResult SearchEvent(@event eventIn)
        {
            //AjaxResponseViewModel<IEnumerable<eventsViewModel>> ajaxResponse = await _ieventService.GetEventsByDate(fromDate,toDate);
            //return Json(ajaxResponse);
            AjaxResponseViewModel<IEnumerable<eventsViewModel>> rs = _ieventService.GetEventsByDate(eventIn.start_date, eventIn.end_date);
            //return Json(rs, JsonRequestBehavior.AllowGet);
            return Json(rs);
        }

        [HttpPost]
        public ActionResult SearchEventActivities(@event eventIn)
        {
            //AjaxResponseViewModel<IEnumerable<eventsViewModel>> ajaxResponse = await _ieventService.GetEventsByDate(fromDate,toDate);
            //return Json(ajaxResponse);
            AjaxResponseViewModel<IEnumerable<eventsViewModel>> rs = _ieventService.SearchEventActivities(eventIn.start_date, eventIn.end_date);
            //return Json(rs, JsonRequestBehavior.AllowGet);
            return Json(rs);
        }
        public ActionResult GetTopEvents()
        {
            return Json(_ieventService.GetTop8Event(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTopPosts()
        {
            return Json(_ipostService.GetTop5Posts(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ScheduleOfActivities()
        {
            ViewBag.Message = "Your contact page.";
            ViewData["Activities"] = _ieventService.GetAllActivitiesAvailale();
            return View();
        }
        public ActionResult Index()
        {
            //if (HttpContext.Session["username"] != null)
            //{
            //    var u = _iapp_UserService.GetUserByUsername(HttpContext.Session["username"].ToString());
            //    if (u != null)
            //    {
            //        var memberTemp = _imemberService.GetMemberByAvaibleUserId(u.user_id);
            //        if (memberTemp != null)
            //        {
            //            if (String.IsNullOrWhiteSpace(memberTemp.shirt_sizing))
            //            {
            //                return RedirectToAction("RegisShirtSizing", "Login");
            //            }
            //        }
            //    }
            //}
            ViewBag.Message = "Your contact page.";
            return View();
        }
        public ActionResult RegisShirtSizing()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult ChangPassword()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult HomeLogin()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult RegistrationTitle()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult GetContentRegistrationTitle()
        {
            return Json(_ipage_contentRepository.GetPage_ContentByPageId("GUID"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult RegistrationFrom()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
        public ActionResult RegistrationImport()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult GetContentAbout()
        {
            return Json(_ipage_contentRepository.GetPage_ContentByPageId("ABOUT"), JsonRequestBehavior.AllowGet);
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Contest()
        {
            ViewBag.Message = "Your contact page.";
            ViewData["Contest"] = _icontestService.GetContests();
            return View();
        }

        public ActionResult SearchContestType(@event eventIn)
        {
            //AjaxResponseViewModel<IEnumerable<eventsViewModel>> ajaxResponse = await _ieventService.GetEventsByDate(fromDate,toDate);
            //return Json(ajaxResponse);
            var rs = _ieventService.GetEventsByDate(eventIn.start_date, eventIn.end_date);
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult ContestDetail(int id)
        //{
        //    //if (HttpContext.Session["username"] != null)
        //    //{
        //    //    var u = _iapp_UserService.GetUserByUsername(HttpContext.Session["username"].ToString());
        //    //    if (u != null)
        //    //    {
        //    //        ViewData["Contest"] = _icontestService.GetContestById(id);
        //    //        return View();
        //    //    }
        //    //}
        //    ViewBag.Message = "Your contact page.";
        //    ViewData["Contest"] = _icontestService.GetContestById(id);
        //    return View();
        //}
        public ActionResult ContestEdit(int id)
        {
            ViewBag.Message = "Your contact page.";
            var contests = _icontestService.GetContestById(id);
            return View(contests);
        }
        [HttpPost]
        public async Task<ActionResult> ContestEdit(contestViewModel contest)
        {
            var rsUpdate = await _icontestService.UpdateContest(contest);
            if (rsUpdate != null)
            {
                ViewData["success"] = "*Edit Contest Successfully !!!";
                return View(rsUpdate);
            }
            ViewData["error"] = "*Edit Contest Failed !!!";
            return View(contest);
        }

        public ActionResult ContestUpload()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ContestUpload(contestViewModel contest)
        {
            var rsCreate = await _icontestService.CreateContest(contest);
            if (rsCreate != null)
            {
                ViewData["success"] = "*Add Contest Successfully !!!";
                return View(rsCreate);
            }
            ViewData["error"] = "*Add Contest Failed !!!";
            return View(rsCreate);
        }

        public async Task<ActionResult> ContestDelete(int id)
        {
            var rsCreate = await _icontestService.DeleteContest(id);
            ViewData["Contest"] = _icontestService.GetAllContestAvailale();
            if (rsCreate == true)
            {
                ViewData["success"] = "*Delete Contest Successfully !!!";
                return View("Contest");
            }
            ViewData["error"] = "*Delete Contest Failed !!!";
            return View("Contest");
        }

        public ActionResult FilterContest(string keyFilter)
        {
            return Json(_icontestService.FilterContest(keyFilter), JsonRequestBehavior.AllowGet);
        }

    }
}