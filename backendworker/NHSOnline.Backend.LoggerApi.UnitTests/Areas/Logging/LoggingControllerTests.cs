using System;
using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.LoggerApi.Areas.Logging;
using NHSOnline.Backend.LoggerApi.Areas.Logging.Models;
using NHSOnline.Backend.LoggerApi.Logging;
using UnitTestHelper;

namespace NHSOnline.Backend.LoggerApi.UnitTests.Areas.Logging
{   
    [TestClass]
    public class LoggingControllerTests
    {
        private IFixture _fixture;
        private Mock<ILoggingService> _mockLoggingService;
        private LoggingController _systemUnderTest;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _mockLoggingService = _fixture.Freeze<Mock<ILoggingService>>();
            
            _systemUnderTest = _fixture.Create<LoggingController>();
        }
        
        [TestMethod]
        public void Post_ReturnsSuccess_WhenLoggingServiceLogsMessageSuccessfully()
        {
            // Arrange
            CreateLogRequest createLogRequestModel = new CreateLogRequest
            {
                TimeStamp = DateTimeOffset.Now,
                Level = Level.Debug,
                Message = "test log message"
            };
           
            _mockLoggingService
                .Setup(x => x.LogMessage(createLogRequestModel))
                .Verifiable();
            
            // Act
            var result = _systemUnderTest.Post(createLogRequestModel);
            
            // Assert
            _mockLoggingService.Verify();
            var value = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            value.StatusCode.Should().Be((int) HttpStatusCode.OK);
        }
        
        [TestMethod]
        public void Post_ReturnsInternalServerError_WhenLoggingServiceThrowsException()
        {
            // Arrange
            CreateLogRequest createLogRequestModel = new CreateLogRequest
            {
                TimeStamp = DateTimeOffset.Now,
                Level = Level.Debug,
                Message = "test log message"
            };
           
            _mockLoggingService
                .Setup(x => x.LogMessage(createLogRequestModel))
                .Throws<Exception>()
                .Verifiable();
            
            // Act
            var result = _systemUnderTest.Post(createLogRequestModel);
            
            // Assert
            _mockLoggingService.Verify();
            var value = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            value.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
        }
    }
}
