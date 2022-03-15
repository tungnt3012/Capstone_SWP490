using Capstone_SWP490.Models;
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


        public ActionResult ShirtSizing()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult EventUpload()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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
                    var member = _imemberService.GetMemberByUserId(u.user_id);
                    ViewData["Members"] = member;
                    return View();
                }
            }
            ViewBag.Message = "Your contact page.";
            ViewData["Events"] = _ieventService.GetEventsById(id);
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> JoinEvent(int userId)
        {
            AjaxResponseViewModel<bool> ajaxResponse = await _imemberService.JoinEvent(userId);
            return Json(ajaxResponse);
        }
        public ActionResult Event()
        {
            ViewBag.Message = "Your contact page.";
            ViewData["Events"] = _ieventService.GetAllEvents();
            return View();
        }
        public ActionResult SearchEvent(@event eventIn)
        {
            //AjaxResponseViewModel<IEnumerable<eventsViewModel>> ajaxResponse = await _ieventService.GetEventsByDate(fromDate,toDate);
            //return Json(ajaxResponse);
            var rs = _ieventService.GetEventsByDate(eventIn.start_date, eventIn.end_date);
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ScheduleOfActivities()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Index()
        {
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

            return View();
        }
        public ActionResult ContestUpload()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}