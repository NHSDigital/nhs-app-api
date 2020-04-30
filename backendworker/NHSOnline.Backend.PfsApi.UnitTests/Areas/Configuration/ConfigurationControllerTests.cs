using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.Areas.Configuration;
using NHSOnline.Backend.PfsApi.Areas.Configuration.Models;
using NHSOnline.Backend.PfsApi.Configuration;
using NHSOnline.Backend.PfsApi.Devices;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Configuration
{
    [TestClass]
    public sealed class ConfigurationControllerTests: IDisposable
    {
        private ConfigurationController _systemUnderTest;
        private Mock<IConfigurationService> _mockDeviceConfigurationService;
        private Mock<ISupportedDeviceService> _mockSupportedDeviceService;
        private GetConfigurationQueryParameters _queryParameters;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _queryParameters = new GetConfigurationQueryParameters
            {
                DeviceName = "name",
                NativeAppVersion = "version"
            };
            
            _mockDeviceConfigurationService = new Mock<IConfigurationService>();
            _mockSupportedDeviceService = new Mock<ISupportedDeviceService>();
            
            _systemUnderTest = new ConfigurationController(
                new Mock<ILogger<ConfigurationController>>().Object,
                _mockSupportedDeviceService.Object,
                _mockDeviceConfigurationService.Object);
        }

        [TestMethod]
        public void Get_Returns_Success()
        {
            var response = new GetConfigurationResult.Success(true, new Uri("http://localhost/"));

            _mockSupportedDeviceService.Setup(x => x.IsDeviceSupported(It.IsAny<DeviceDetails>()))
                .Returns(response);

            // Act
            var result = _systemUnderTest.Get(_queryParameters);

            // Assert
            _mockSupportedDeviceService.VerifyAll();
            result.Should().BeAssignableTo<OkObjectResult>();
        }
        
        [TestMethod]
        public void Get_WhenServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockSupportedDeviceService.Setup(x => x.IsDeviceSupported(It.IsAny<DeviceDetails>()))
                .Returns(new GetConfigurationResult.InternalServerError());

            // Act
            var result = _systemUnderTest.Get(_queryParameters);

            // Assert
            _mockSupportedDeviceService.VerifyAll();
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
        
        [TestMethod]
        public void Get_DeviceDetailsNull_ReturnsBadRequest()
        {
            // Arrange
            _mockSupportedDeviceService.Setup(x => x.IsDeviceSupported(It.IsAny<DeviceDetails>()))
                .Returns(new GetConfigurationResult.BadRequest());

            // Act
            var result = _systemUnderTest.Get(_queryParameters);

            // Assert
            _mockSupportedDeviceService.VerifyAll();
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }
        
        [TestMethod]
        public void GetV2_Returns_Success()
        {
            // Arrange
            var response = new GetConfigurationResultV2(
                new List<string>(), 
                new Uri("http://localhost/"), 
                "min android version",
                "max iOS version",
                new List<RootService>());
            
            _mockDeviceConfigurationService.Setup(x => x.GetConfiguration()).Returns(response);
            
            // Act
            var result = _systemUnderTest.GetV2();

            // Assert
            _mockDeviceConfigurationService.VerifyAll();
            result.Should().BeAssignableTo<OkObjectResult>();
        }

        public void Dispose() => _systemUnderTest?.Dispose();
    }
}