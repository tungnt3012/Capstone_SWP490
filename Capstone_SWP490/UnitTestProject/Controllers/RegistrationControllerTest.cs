using Capstone_SWP490;
using Capstone_SWP490.Constant.Const;
using Capstone_SWP490.Controllers.Coach;
using Capstone_SWP490.Models.app_userViewModel;
using Capstone_SWP490.Models.registrationViewModel;
using Capstone_SWP490.Models.school_memberViewModel;
using Capstone_SWP490.Services;
using Capstone_SWP490.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;


namespace UnitTestProject.Controllers
{
    /// <summary>
    /// Summary description for RegistrationControllerTest
    /// </summary>
    [TestClass]
    public class RegistrationControllerTest
    {
        private readonly RegistrationController controller;
        private readonly Iapp_userService _iapp_UserService = new app_userService();
        private readonly IschoolService _ischoolService = new schoolService();
        private readonly ImemberService _imemberService = new memberService();
        private readonly Icontest_memberService _icontest_memberService = new contest_memberService();
        private readonly IcontestService _icontestService = new contestService();
        private readonly Iteam_memberService _iteam_memberService = new teamMemberService();

        public RegistrationControllerTest()
        {
            controller = new RegistrationController();
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
        public void Guide()
        {
            // Act
            ViewResult result = controller.Guide() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void IndexLogined()
        {
            // dump session
            var user = _iapp_UserService.GetUserByUsername("mathlover1998@gmail.com");
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["profile"]).Returns(user);
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;

            ViewResult result = controller.Index() as ViewResult;
            // Assert
            Assert.AreEqual("", result.ViewName);
        }

        [TestMethod]
        public void IndexNotLoginedYet()
        {
            // dump session
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["profile"]).Returns(null);
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;

            //act
            ViewResult result = controller.Index() as ViewResult;
            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void MemberDetailWithNoDataSchool()
        {
            // dump session
            var user = _iapp_UserService.GetUserByUsername("mathlover1998@gmail.com");
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["profile"]).Returns(user);
            session.Setup(p => p["SCHOOL"]).Returns(null);
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;

            ViewResult result = controller.MemberDetail(2, 3) as ViewResult;
            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void MemberDetailWithNoDataTeamMember()
        {
            // dump session
            var user = _iapp_UserService.GetUserByUsername("mathlover1998@gmail.com");
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            var controllerDump = new Mock<RegistrationController>();
            session.Setup(p => p["profile"]).Returns(user);
            session.Setup(p => p["SCHOOL"]).Returns(new import_resultViewModel());
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;
            //not found member with id = 2 and belong to team 3
            ViewResult result = controller.MemberDetail(2, 3) as ViewResult;
            // Assert
            Assert.IsNull(result);
        }
        [TestMethod]
        public void SaveChangeWithNoDataSchool()
        {
            // dump session
            var user = _iapp_UserService.GetUserByUsername("mathlover1998@gmail.com");
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["profile"]).Returns(user);
            session.Setup(p => p["SCHOOL"]).Returns(null);
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;

            ViewResult result = controller.MemberDetail(2, 3) as ViewResult;
            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void SaveChangeWithDataSchool()
        {
            // dump session
            var user = _iapp_UserService.GetUserByUsername("mathlover1998@gmail.com");
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["profile"]).Returns(user);
            import_resultViewModel school = MockSchoolData(86);
            session.Setup(p => p["SCHOOL"]).Returns(school);
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;

            RedirectToRouteResult result = controller.SaveChange() as RedirectToRouteResult;
            // Assert
            Assert.IsNotNull(controller.HttpContext.Session["SCHOOL"]);
        }


        [TestMethod]
        public void SaveChangeTeamWithNoDataSchool()
        {
            // dump session
            var user = _iapp_UserService.GetUserByUsername("mathlover1998@gmail.com");
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["profile"]).Returns(user);
            import_resultViewModel school = MockSchoolData(86);
            session.Setup(p => p["SCHOOL"]).Returns(school);
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;

            RedirectToRouteResult result = controller.SaveChangeTeam(null, 1, 1) as RedirectToRouteResult;
            // Assert, check for redirect to index view
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SaveChangeTeamWithTeamName()
        {
            // dump session
            var user = _iapp_UserService.GetUserByUsername("mathlover1998@gmail.com");
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["profile"]).Returns(user);
            import_resultViewModel school = MockSchoolData(86);
            session.Setup(p => p["SCHOOL"]).Returns(school);
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;

            RedirectToRouteResult result = controller.SaveChangeTeam("test team name", 160, 1) as RedirectToRouteResult;
            import_resultViewModel actual = (import_resultViewModel)controller.HttpContext.Session["SCHOOL"];
            // Assert, check for redirect to index view
            Assert.AreEqual("test team name", actual.School.teams.ElementAt(0).team_name);
        }

        [TestMethod]
        public void SaveChangeTeamWithTeamNameNull()
        {
            // dump session
            var user = _iapp_UserService.GetUserByUsername("mathlover1998@gmail.com");
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["profile"]).Returns(user);
            session.Setup(p => p["SCHOOL"]).Returns(null);
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;

            RedirectToRouteResult result = controller.SaveChangeTeam("1", 1, 1) as RedirectToRouteResult;
            // Assert, check for redirect to index view
            Assert.AreEqual("", result.RouteName);
        }

        [TestMethod]
        public void TeamDetailWithNoDataSchool()
        {
            // dump session
            var user = _iapp_UserService.GetUserByUsername("mathlover1998@gmail.com");
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["profile"]).Returns(user);
            session.Setup(p => p["SCHOOL"]).Returns(null);
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;

            RedirectToRouteResult result = controller.TeamDetail(0) as RedirectToRouteResult;
            // Assert, check for redirect to index view
            Assert.AreEqual("", result.RouteName);
        }

        [TestMethod]
        public void TeamDetail()
        {
            // dump session
            var user = _iapp_UserService.GetUserByUsername("mathlover1998@gmail.com");
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["profile"]).Returns(user);
            import_resultViewModel school = MockSchoolData(86);
            session.Setup(p => p["SCHOOL"]).Returns(school);
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;

            RedirectToRouteResult result = controller.TeamDetail(160) as RedirectToRouteResult;
            // Assert, check for redirect to index view
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task InsertMember()
        {
            // dump session
            var user = _iapp_UserService.GetUserByUsername("mathlover1998@gmail.com");
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["profile"]).Returns(user);
            import_resultViewModel school = MockSchoolData(86);
            session.Setup(p => p["SCHOOL"]).Returns(school);
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;

            RedirectToRouteResult result = await controller.InsertMember() as RedirectToRouteResult;
            // Assert, check for redirect to index view
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void MemberDetail()
        {
            // dump session
            var user = _iapp_UserService.GetUserByUsername("mathlover1998@gmail.com");
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["profile"]).Returns(user);
            import_resultViewModel school = MockSchoolData(86);
            session.Setup(p => p["SCHOOL"]).Returns(school);
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;

            RedirectToRouteResult result =  controller.MemberDetail(0,1) as RedirectToRouteResult;
            // Assert, check for redirect to index view
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Result()
        {
            // dump session
            var user = _iapp_UserService.GetUserByUsername("mathlover1998@gmail.com");
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["profile"]).Returns(user);
            import_resultViewModel school = MockSchoolData(86);
            session.Setup(p => p["SCHOOL"]).Returns(school);
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;

            RedirectToRouteResult result = controller.Result("HCMUE.Frost's Descent") as RedirectToRouteResult;
            // Assert, check for redirect to index view
            Assert.IsNull(result);
        }

        [TestMethod]
        public void History()
        {
            // dump session
            var user = _iapp_UserService.GetUserByUsername("mathlover1998@gmail.com");
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["profile"]).Returns(user);
            import_resultViewModel school = MockSchoolData(86);
            session.Setup(p => p["SCHOOL"]).Returns(school);
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;

            RedirectToRouteResult result = controller.History("86") as RedirectToRouteResult;
            // Assert, check for redirect to index view
            Assert.IsNull(result);
        }

        [TestMethod]
        public void RemoveMember()
        {
            // dump session
            var user = _iapp_UserService.GetUserByUsername("mathlover1998@gmail.com");
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["profile"]).Returns(user);
            import_resultViewModel school = MockSchoolData(86);
            session.Setup(p => p["SCHOOL"]).Returns(school);
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;
            team team = school.School.teams.FirstOrDefault();
            RedirectToRouteResult result = controller.RemoveMember(team.team_id, team.team_member.FirstOrDefault().member.member_id) as RedirectToRouteResult;
            // Assert, check for redirect to index view
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void RemoveTeam()
        {
            // dump session
            var user = _iapp_UserService.GetUserByUsername("mathlover1998@gmail.com");
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(p => p["profile"]).Returns(user);
            import_resultViewModel school = MockSchoolData(86);
            session.Setup(p => p["SCHOOL"]).Returns(school);
            context.Setup(p => p.HttpContext.Session).Returns(session.Object);
            controller.ControllerContext = context.Object;

            RedirectToRouteResult result = controller.RemoveTeam(160) as RedirectToRouteResult;
            // Assert, check for redirect to index view
            Assert.IsNotNull(result);
        }

        private import_resultViewModel MockSchoolData(int schoolId)
        {
            import_resultViewModel data = new import_resultViewModel();
            school school = null;
            List<team> teams = null;
            try
            {
                data.Source = "VIEW";
                school = _ischoolService.findActiveById(schoolId);
                teams = school.teams.ToList();
                var user = _iapp_UserService.GetUserByUsername("mathlover1998@gmail.com");
                member storedCoach = _imemberService.GetMemberByUserId(user.user_id);
                teams = teams.Where(x => !x.type.Trim().Equals(APP_CONST.TEAM_ROLE.COACH_TEAM)).ToList();
                foreach (var item in teams)
                {
                    item.contest = _icontestService.getById(item.contest_id);
                }
                school.teams = teams;
                data.School = school;
                team coachTeam = storedCoach.team_member.FirstOrDefault().team;
                if (coachTeam != null)
                {
                    List<team_member> coachTeamMember = _iteam_memberService.getCoachTeamMember(coachTeam.team_id);
                    team_member coachMember = coachTeamMember.Where(x => x.member.member_role == 1).FirstOrDefault();
                    if (coachMember != null)
                    {
                        data.Coach = coachMember.member;
                    }
                    team_member viceCoachMember = coachTeamMember.Where(x => x.member.member_role == 2).FirstOrDefault();
                    if (viceCoachMember != null)
                    {
                        data.ViceCoach = viceCoachMember.member;
                    }
                }

            }
            catch
            {

            }
            if (school == null)
            {
                return data;
            }
            data.School = school;
            data.SetDisplayTeam(0);
            return data;
        }
    }
}
