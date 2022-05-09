using Capstone_SWP490;
using Capstone_SWP490.Models;
using Capstone_SWP490.Models.contestViewModel;
using Capstone_SWP490.Models.statisticViewModel;
using Capstone_SWP490.Services;
using Capstone_SWP490.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject.Services
{
    /// <summary>
    /// Summary description for ContestServiceTest
    /// </summary>
    [TestClass]
    public class ContestServiceTest
    {
        private readonly IcontestService _icontestService = new contestService();
        public ContestServiceTest()
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
        public void getByCode()
        {
            contest actual = _icontestService.getByCode("SC");
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void getByCodeNotFound()
        {
            contest actual = _icontestService.getByCode("TESTUNIT");
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void getByCodeOrNameWithoutCode()
        {
            try
            {
                contest actual = _icontestService.getByCodeOrName(null, "OKLA");
                Assert.Fail("fail");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is Exception);
            }
        }

        [TestMethod]
        public void getByCodeOrNameWithoutName()
        {
            try
            {
                contest actual = _icontestService.getByCodeOrName("OKLA", null);
                Assert.Fail("fail");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is Exception);
            }
        }

        [TestMethod]
        public void getByCodeOrNameNotFound()
        {
            contest actual = _icontestService.getByCodeOrName("OKLA", "OKLA");
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void getByCodeOrName()
        {
            contest actual = _icontestService.getByCodeOrName("SC", "Siêu Cúp");
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void getByIdNotFound()
        {
            contest actual = _icontestService.getById(1000);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void getById()
        {
            contest actual = _icontestService.getById(1);
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void getIndividualContest()
        {
            List<contest> actual = _icontestService.getIndividualContest();
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GetContests()
        {
            List<contestViewModel> actual = _icontestService.GetContests();
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public async Task CreateIndividualContest()
        {
            contest a = _icontestService.getById(1);
            a.code = DateTime.Now.Ticks + "";
            a.contest_name = DateTime.Now.Ticks + "";
            a.max_contestant = -1;
            contestViewModel test = new contestViewModel();
            test.max_contestant = -1;
            test.contest_name = a.contest_name;
            test.code = a.code;
            test.start_date = DateTime.Now;
            test.end_date = DateTime.Now.AddDays(20);
            test.contest_type = "Individual";
            contestViewModel actual = await _icontestService.CreateContest(test);
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public async Task CreateTeamContest()
        {
            contest a = _icontestService.getById(1);
            a.code = DateTime.Now.Ticks + "";
            a.contest_name = DateTime.Now.Ticks + "";
            a.max_contestant = -1;
            contestViewModel test = new contestViewModel();
            test.max_contestant = -1;
            test.contest_name = a.contest_name;
            test.code = a.code;
            test.start_date = DateTime.Now;
            test.end_date = DateTime.Now.AddDays(20);
            test.contest_type = "Team";
            contestViewModel actual = await _icontestService.CreateContest(test);
            Assert.IsNotNull(actual);
        }


        [TestMethod]
        public void GetAllContestAvailale()
        {
            IEnumerable<contestViewModel> actual = _icontestService.GetAllContestAvailale();
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void FilterContestTeam()
        {
            AjaxResponseViewModel<List<contestViewModel>> actual = _icontestService.FilterContest("Team");
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void FilterContestIndividual()
        {
            AjaxResponseViewModel<List<contestViewModel>> actual = _icontestService.FilterContest("Individual");
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GetStaticAllContestAvailale()
        {
            List<registered_contest_ViewModel> actual = _icontestService.GetStaticAllContestAvailale();
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GetStaticContest()
        {
            registered_contest_ViewModel actual = _icontestService.GetStaticContest();
            Assert.IsNotNull(actual);
        }
    }
}
