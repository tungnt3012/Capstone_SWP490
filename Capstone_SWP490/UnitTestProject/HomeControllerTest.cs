using Capstone_SWP490.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTestProject
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void ShirtSizing()
        {
            var controller = new HomeController();
            var index = controller.ShirtSizing();
            Assert.IsNotNull(index);
        }
        [TestMethod]
        public void Scoreboard()
        {
            var controller = new HomeController();
            var index = controller.Scoreboard();
            Assert.IsNotNull(index);
        }
        [TestMethod]
        public void Error()
        {
            var controller = new HomeController();
            var index = controller.Error();
            Assert.IsNotNull(index);
        }
        
        
    }
}
