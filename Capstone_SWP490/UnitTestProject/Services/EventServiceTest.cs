using Capstone_SWP490.Models;
using Capstone_SWP490.Models.events_ViewModel;
using Capstone_SWP490.Models.statisticViewModel;
using Capstone_SWP490.Services;
using Capstone_SWP490.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject.Services
{
    /// <summary>
    /// Summary description for EventServiceTest
    /// </summary>
    [TestClass]
    public class EventServiceTest
    {
        private readonly IeventService _ieventService = new eventService();
        public EventServiceTest()
        {
            //
            // TODO: Add constructor logic here
            //
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
        public void GetAllEventsAvailale()
        {
            IEnumerable<eventsMainViewModel> actual = _ieventService.GetAllEventsAvailale();
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GetEventsByDateFail()
        {
            AjaxResponseViewModel<IEnumerable<eventsMainViewModel>> actual = _ieventService.GetEventsByDate(DateTime.Now.AddDays(100), DateTime.Now.AddDays(100));
            Assert.AreEqual("success", actual.Message);
        }

        [TestMethod]
        public void GetEventsByDateSuccess()
        {
            AjaxResponseViewModel<IEnumerable<eventsMainViewModel>> actual = _ieventService.GetEventsByDate(DateTime.Now, DateTime.Now);
            Assert.AreEqual("success", actual.Message);
        }

        [TestMethod]
        public void SearchEventActivitiesFail()
        {
            AjaxResponseViewModel<IEnumerable<eventsViewModel>> actual = _ieventService.SearchEventActivities(DateTime.Now, DateTime.Now);
            Assert.AreEqual("success", actual.Message);
        }

        [TestMethod]
        public void SearchEventActivitiesSuccecss()
        {
            AjaxResponseViewModel<IEnumerable<eventsViewModel>> actual = _ieventService.SearchEventActivities(DateTime.Now, DateTime.Now);
            Assert.AreEqual("success", actual.Message);
        }

        [TestMethod]
        public void GetEventsById()
        {
            eventsViewModel actual = _ieventService.GetEventsById(34);
            Assert.IsNotNull(actual);
        }


        [TestMethod]
        public void GetEventsByIdNotFound()
        {
            eventsViewModel actual = _ieventService.GetEventsById(-1);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public async Task UpdateEventSuccess()
        {
            eventsViewModel eventtest = _ieventService.GetEventsById(34);
            eventtest.contactor_name = "MR A";
            eventsViewModel actual = await _ieventService.UpdateEvent(eventtest);
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public async Task UpdateEventFail()
        {
            eventsViewModel eventtest = _ieventService.GetEventsById(34);
            eventtest.contactor_name = "MR A";
            eventtest.event_id = -1;
            eventsViewModel actual = await _ieventService.UpdateEvent(eventtest);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public async Task UpdateSubEventSuccess()
        {
            eventsViewModel eventtest = _ieventService.GetEventsById(34);
            eventtest.contactor_name = "MR A";
            eventsViewModel actual = await _ieventService.UpdateSubEvent(eventtest);
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public async Task UpdateSubEventFail()
        {
            eventsViewModel eventtest = _ieventService.GetEventsById(34);
            eventtest.contactor_name = "MR A";
            eventtest.event_id = -1;
            eventsViewModel actual = await _ieventService.UpdateSubEvent(eventtest);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public async Task CreateEventNoTitle()
        {
            eventsMainCreateViewModel eventsIn = new eventsMainCreateViewModel();
            eventsIn.title = null;
            eventsMainCreateViewModel actual = await _ieventService.CreateEvent(eventsIn);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public async Task CreateEventNoDescription()
        {
            eventsMainCreateViewModel eventsIn = new eventsMainCreateViewModel();
            eventsIn.title = "UT title";
            eventsIn.desctiption = null;
            eventsMainCreateViewModel actual = await _ieventService.CreateEvent(eventsIn);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public async Task CreateEventNovenue()
        {
            eventsMainCreateViewModel eventsIn = new eventsMainCreateViewModel();
            eventsIn.title = "UT title";
            eventsIn.desctiption = "UT description";
            eventsIn.venue = null;
            eventsMainCreateViewModel actual = await _ieventService.CreateEvent(eventsIn);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public async Task CreateEventNofan_page()
        {
            eventsMainCreateViewModel eventsIn = new eventsMainCreateViewModel();
            eventsIn.title = "UT title";
            eventsIn.desctiption = "UT description";
            eventsIn.venue = "HANOi";
            eventsIn.fan_page = null;
            eventsMainCreateViewModel actual = await _ieventService.CreateEvent(eventsIn);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public async Task CreateEventNoSubEventTitle()
        {
            eventsMainCreateViewModel eventsIn = new eventsMainCreateViewModel();
            eventsIn.title = "UT title";
            eventsIn.desctiption = "UT description";
            eventsIn.venue = "HANOi";
            eventsIn.fan_page = "https://www.facebook.com/icpc.vietnam";
            eventsIn.subEvent = new eventsViewModel();
            eventsIn.subEvent.title = null;
            eventsMainCreateViewModel actual = await _ieventService.CreateEvent(eventsIn);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public async Task CreateEventNoSubEventDescription()
        {
            eventsMainCreateViewModel eventsIn = new eventsMainCreateViewModel();
            eventsIn.title = "UT title";
            eventsIn.desctiption = "UT description";
            eventsIn.venue = "HANOi";
            eventsIn.fan_page = "https://www.facebook.com/icpc.vietnam";
            eventsIn.subEvent = new eventsViewModel();
            eventsIn.subEvent.title = "UT subevent title";
            eventsIn.subEvent.desctiption = null;
            eventsMainCreateViewModel actual = await _ieventService.CreateEvent(eventsIn);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public async Task CreateEventNoSubEventVenue()
        {
            eventsMainCreateViewModel eventsIn = new eventsMainCreateViewModel();
            eventsIn.title = "UT title";
            eventsIn.desctiption = "UT description";
            eventsIn.venue = "HANOi";
            eventsIn.fan_page = "https://www.facebook.com/icpc.vietnam";
            eventsIn.subEvent = new eventsViewModel();
            eventsIn.subEvent.title = "UT subevent title";
            eventsIn.subEvent.desctiption = "ut description";
            eventsIn.subEvent.venue = null;
            eventsMainCreateViewModel actual = await _ieventService.CreateEvent(eventsIn);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public async Task CreateEventNoSubFanPage()
        {
            eventsMainCreateViewModel eventsIn = new eventsMainCreateViewModel();
            eventsIn.title = "UT title";
            eventsIn.desctiption = "UT description";
            eventsIn.venue = "HANOi";
            eventsIn.fan_page = "https://www.facebook.com/icpc.vietnam";
            eventsIn.subEvent = new eventsViewModel();
            eventsIn.subEvent.title = "UT subevent title";
            eventsIn.subEvent.desctiption = "ut description";
            eventsIn.subEvent.venue = "HA NOI";
            eventsIn.subEvent.fan_page = null;
            eventsMainCreateViewModel actual = await _ieventService.CreateEvent(eventsIn);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public async Task CreateEventSuccess()
        {
            eventsMainCreateViewModel eventsIn = new eventsMainCreateViewModel();
            eventsIn.title = "UT title";
            eventsIn.desctiption = "UT description";
            eventsIn.venue = "HANOi";
            eventsIn.fan_page = "https://www.facebook.com/icpc.vietnam";
            eventsIn.subEvent = new eventsViewModel();
            eventsIn.subEvent.title = "UT subevent title";
            eventsIn.subEvent.desctiption = "ut description";
            eventsIn.subEvent.venue = "HA NOI";
            eventsIn.subEvent.fan_page = "https://www.facebook.com/icpc.vietnam";
            eventsIn.start_date = Convert.ToDateTime("01/01/0001");
            var context = new Mock<IeventService>();
            context.Setup(p => p.CreateEvent(eventsIn)).ReturnsAsync(eventsIn);
            eventsMainCreateViewModel actual = await _ieventService.CreateEvent(eventsIn);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public async Task DeleteEvent()
        {
            eventsViewModel test = _ieventService.GetEventsById(34);
            bool actual = await _ieventService.DeleteEvent(34);
            await _ieventService.UpdateEvent(test);
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public async Task DeleteEventFail()
        {
            bool actual = await _ieventService.DeleteEvent(-5);
            Assert.IsFalse(actual);
        }


        [TestMethod]
        public void GetAllActivitiesAvailale()
        {
            IEnumerable<eventsViewModel> actual = _ieventService.GetAllActivitiesAvailale();
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GetSubEventsByEventId()
        {
            IEnumerable<eventsViewModel> actual = _ieventService.GetSubEventsByEventId(1);
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GetSubEventsByEventIdNotFound()
        {
            IEnumerable<eventsViewModel> actual = _ieventService.GetSubEventsByEventId(-2);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public async Task JoinSubEvent()
        {
            AjaxResponseViewModel<bool> actual = await _ieventService.JoinSubEvent(1, 20);
            Assert.IsTrue(actual.Data);
        }


        [TestMethod]
        public async Task JoinSubEventNotFoundEvent()
        {
            AjaxResponseViewModel<bool> actual = await _ieventService.JoinSubEvent(-5, 20);
            Assert.IsFalse(actual.Data);
        }

        [TestMethod]
        public async Task JoinSubEventNotFoundMember()
        {
            AjaxResponseViewModel<bool> actual = await _ieventService.JoinSubEvent(1, -10);
            Assert.IsFalse(actual.Data);
        }

        [TestMethod]
        public async Task UnJoinSubEvent()
        {
            AjaxResponseViewModel<bool> actual = await _ieventService.UnJoinSubEvent(1, 20);
            Assert.IsTrue(actual.Data);
        }

        [TestMethod]
        public async Task UnJoinSubEventNotFoundEvents()
        {
            AjaxResponseViewModel<bool> actual = await _ieventService.UnJoinSubEvent(-5, 20);
            Assert.IsFalse(actual.Data);
        }

        [TestMethod]
        public async Task UnJoinSubEventNotFoundMember()
        {
            AjaxResponseViewModel<bool> actual = await _ieventService.UnJoinSubEvent(1, -10);
            Assert.IsFalse(actual.Data);
        }

        [TestMethod]
        public void IsUserJoinEventTrue()
        {
            bool actual = _ieventService.IsUserJoinEvent(23, 16);
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void IsUserJoinEventNotFoundMember()
        {
            bool actual = _ieventService.IsUserJoinEvent(1, -5);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsUserJoinEventNotFoundEvent()
        {
            bool actual = _ieventService.IsUserJoinEvent(-5, 20);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void IsUserJoinEventFalse()
        {
            bool actual = _ieventService.IsUserJoinEvent(20, 1);
            Assert.IsFalse(actual);
        }


        [TestMethod]
        public void GetSubEventsByEventIdAndUser()
        {
            List<eventsViewModel> actual = _ieventService.GetSubEventsByEventIdAndUser(26, 15);
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GetSubEventsByEventIdAndUserNotFound()
        {
            List<eventsViewModel> actual = _ieventService.GetSubEventsByEventIdAndUser(-1, -10);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void GetSubEventsByEventIdAndUserNotFoundMember()
        {
            List<eventsViewModel> actual = _ieventService.GetSubEventsByEventIdAndUser(2, -10);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void GetSubEventsByEventIdAndUserNotEvent()
        {
            List<eventsViewModel> actual = _ieventService.GetSubEventsByEventIdAndUser(-5, 20);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void CountMemberJoinEvent()
        {
            int actual = _ieventService.CountMemberJoinEvent(28);
            Assert.AreNotEqual(0, actual);
        }

        [TestMethod]
        public void CountMemberJoinEventNotFoundEvent()
        {
            int actual = _ieventService.CountMemberJoinEvent(-2);
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void CountMemberJoinEventNoMemberJoined()
        {
            int actual = _ieventService.CountMemberJoinEvent(17);
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void GetTop8Event()
        {
            List<eventsViewModel> actual = _ieventService.GetTop8Event();
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void EventStatic()
        {
            statistic_eventViewModel actual = _ieventService.EventStatic();
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void AllUsersInEvent()
        {
            statistic_eventDetailViewModel actual = _ieventService.AllUsersInEvent(28);
            Assert.IsNotNull(actual);
        }


        [TestMethod]
        public void AllUsersInEventNotFoundEvent()
        {
            statistic_eventDetailViewModel actual = _ieventService.AllUsersInEvent(-2);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void AllUsersInEventNotFoundAnyMember()
        {
            statistic_eventDetailViewModel actual = _ieventService.AllUsersInEvent(26);
            Assert.IsNull(actual);
        }
    }
}
