using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.LoggerApi.Areas.Logging;
using NHSOnline.Backend.LoggerApi.Areas.Logging.Models;
using Moq;

namespace NHSOnline.Backend.LoggerApi.UnitTests.Areas.Logging
{   
    [TestClass]
    public class CreateLogRequestValidatorTests
    {
        private IFixture _fixture;
        private Mock<ILogger> _logger;
        private CreateLogRequestValidator _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

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
           
            var isValid = _systemUnderTest.ValidateAndSanitize(createLogRequest);
            
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
            var isValid = _systemUnderTest.ValidateAndSanitize(createLogRequest);
            
            // Assert
            isValid.Should().Be(false);
        }

        [DataTestMethod]
        [DataRow("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789().;:/", "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789().;:/")]
        [DataRow("m*e&s^s%a@g!e wi+t=h b?a#d c|h{a}r>a<cters", "message with bad characters")]
        [DataRow("There\r\nshould be spaces\r\nafter\r\nnew lines", "There\r\n should be spaces\r\n after\r\n new lines")]
        public void IsPostValid_KeepsWhiteListedCharacters_AndStripsOthers(string input, string expectedAfterValidation)
        {
            // Arrange
            var createLogRequest = new CreateLogRequest
            {
                Level = Level.Debug,
                Message = input,
                TimeStamp = new DateTimeOffset()
            };

            // Act
            var isValid = _systemUnderTest.ValidateAndSanitize(createLogRequest);

            // Assert
            isValid.Should().Be(true);
            createLogRequest.Message.Should().Be(expectedAfterValidation);
        }
    }
}
