using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bitly.Controllers;

namespace Bitly.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();
            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Укоротить ссылку", result.ViewBag.Title);
        }

        [TestMethod]
        public void Statistics()
        {
            // Arrange
            HomeController controller = new HomeController();
            // Act
            ViewResult result = controller.ShortLinksStatistics() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Ваши ссылки", result.ViewBag.Title);
        }
    }
}
