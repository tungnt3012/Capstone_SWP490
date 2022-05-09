using Capstone_SWP490.Controllers.Coach;
using Capstone_SWP490.Models.coachViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UnitTestProject.Controllers
{
    /// <summary>
    /// Summary description for SignUpControllerTest
    /// </summary>
    [TestClass]
    public class SignUpControllerTest
    {
        private readonly SignUpController controller;
        public SignUpControllerTest()
        {
            controller = new SignUpController();
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
        public async Task SignUpWithNoEmail()
        {
            coachSignUpViewModel data = new coachSignUpViewModel();

            data.email = null;
            data.full_name = "the anh tran";
            data.institution_name = "FUTEST";
            data.phone_numer = "123";
            data.school_address = "HA NOI";
            data.school_name = "FPT";
            data.rector_name = "MR A";
            ViewResult result = await controller.Index(data) as ViewResult;
            Assert.AreEqual("Invalid email!", result.ViewData["CREATE_ERROR"]);
        }


        [TestMethod]
        public async Task SignUpWithEmailInUse()
        {
            coachSignUpViewModel data = new coachSignUpViewModel();

            data.email = "ken47376@zcrcd.com";
            data.full_name = "the anh tran";
            data.institution_name = "FUTEST";
            data.phone_numer = "123";
            data.school_address = "HA NOI";
            data.school_name = "FPT";
            data.rector_name = "MR A";
            ViewResult result = await controller.Index(data) as ViewResult;
            Assert.AreEqual("Email is used, please try again!", result.ViewData["CREATE_ERROR"]);
        }


        [TestMethod]
        public async Task SignUpWithEmptyCoachName()
        {
            coachSignUpViewModel data = new coachSignUpViewModel();

            data.email = "akbch@gmail.com";
            data.full_name = null;
            data.institution_name = "FUTEST";
            data.phone_numer = "123";
            data.school_address = "HA NOI";
            data.school_name = "FPT";
            data.rector_name = "MR A";
            ViewResult result = await controller.Index(data) as ViewResult;
            Assert.AreEqual("Fullname cannot be empty", result.ViewData["CREATE_ERROR"]);
        }


        [TestMethod]
        public async Task SignUpWithDuplicateSchool()
        {
            coachSignUpViewModel data = new coachSignUpViewModel();

            data.email = "akbch@gmail.com";
            data.full_name = "the anh tran";
            data.institution_name = "SP-HCM";
            data.phone_numer = "123";
            data.school_address = "HA NOI";
            data.school_name = "ĐẠI HỌC SƯ PHẠM THÀNH PHỐ HỒ CHÍ MINH";
            data.rector_name = "MR A";
            ViewResult result = await controller.Index(data) as ViewResult;
            Assert.AreEqual("School is registered by another,please check SCHOOL NAME and INSITUTION NAME carefully \n" +
                        "or contact to Organizer for more information", result.ViewData["CREATE_ERROR"]);
        }

        [TestMethod]
        public async Task Index()
        {
            coachSignUpViewModel data = new coachSignUpViewModel();

            data.email = "testunit@gmail.com";
            data.full_name = "the anh tran";
            data.institution_name = "FUTEST";
            data.phone_numer = "123";
            data.school_address = "HA NOI";
            data.school_name = "FPT";
            data.rector_name = "MR A";
            ViewResult result = await controller.Index(data) as ViewResult;
            Assert.IsNotNull(result);
        }
    }
}
