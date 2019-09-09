using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.PfsApi.Areas.Session.Models;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Session
{
    [TestClass]
    public class SessionValidatorTests
    {
        private SessionValidator _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            var logger = new Mock<ILogger<SessionValidator>>();
            _systemUnderTest = new SessionValidator(logger.Object);
        }

        [DataTestMethod]
        [DataRow("", "", "")]
        [DataRow(null, "1234567890", "1234567890")]
        [DataRow("1234567890", null, "1234567890")]
        [DataRow("1234567890", "1234567890", null)]
        public void IsPostValid_InvalidData_ReturnsFalse(string authCode, string codeVerifier, string redirectUrl)
        {
            // Arrange
            var request = new UserSessionRequest
            {
                AuthCode = authCode,
                CodeVerifier = codeVerifier,
                RedirectUrl = redirectUrl
            };

            // Act
            var result = _systemUnderTest.IsPostValid(request);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsPostValid_ValidData_ReturnsTrue()
        {
            // Arrange
            var request = new UserSessionRequest
            {
                AuthCode = "1234567890",
                CodeVerifier = "1234567890",
                RedirectUrl = "12334567890"
            };

            // Act
            var result = _systemUnderTest.IsPostValid(request);

            // Assert
            result.Should().BeTrue();
        }
    }
}
