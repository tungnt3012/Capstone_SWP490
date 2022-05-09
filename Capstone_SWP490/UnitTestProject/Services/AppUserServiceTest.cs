using Capstone_SWP490;
using Capstone_SWP490.Models;
using Capstone_SWP490.Models.app_userViewModel;
using Capstone_SWP490.Repositories;
using Capstone_SWP490.Repositories.Interfaces;
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
    /// Summary description for AppUserServiceTest
    /// </summary>
    [TestClass]
    public class AppUserServiceTest
    {
        private readonly Iapp_userService _iapp_UserService = new app_userService();
        public AppUserServiceTest()
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
        public void GetAllUser()
        {
            //act
            List<app_user> actual = _iapp_UserService.GetAllUser();
            //assert
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public async Task CreateUser()
        {
            //act
            app_user user = new app_user();
            user.active = false;
            user.confirm_password = 0;
            user.email = "testunitcreateuser@gmail.com";
            user.encrypted_psw = "aaa";
            user.full_name = "the anh tran";
            user.insert_date = DateTime.Now + "";
            user.psw = "aaa";
            user.update_date = DateTime.Now + "";
            user.user_name = "testunitcreateuser@gmail.com";
            user.user_role = "MEMBER";
            user.verified = false;

            app_user actual = await _iapp_UserService.CreateUser(user);
            //assert
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void CheckLoginFail()
        {
            //act
            app_user user = new app_user();
            user.active = false;
            user.confirm_password = 0;
            user.email = "ken47376@zcrcd.com";
            user.encrypted_psw = "aaa";
            user.full_name = "the anh tran";
            user.insert_date = DateTime.Now + "";
            user.psw = "aaa";
            user.update_date = DateTime.Now + "";
            user.user_name = "ken47376@zcrcd.com";
            user.user_role = "MEMBER";
            user.verified = false;

            bool actual = _iapp_UserService.CheckLogin(user);
            //assert
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void CheckLoginSuccess()
        {
            //act
            app_user user = new app_user();
            user.active = false;
            user.confirm_password = 0;
            user.email = "ken47376@zcrcd.com";
            user.encrypted_psw = "aaa";
            user.full_name = "the anh tran";
            user.insert_date = DateTime.Now + "";
            user.psw = "abc123";
            user.update_date = DateTime.Now + "";
            user.user_name = "ken47376@zcrcd.com";
            user.user_role = "MEMBER";
            user.verified = false;

            bool actual = _iapp_UserService.CheckLogin(user);
            //assert
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public async Task UpdatePasswordFirstFail()
        {
            //act
            app_user user = new app_user();
            user.active = false;
            user.confirm_password = 0;
            user.email = "sew01195@jeoce.com";
            user.encrypted_psw = "6ZoYxCjLONXyYIU2eJIuAw==";
            user.full_name = "the anh tran";
            user.insert_date = DateTime.Now + "";
            user.psw = "123";
            user.update_date = DateTime.Now + "";
            user.user_name = "sew01195@jeoce.com";
            user.user_role = "MEMBER";
            user.verified = false;

            bool actual = await _iapp_UserService.UpdatePasswordFirst(user.user_name, user.psw, user.encrypted_psw, true);
            //assert
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public async Task UpdatePasswordFirstSuccess()
        {
            //act
            app_user user = new app_user();
            user.active = false;
            user.confirm_password = 0;
            user.email = "oxc85870@jiooq.com";
            user.encrypted_psw = "6ZoYxCjLONXyYIU2eJIuAw==";
            user.full_name = "the anh tran";
            user.insert_date = DateTime.Now + "";
            user.psw = "123";
            user.update_date = DateTime.Now + "";
            user.user_name = "oxc85870@jiooq.com";
            user.user_role = "MEMBER";
            user.verified = false;
            app_user userTest = _iapp_UserService.getByUserName(user.user_name);
            userTest.psw = "213test";
            userTest.verified = false;
            await _iapp_UserService.update(userTest);
            bool actual = await _iapp_UserService.UpdatePasswordFirst(user.user_name, user.psw, user.encrypted_psw, true);
            //assert
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public async Task UpdateSendNotify()
        {
            //act
            app_user user = new app_user();
            user.active = false;
            user.confirm_password = 0;
            user.email = "oxc85870@jiooq.com";
            user.encrypted_psw = "6ZoYxCjLONXyYIU2eJIuAw==";
            user.full_name = "the anh tran";
            user.insert_date = DateTime.Now + "";
            user.psw = "123";
            user.update_date = DateTime.Now + "";
            user.user_name = "oxc85870@jiooq.com";
            user.user_role = "MEMBER";
            user.verified = false;
            app_user userTest = _iapp_UserService.getByUserName(user.user_name);
            userTest.psw = "213test";
            userTest.verified = false;
            await _iapp_UserService.update(userTest);
            bool actual = await _iapp_UserService.UpdatePasswordFirst(user.user_name, user.psw, user.encrypted_psw, true);
            //assert
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public async Task UpdatePasswordNotFoundUserName()
        {
            //act
            app_user user = new app_user();
            user.active = false;
            user.confirm_password = 0;
            user.email = "oxc85870@jiooq.com";
            user.encrypted_psw = "6ZoYxCjLONXyYIU2eJIuAw==";
            user.full_name = "the anh tran";
            user.insert_date = DateTime.Now + "";
            user.psw = "123";
            user.update_date = DateTime.Now + "";
            user.user_name = "testUT";
            user.user_role = "MEMBER";
            user.verified = false;
            bool actual = await _iapp_UserService.UpdatePassword(user.user_name, user.psw, user.encrypted_psw);
            //assert
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public async Task UpdatePasswordSameWithCurrent()
        {
            //act
            app_user user = new app_user();
            user.active = false;
            user.confirm_password = 0;
            user.email = "oxc85870@jiooq.com";
            user.encrypted_psw = "6ZoYxCjLONXyYIU2eJIuAw==";
            user.full_name = "the anh tran";
            user.insert_date = DateTime.Now + "";
            user.psw = "123";
            user.update_date = DateTime.Now + "";
            user.user_name = "oxc85870@jiooq.com";
            user.user_role = "MEMBER";
            user.verified = false;
            app_user userTest = _iapp_UserService.getByUserName(user.user_name);
            userTest.psw = "123";
            userTest.verified = false;
            await _iapp_UserService.update(userTest);
            bool actual = await _iapp_UserService.UpdatePassword(user.user_name, user.psw, user.encrypted_psw);
            //assert
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public async Task UpdatePasswordSuccess()
        {
            //act
            app_user user = new app_user();
            user.active = false;
            user.confirm_password = 0;
            user.email = "oxc85870@jiooq.com";
            user.encrypted_psw = "6ZoYxCjLONXyYIU2eJIuAw==";
            user.full_name = "the anh tran";
            user.insert_date = DateTime.Now + "";
            user.psw = "abc123";
            user.update_date = DateTime.Now + "";
            user.user_name = "oxc85870@jiooq.com";
            user.user_role = "MEMBER";
            user.verified = false;
            app_user userTest = _iapp_UserService.getByUserName(user.user_name);
            userTest.psw = "213test";
            userTest.verified = false;
            await _iapp_UserService.update(userTest);
            bool actual = await _iapp_UserService.UpdatePassword(user.user_name, user.psw, user.encrypted_psw);
            //assert
            Assert.AreEqual(true, actual);
        }


        [TestMethod]
        public void GetUserByUsernameNotFound()
        {
            //act
            app_userViewModel actual = _iapp_UserService.GetUserByUsername("testUTK");
            //assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void GetUserByUsername()
        {
            //act
            app_userViewModel actual = _iapp_UserService.GetUserByUsername("mathlover1998@gmail.com");
            //assert
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void getByUserId()
        {
            //act
            app_user actual = _iapp_UserService.getByUserId(10);
            //assert
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void getByUserName()
        {
            //act
            app_user actual = _iapp_UserService.getByUserName("oxc85870@jiooq.com");
            //assert
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void isEmailInUseTrue()
        {
            //act
            bool actual = _iapp_UserService.isEmailInUse("ywj86250@zcrcd.com", 8);
            //assert
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void isEmailInUseFalse()
        {
            //act
            bool actual = _iapp_UserService.isEmailInUse("testuta@gmail.com", 7);
            //assert
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public async Task update()
        {
            //act
            app_user user = _iapp_UserService.getByUserId(7);
            int actual = await _iapp_UserService.update(user);
            //assert
            Assert.AreEqual(1, actual);
        }

        [TestMethod]
        public async Task updatefail()
        {
            try
            {
                //act
                int actual = await _iapp_UserService.update(null);
                //assert
                Assert.Fail("no exception thrown");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is NullReferenceException);
            }
        }

        [TestMethod]
        public void findCoachWithoutkeyWord()
        {
            //act
            List<app_user> actual = _iapp_UserService.findCoach("ALL", "");
            //assert
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void findCoachWithkeyWord()
        {
            //act
            List<app_user> actual = _iapp_UserService.findCoach("se", "");
            //assert
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GetListUsersManager()
        {
            //act
            PagingOutput<List<app_userViewModel>> actual = _iapp_UserService.GetListUsersManager(10);
            //assert
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public async Task SwitchableUsersFail()
        {
            //act
            bool actual = await _iapp_UserService.SwitchableUsers(10000, false);
            //assert
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public async Task SwitchableUsersSuccess()
        {
            //act
            bool actual = await _iapp_UserService.SwitchableUsers(10, false);
            //assert
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void findNewRegistCoachWithoutkeyword()
        {
            //act
            List<app_user> actual = _iapp_UserService.findNewRegistCoach("");
            //assert
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void findNewRegistCoachWithkeyword()
        {
            //act
            List<app_user> actual = _iapp_UserService.findNewRegistCoach("se");
            //assert
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void findNewRegistCoachWithkeywordNotFound()
        {
            //act
            List<app_user> actual = _iapp_UserService.findNewRegistCoach("testneee");
            //assert
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public async Task ForgotPasswordSuccess()
        {
            //act
            bool actual = await _iapp_UserService.ForgotPassword("tungntse05838@fpt.edu.vn");
            //assert
            Assert.AreEqual(true, actual);
        }


        [TestMethod]
        public async Task ForgotPasswordFail()
        {
            //act
            bool actual = await _iapp_UserService.ForgotPassword("aas123@fpt.edu.vn");
            //assert
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public async Task ResetPasswordSuccess()
        {
            //act
            app_user userCurrent = _iapp_UserService.getByUserName("tungntse05838@fpt.edu.vn");
            userCurrent.psw = "OWYViu9a";
            userCurrent.encrypted_psw = "+GDb/Vr9WP+1enMJzIp/Fg==";
            await _iapp_UserService.update(userCurrent);

            bool actual = await _iapp_UserService.ResetPassword("tungntse05838@fpt.edu.vn", "testreset");
            //assert
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public async Task ResetPasswordSameWithCurrent()
        {
            //act
            app_user userCurrent = _iapp_UserService.getByUserName("tungntse05838@fpt.edu.vn");

            bool actual = await _iapp_UserService.ResetPassword(userCurrent.user_name, userCurrent.psw);
            //assert
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public async Task ResetPasswordNotFoundUser()
        {
            //act
            bool actual = await _iapp_UserService.ResetPassword("asdks@fpt.edu.vn", "abc123");
            //assert
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public async Task CreateOrganizer()
        {
            //act
            app_userViewModel userIn = _iapp_UserService.GetUserByUsername("anhtts2e06009@fpt.edu.vn");
            userIn.email = DateTime.Now + "@gmail.com";
            userIn.user_name = userIn.email;
            userIn.user_role = "ORGANIZER";
            userIn.active = false;
            userIn.verified = false;
            app_userViewModel actual = await _iapp_UserService.CreateOrganizer(userIn);
            //assert
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public async Task CreateOrganizerWithoutFullName()
        {
            //act
            app_userViewModel userIn = _iapp_UserService.GetUserByUsername("anhtts2e06009@fpt.edu.vn");
            userIn.full_name = null;
            app_userViewModel actual = await _iapp_UserService.CreateOrganizer(userIn);
            //assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public async Task CreateOrganizerWithoutEmail()
        {
            //act
            app_userViewModel userIn = _iapp_UserService.GetUserByUsername("anhtts2e06009@fpt.edu.vn");
            userIn.email = null;
            app_userViewModel actual = await _iapp_UserService.CreateOrganizer(userIn);
            //assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public async Task CreateOrganizerWithDuplicateEmail()
        {
            //act
            app_userViewModel userIn = _iapp_UserService.GetUserByUsername("anhtts2e06009@fpt.edu.vn");
            app_userViewModel actual = await _iapp_UserService.CreateOrganizer(userIn);
            //assert
            Assert.IsNull(actual);
        }

    }
}
