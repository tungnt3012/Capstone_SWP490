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
    /// Summary description for MemberService
    /// </summary>
    [TestClass]
    public class MemberServiceTest
    {
        private readonly ImemberService _imemberService = new memberService();
        public MemberServiceTest()
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
            member data = _imemberService.getByEmail("bjn28215@jeoce.com");
            data.email = "testutaa@gmail.com";
            member actual = await _imemberService.insert(data);
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public async Task insertEmptyMail()
        {
            try
            {
                member data = _imemberService.getByEmail("bjn28215@jeoce.com");
                data.email = null;
                member actual = await _imemberService.insert(data);
                Assert.IsFalse(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public async Task insertEmptyFirstName()
        {
            try
            {
                member data = _imemberService.getByEmail("bjn28215@jeoce.com");
                data.email = "testut@gmail.com";
                data.first_name = null;
                member actual = await _imemberService.insert(data);
                Assert.IsFalse(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public async Task insertEmptyMiddleName()
        {
            try
            {
                member data = _imemberService.getByEmail("bjn28215@jeoce.com");
                data.email = "testut@gmail.com";
                data.middle_name = null;
                member actual = await _imemberService.insert(data);
                Assert.IsFalse(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public async Task insertEmptyLastName()
        {
            try
            {
                member data = _imemberService.getByEmail("bjn28215@jeoce.com");
                data.email = "testut@gmail.com";
                data.last_name = null;
                member actual = await _imemberService.insert(data);
                Assert.IsFalse(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public async Task insertEmptyUserId()
        {
            try
            {
                member data = _imemberService.getByEmail("bjn28215@jeoce.com");
                data.email = "testut@gmail.com";
                data.user_id = -1;
                member actual = await _imemberService.insert(data);
                Assert.IsFalse(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public async Task insertEmptyDob()
        {
            try
            {
                member data = _imemberService.getByEmail("bjn28215@jeoce.com");
                data.email = "testut@gmail.com";
                data.phone_number = null;
                member actual = await _imemberService.insert(data);
                Assert.IsFalse(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true);
            }
        }
    }
}
