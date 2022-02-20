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

        public ActionResult ShirtSizing()
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
        [HttpPost]
        public JsonResult UpdateListContent(List<page_content> page_Contents)
        {
            var result = _ipage_contentService.UpdateListPageContent(page_Contents);
            return Json(result);
        }

        [HttpPost]
        public JsonResult UpdateSingleContent(page_content page_Contents)
        {
            var result = _ipage_contentService.UpdateSingleContent(page_Contents);
            return Json(result);
        }

        [HttpPost]
        public JsonResult DeleteSingleContent(page_content page_Contents)
        {
            var result = _ipage_contentService.DeleteSingleContent(page_Contents);
            return Json(result);
        }

        [HttpPost]
        public JsonResult CreatePageContent(page_content page_Contents)
        {
            var result = _ipage_contentService.CreatePageContent(page_Contents);
            return Json(result);
        }

        public ActionResult EventDetail()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Event()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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
    }
}