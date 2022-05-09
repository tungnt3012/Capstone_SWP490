using Capstone_SWP490;
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
    /// Summary description for ContestMemberServiceTest
    /// </summary>
    [TestClass]
    public class ContestMemberServiceTest
    {
        private readonly Icontest_memberService _icontest_memberService = new contest_memberService();
        public ContestMemberServiceTest()
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
        public async Task insert()
        {
            contest_member ct = new contest_member();
            ct.member_id = 20;
            ct.contest_id = 1;
            contest_member actual = await _icontest_memberService.insert(ct);
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public async Task insertFail()
        {
            try
            {
                //act
                contest_member ct = new contest_member();
                ct.member_id = 10000;
                ct.contest_id = 1;
                contest_member actual = await _icontest_memberService.insert(ct);
                //assert
                Assert.Fail("no exception thrown");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(true);
            }

        }

        [TestMethod]
        public async Task insertFailIncorrectContest()
        {
            try
            {
                //act
                contest_member ct = new contest_member();
                ct.member_id = 20;
                ct.contest_id = 1000;
                contest_member actual = await _icontest_memberService.insert(ct);
                //assert
                Assert.Fail("no exception thrown");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(true);
            }

        }
    }
}
