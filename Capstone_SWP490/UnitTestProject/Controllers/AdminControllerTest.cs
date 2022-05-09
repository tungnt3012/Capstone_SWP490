using Capstone_SWP490.Controllers.Admin;
using Capstone_SWP490.Services;
using Capstone_SWP490.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace UnitTestProject.Controllers
{
    /// <summary>
    /// Summary description for AdminControllerTest
    /// </summary>
    [TestClass]
    public class AdminControllerTest
    {
        private readonly AdminController controller;
        private readonly Iapp_userService _iapp_UserService = new app_userService();
        public AdminControllerTest()
        {
            controller = new AdminController();
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
        public void NotLoggedInYet()
        {
            // dump session
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["username"]).Returns(null);
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;

            RedirectToRouteResult result = controller.Index() as RedirectToRouteResult;
            // Assert, check for redirect to index view
            Assert.AreEqual("Login", result.RouteValues["action"]);

        }

        [TestMethod]
        public void Index()
        {
            // dump session
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["username"]).Returns("organizer@gmail.com");
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;

            ViewResult result = controller.Index() as ViewResult;
            // Assert, check for redirect to index view
            Assert.IsNotNull(result);

        }

        [TestMethod]
        public void Admin()
        {
            // Act
            ViewResult result = controller.Admin() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AddOrganizerGet()
        {
            // dump session
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["username"]).Returns("admin");
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;

            // Act

            ViewResult result = controller.AddOrganizer() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void ManagermentAccount()
        {
            // dump session
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["username"]).Returns("admin");
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;
            // Act
            ViewResult result = controller.AddOrganizer() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DisableUsers()
        {
            // dump session
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["username"]).Returns("admin");
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;
            // Act
            RedirectToRouteResult result = controller.DisableUsers(188) as RedirectToRouteResult;
            // Assert, check for redirect to index view
            Assert.AreEqual("ManagermentAccount", result.RouteValues["action"]);
        }

        [TestMethod]
        public void EnableUsers()
        {
            // dump session
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["username"]).Returns("admin");
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;
            // Act
            RedirectToRouteResult result = controller.EnableUsers(188) as RedirectToRouteResult;
            // Assert, check for redirect to index view
            Assert.AreEqual("ManagermentAccount", result.RouteValues["action"]);
        }

        [TestMethod]
        public void ManagermentRole()
        {
            // dump session
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["username"]).Returns("admin");
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;
            // Act
            ViewResult result = controller.ManagermentRole() as ViewResult;
            // Assert, check for redirect to index view
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetMenuContentByRole()
        {
            // dump session
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["username"]).Returns("admin");
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;
            // Act
            JsonResult result = controller.GetMenuContentByRole("ADMIN") as JsonResult;
            // Assert, check for redirect to index view
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task HideMenuContent()
        {
            // dump session
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["username"]).Returns("admin");
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;
            // Act
            JsonResult result = await controller.HideMenuContent(1) as JsonResult;
            // Assert, check for redirect to index view
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task ShowMenuContent()
        {
            // dump session
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["username"]).Returns("admin");
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;
            // Act
            JsonResult result = await controller.ShowMenuContent(1) as JsonResult;
            // Assert, check for redirect to index view
            Assert.IsNotNull(result);
        }

    }
}
