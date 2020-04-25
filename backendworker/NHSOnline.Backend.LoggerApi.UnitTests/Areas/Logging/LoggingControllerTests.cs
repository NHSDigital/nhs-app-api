using System;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.LoggerApi.Areas.Logging;
using NHSOnline.Backend.LoggerApi.Areas.Logging.Models;
using NHSOnline.Backend.LoggerApi.Logging;

namespace NHSOnline.Backend.LoggerApi.UnitTests.Areas.Logging
{   
    [TestClass]
    public sealed class LoggingControllerTests : IDisposable
    {
        private Mock<ILoggingService> _mockLoggingService;
        private LoggingController _systemUnderTest;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _mockLoggingService = new Mock<ILoggingService>();
            
            _systemUnderTest = new LoggingController(
                new Mock<ILogger<LoggingController>>().Object,
                _mockLoggingService.Object);
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

        public void Dispose() => _systemUnderTest?.Dispose();
    }
}
