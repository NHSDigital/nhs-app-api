using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Logger.Areas.Logging.Models;
using NHSOnline.Backend.PfsApi.Areas.Logging;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Logging
{
    [TestClass]
    public class CreateLogRequestValidatorTests
    {
        private Mock<ILogger> _logger;
        private CreateLogRequestValidator _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = new Mock<ILogger>();
            _systemUnderTest = new CreateLogRequestValidator(_logger.Object);
        }

        [TestMethod]
        public void IsPostValid_ReturnsTrue_IfLogRequestIsValid()
        {
            // Arrange
            var createLogRequest = new CreateLogRequest
            {
                Level = Level.Error,
                Message = "test log message",
                TimeStamp = DateTimeOffset.Now
            };

            // Act

            var isValid = _systemUnderTest.Validate(createLogRequest);

            // Assert
            isValid.Should().Be(true);
        }

        [TestMethod]
        public void IsPostValid_ReturnsFalse_IfLogMessageIsNull()
        {
            // Arrange
            var createLogRequest = new CreateLogRequest
            {
                Level = Level.Debug,
                Message = "",
                TimeStamp = new DateTimeOffset()
            };

            // Act
            var isValid = _systemUnderTest.Validate(createLogRequest);

            // Assert
            isValid.Should().Be(false);
        }
    }
}
