using Microsoft.VisualStudio.TestTools.UnitTesting;
using Capstone_SWP490.Controllers;
using System.Web.Mvc;
using Capstone_SWP490.Services.Interfaces;
using Capstone_SWP490.Models;
using Capstone_SWP490.Models.contestViewModel;
using Capstone_SWP490.Models.events_ViewModel;
using Capstone_SWP490.Repositories;
using Capstone_SWP490.Repositories.Interfaces;
using Capstone_SWP490.Sercurity;
using Capstone_SWP490.Services;
using Capstone_SWP490.Models.post_ViewModel;
using System.Collections.Generic;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Capstone_SWP490;
using System;

namespace UnitTestProject.Controllers
{
    /// <summary>
    /// Summary description for HomeControllerTest
    /// </summary>
    [TestClass]
    public class HomeControllerTest
    {
        private readonly Ipage_contentRepository _ipage_contentRepository = new page_contentRepository();
        private readonly Ipage_contentService _ipage_contentService = new page_contentService();
        private readonly IeventService _ieventService = new eventService();
        private readonly Iapp_userService _iapp_UserService = new app_userService();
        private readonly ImemberService _imemberService = new memberService();
        private readonly IcontestService _icontestService = new contestService();
        private readonly postService _ipostService = new postService();
        private readonly HomeController controller;
        public HomeControllerTest()
        {
            controller = new HomeController();
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void ShirtSizing()
        {
            // Act
            ViewResult result = controller.Contact() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void IndexWithLoggedIn()
        {
            // Act
            var loginUser = new Capstone_SWP490.app_user();
            loginUser.user_name = "anhttse06009@fpt.edu.vn";
            // Assert
            Assert.IsNotNull(loginUser);
        }


        [TestMethod]
        public void IndexRedirectSizing()
        {
            ViewResult result = controller.ShirtSizing() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void Scoreboard()
        {
            // Act
            ViewResult result = controller.Scoreboard() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Error()
        {
            // Act
            ViewResult result = controller.Error() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ListHomePageContentNotEmptyPost()
        {
            // Act
            List<post_TopViewModel> result = new List<post_TopViewModel>();
            post_TopViewModel item = new post_TopViewModel();
            item.content = "test post content";
            result.Add(item);
            // Assert
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void ListHomePageContentEmptyPost()
        {
            // Act
            List<post_TopViewModel> actual = new List<post_TopViewModel>();
            // Assert
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void ScoreboardUpload()
        {
            // Act
            ViewResult result = controller.ScoreboardUpload() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ScoreboardUploadSuccess()
        {

            Capstone_SWP490.page_content pageContent = new Capstone_SWP490.page_content();
            pageContent.page_id = "0";
            pageContent.title = "test unit";
            pageContent.content = "test unit";
            pageContent.position = 10;
            pageContent.user_role = "MEMBER";
            pageContent.status = 0;
            // Act
            Task<ActionResult> result = controller.ScoreboardUpload(pageContent);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ScoreboardUploadFail()
        {

            // Act
            string result = "*Edit Content Failed !!!";

            // Assert
            Assert.AreEqual(result, result);
        }

        [TestMethod]
        public void ScoreboardManagement()
        {

            // Act
            List<page_content> data = new List<page_content>();
            page_content item = new page_content();
            item.title = "test unit";
            item.content = "test unit";
            data.Add(item);

            // Assert
            Assert.AreEqual(1, data.Count);
        }


        [TestMethod]
        public void ScoreboardEditGet()
        {
            var pageContent = _ipage_contentRepository.FindBy(x => x.content_id == 1);
            ViewResult result = controller.ScoreboardEdit(1) as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ScoreboardEditSuccess()
        {

            Capstone_SWP490.page_content pageContent = new Capstone_SWP490.page_content();
            pageContent.page_id = "0";
            pageContent.title = "test unit";
            pageContent.content = "test unit";
            pageContent.position = 10;
            pageContent.user_role = "MEMBER";
            pageContent.status = 0;
            // Act
            Task<ActionResult> result = controller.ScoreboardEdit(pageContent);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ScoreboardEditFail()
        {

            // Act
            string result = "*Edit Content Failed !!!";

            // Assert
            Assert.AreEqual(result, result);
        }


        [TestMethod]
        public async Task ScoreboardDeleteFail()
        {
            int id = 1000;
            var rsPage = await _ipage_contentService.DeleteContent(id);
            ViewResult result = await controller.ScoreboardDelete(id) as ViewResult;
            // Assert
            Assert.AreEqual("Delete Fail!!!", result.ViewData["error"]);
        }

        [TestMethod]
        public async Task ScoreboardDeleteSuccess()
        {
            int id = 217;
            var rsPage = await _ipage_contentService.DeleteContent(id);
            ViewResult result = await controller.ScoreboardDelete(id) as ViewResult;
            // Assert
            Assert.AreEqual("Delete " + rsPage.title + " Successfully!!!", result.ViewData["success"]);
        }

        [TestMethod]
        public void ContestStatistic()
        {
            // Act
            ViewResult result = controller.ContestStatistic() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void EventEdit()
        {
            // Act
            ViewResult result = controller.EventEdit(1) as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void EventUploadGet()
        {
            // Act
            ViewResult result = controller.EventUpload() as ViewResult;
            string msg = "Your Event Upload page.";
            // Assert
            Assert.AreEqual(msg, result.ViewBag.Message);
        }

        [TestMethod]
        public async Task EventUploadSuccessAsync()
        {
            // Act
            eventsMainCreateViewModel events = new eventsMainCreateViewModel();
            events.contactor_email = "anhttse06009@fpt.edu.vn";
            events.contactor_name = "the anh";
            events.contactor_phone = "1231231";
            events.desctiption = "test";
            events.end_date = DateTime.Now;
            events.end_time = TimeSpan.MinValue;
            events.event_type = 1;
            events.main_event_id = 1;
            events.venue = "HA NOI";
            ViewResult result = await controller.EventUpload(events) as ViewResult;
            string msg = "*Add Event Failed !!!";
            // Assert
            Assert.AreEqual(msg, result.ViewData["error"]);
        }

        [TestMethod]
        public async Task EventDeleteFail()
        {
            // Act
            ViewResult result = await controller.EventDelete(1000) as ViewResult;
            string msg = "*Delete Event Failed !!!";
            // Assert
            Assert.AreEqual(msg, result.ViewData["error"]);
        }


        [TestMethod]
        public async Task EventDeleteSuccess()
        {
            // Act
            ViewResult result = await controller.EventDelete(41) as ViewResult;
            string msg = "*Delete Event Successfully !!!";
            // Assert
            Assert.AreEqual(msg, result.ViewData["success"]);
        }

        [TestMethod]
        public void Downloads()
        {
            // Act
            ViewResult result = controller.Downloads() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetContentAccommodations()
        {
            // Act
            JsonResult result = controller.GetContentAccommodations() as JsonResult;
            // Assert
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void LocalInformation()
        {
            // Act
            ViewResult result = controller.LocalInformation() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetContentLocalInformation()
        {
            // Act
            JsonResult result = controller.GetContentLocalInformation() as JsonResult;
            // Assert
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void Rules()
        {
            // Act
            ViewResult result = controller.Rules() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetContentRule()
        {
            // Act
            JsonResult result = controller.GetContentRule() as JsonResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetContentDownloads()
        {
            // Act
            JsonResult result = controller.GetContentDownloads() as JsonResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetGuestMenu()
        {
            // Act
            JsonResult result = controller.GetGuestMenu() as JsonResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetAdminMenu()
        {
            // Act
            JsonResult result = controller.GetAdminMenu() as JsonResult;
            // Assert
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void GetOrganizationMenu()
        {
            // Act
            JsonResult result = controller.GetOrganizationMenu() as JsonResult;
            // Assert
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void UpdateListContent()
        {
            // Act

            // Assert
            Assert.AreEqual(1, 1);
        }

        [TestMethod]
        public void UpdateSingleContent()
        {
            // Act

            // Assert
            Assert.AreEqual(1, 1);
        }

        [TestMethod]
        public void DeleteSingleContent()
        {
            // Act

            // Assert
            Assert.AreEqual(1, 1);
        }

        [TestMethod]
        public async Task CreatePageContentAsync()
        {
            // Act
            Capstone_SWP490.page_content pageContent = new Capstone_SWP490.page_content();
            pageContent.page_id = "0";
            pageContent.title = "test unit";
            pageContent.content = "test unit";
            pageContent.position = 10;
            pageContent.user_role = "MEMBER";
            pageContent.status = 0;
            JsonResult result = await controller.CreatePageContent(pageContent) as JsonResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task PinPageContent()
        {
            // Act
            Capstone_SWP490.page_content pageContent = new Capstone_SWP490.page_content();
            pageContent.page_id = "0";
            pageContent.title = "test unit";
            pageContent.content = "test unit";
            pageContent.position = 10;
            pageContent.user_role = "MEMBER";
            pageContent.status = 0;
            JsonResult result = await controller.CreatePageContent(pageContent) as JsonResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SubEventDetail()
        {
            // Act
            ViewResult result = controller.SubEventDetail(1) as ViewResult;
            // Assert
            Assert.AreEqual(1, 1);
        }


        [TestMethod]
        public void SubEventUpload()
        {
            // Act
            ViewResult result = controller.SubEventUpload(1) as ViewResult;
            // Assert
            Assert.AreEqual("Your Sub-Event Upload page.", result.ViewBag.Message);
        }

        [TestMethod]
        public async Task SubEventUploadSuccess()
        {
            // Act
            eventsViewModel events = new eventsViewModel();
            events.contactor_email = "anhttse06009@fpt.edu.vn";
            events.contactor_name = "the anh";
            events.contactor_phone = "1231231";
            events.desctiption = "test";
            events.end_date = DateTime.Now;
            events.end_time = TimeSpan.MinValue;
            events.event_type = 1;
            events.main_event_id = 1;
            events.venue = "HA NOI";
            ViewResult result = await controller.SubEventUpload(events) as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        public async Task SubEventUploadFail()
        {
            // Act
            eventsViewModel events = new eventsViewModel();
            events.contactor_email = "anhttse06009@fpt.edu.vn";
            events.contactor_name = "the anh";
            events.contactor_phone = "1231231";
            events.desctiption = "test";
            events.end_date = DateTime.Now;
            events.end_time = TimeSpan.MinValue;
            events.event_type = 1;
            events.main_event_id = 1;
            events.venue = "HA NOI";
            ViewResult result = await controller.SubEventUpload(events) as ViewResult;
            // Assert
            Assert.AreEqual("*Add Event Failed !!!", result.ViewData["error"]);
        }

        [TestMethod]
        public void SubEventEdit()
        {
            // Act
            ViewResult result = controller.SubEventEdit(1, 1) as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SubEventEditSuccess()
        {
            // Act
            ViewResult result = controller.SubEventEdit(1, 1) as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task SubEventEditFailAsync()
        {
            // Act
            eventsViewModel events = new eventsViewModel();
            events.contactor_email = "anhttse06009@fpt.edu.vn";
            events.contactor_name = "the anh";
            events.contactor_phone = "1231231";
            events.desctiption = "test";
            events.end_date = DateTime.Now;
            events.end_time = TimeSpan.MinValue;
            events.event_type = 1;
            events.main_event_id = 1;
            events.venue = "HA NOI";
            ViewResult result = await controller.SubEventEdit(events) as ViewResult;
            // Assert
            Assert.AreEqual("*Edit Event Failed !!!", result.ViewData["error"]);
        }


        [TestMethod]
        public async Task JoinSubEvent()
        {
            // Act
            JsonResult result = await controller.JoinSubEvent(1, 1) as JsonResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task UnJoinSubEvent()
        {
            // Act
            JsonResult result = await controller.UnJoinSubEvent(1, 1) as JsonResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task JoinEvent()
        {
            // Act
            JsonResult result = await controller.JoinEvent(192) as JsonResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SearchEvent()
        {
            // Act
            @event eventIn = new @event();
            eventIn.start_date = DateTime.Now;
            eventIn.end_date = DateTime.Now;
            JsonResult result = controller.SearchEvent(eventIn) as JsonResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SearchEventActivities()
        {
            // Act
            @event eventIn = new @event();
            eventIn.start_date = DateTime.Now;
            eventIn.end_date = DateTime.Now;
            JsonResult result = controller.SearchEventActivities(eventIn) as JsonResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SendNotiEvent()
        {
            // Act

            JsonResult result = controller.SendNotiEvent(1) as JsonResult;
            // Assert
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void GetTopEvents()
        {
            // Act

            JsonResult result = controller.GetTopEvents() as JsonResult;
            // Assert
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void GetTopPosts()
        {
            // Act

            JsonResult result = controller.GetTopPosts() as JsonResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ChangPassword()
        {
            ViewResult result = controller.ChangPassword() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void HomeLogin()
        {
            ViewResult result = controller.HomeLogin() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetContentRegistrationTitle()
        {
            // Act

            JsonResult result = controller.GetContentRegistrationTitle() as JsonResult;
            // Assert
            Assert.IsNotNull(result);
        }
    }
}
