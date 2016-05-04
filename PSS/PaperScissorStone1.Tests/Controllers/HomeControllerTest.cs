using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaperScissorStone1;
using PaperScissorStone1.Controllers;
using Moq;
using PaperScissorStoneCore;
using PaperScissorStone1.Models;

namespace PaperScissorStone1.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        // Moq: http://www.developerhandbook.com/unit-testing/writing-unit-tests-with-nunit-and-moq/

        [TestMethod]
        public void Index()
        {
            // Arrange
            var moqPlayerManager = new Mock<IPlayerManager>();
            HomeController controller = new HomeController(moqPlayerManager.Object);
            var model = new HomeViewModel() { };

            // Act
            ViewResult result = controller.Index(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result, "resulting ViewResult is null");
        }

        [TestMethod]
        public void Index_WithNull()
        {
            // Arrange
            var moqPlayerManager = new Mock<IPlayerManager>();
            HomeController controller = new HomeController(moqPlayerManager.Object);

            // Act
            ViewResult result = controller.Index(null) as ViewResult;

            // Assert
            Assert.IsNotNull(result, "resulting ViewResult is null");
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            var moqPlayerManager = new Mock<IPlayerManager>();
            HomeController controller = new HomeController(moqPlayerManager.Object);

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact()
        {
            var moqPlayerManager = new Mock<IPlayerManager>();
            HomeController controller = new HomeController(moqPlayerManager.Object);

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result, "resulting ViewResult is null");
        }

        [TestMethod]
        public void LoginRegister_CanLogIn()
        {
            // Arrange
            var moqPlayerManager = new Mock<IPlayerManager>();
            const string loginName = "Billybob";
            const string loginPassword = "letmein";

            moqPlayerManager
                .Setup(t => t.LogOn(loginName, loginPassword))
                .Returns(() => 1);

            HomeController controller = new HomeController(moqPlayerManager.Object);
            var model = new HomeViewModel() { Name = loginName, Password = loginPassword, IsLogin = true };

            // Act
            var result = controller.LoginRegister(model);

            // Assert            
            // Ensure the method LogOn was called once with expected parameters
            moqPlayerManager.Verify(v => v.LogOn(loginName, loginPassword), Times.Once, "LogOn was not called");            
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult), "expecting return to be redirect");
        }

        [TestMethod]
        public void LoginRegister_RejectWrongPassword()
        {

            // Arrange
            var moqPlayerManager = new Mock<IPlayerManager>();
            const string correctName = "Billybob";
            const string correctPassword = "letmein";
            const string wrongPassword = "wrongpassword";

            moqPlayerManager.Setup(c => c.LogOn(Moq.It.Is<string>(name => name == correctName),
                     Moq.It.Is<string>(password => password == correctPassword)))
                   .Returns(1);
           
            HomeController controller = new HomeController(moqPlayerManager.Object);
            var model = new HomeViewModel() { Name = "Billybob", Password = wrongPassword, IsLogin = true };

            // Act
            var result = controller.LoginRegister(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result, "not expecting view result to be null");
            Assert.IsTrue(result.ViewName == "Index", "expecting wrong password to cause redirection to Index");
        }

        [TestMethod]
        public void LoginRegister_CanRegister()
        {

            // Arrange
            var moqPlayerManager = new Mock<IPlayerManager>();
            const string registerName = "Billybob";
            const string registerPassword = "letmein";

            moqPlayerManager.Setup(c => c.Register(registerName, registerPassword))
                .Returns(1);

            HomeController controller = new HomeController(moqPlayerManager.Object);
            var model = new HomeViewModel() { Name = registerName, Password = registerPassword, ConfirmPassword = registerPassword, IsLogin = false };

            // Act
            var result = controller.LoginRegister(model);

            // Assert
            // Ensure the method Register was called once with expected parameters
            moqPlayerManager.Verify(v => v.Register(registerName, registerPassword), Times.Once, "Register was not called");
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult), "expecting return to be redirect");
        }

        [TestMethod]
        public void LoginRegister_RejectPasswordMismatch()
        {

            // Arrange
            var moqPlayerManager = new Mock<IPlayerManager>();
            const string registerName = "Billybob";
            const string registerPassword = "letmein";
            const string registerConfirmPassword = "lllmein";

            moqPlayerManager.Setup(c => c.Register(registerName, registerPassword))
                .Returns(1);

            HomeController controller = new HomeController(moqPlayerManager.Object);
            var model = new HomeViewModel() { Name = registerName, Password = registerPassword, ConfirmPassword = registerConfirmPassword, IsLogin = false };

            // Act
            var result = controller.LoginRegister(model) as ViewResult;

            // Assert
            Assert.IsTrue(model.Errors.Any(), "Expecting the model to contain errors");
            Assert.IsNotNull(result, "not expecting view result to be null");

        }


        [TestMethod]
        public void LoginRegister_RejectDuplicateName()
        {

            // Arrange
            var moqPlayerManager = new Mock<IPlayerManager>();
            const string registerName = "Billybob";
            const string registerPassword = "letmein";

            moqPlayerManager.Setup(c => c.IsDuplicateName(registerName))
                .Returns(true);

            HomeController controller = new HomeController(moqPlayerManager.Object);
            var model = new HomeViewModel() { Name = registerName, Password = registerPassword, ConfirmPassword = registerPassword, IsLogin = false };

            // Act
            var result = controller.LoginRegister(model) as ViewResult;

            // Assert
            Assert.IsTrue(model.Errors.Any(), "Expecting the model to contain errors");
            Assert.IsNotNull(result, "not expecting view result to be null");
        }
    }
}
