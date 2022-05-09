using Capstone_SWP490.Controllers;
using Capstone_SWP490.Models.app_userViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace UnitTestProject.Controllers
{
    /// <summary>
    /// Summary description for PostController
    /// </summary>
    [TestClass]
    public class PostControllerTest
    {
        private readonly PostController controller;
        public PostControllerTest()
        {
            controller = new PostController();
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
        public void Index()
        {
            // dump session
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["profile"]).Returns(new app_userViewModel());
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;

            ViewResult result = controller.Index("Enabled") as ViewResult;

            Assert.IsNotNull(result);
        }
    }
}
