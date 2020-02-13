using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.Areas.Configuration;
using NHSOnline.Backend.PfsApi.Areas.Configuration.Models;
using NHSOnline.Backend.PfsApi.Configuration;
using NHSOnline.Backend.PfsApi.Devices;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Configuration
{
    [TestClass]
    public class ConfigurationControllerTests
    {
        private IFixture _fixture;
        
        private ConfigurationController _systemUnderTest;
        private Mock<IConfigurationService> _mockDeviceConfigurationService;
        private Mock<ISupportedDeviceService> _mockSupportedDeviceService;
        private GetConfigurationQueryParameters _queryParameters;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());
            
            _queryParameters = _fixture.Create<GetConfigurationQueryParameters>();
            
            _mockDeviceConfigurationService = _fixture.Freeze<Mock<IConfigurationService>>();
            _mockSupportedDeviceService = _fixture.Freeze<Mock<ISupportedDeviceService>>();
            
            _systemUnderTest = _fixture.Create<ConfigurationController>();
        }

        [TestMethod]
        public void Get_Returns_Success()
        {
            var response = new GetConfigurationResult.Success(
                _fixture.Create<bool>(),
                _fixture.Create<Uri>());

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
            var knownServices = _fixture.Create<KnownServices>();
            var response = new GetConfigurationResultV2.Success(
                _fixture.Create<Uri>(),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                knownServices.Services);
            
            _mockDeviceConfigurationService.Setup(x => x.GetConfiguration()).Returns(response);
            
            // Act
            var result = _systemUnderTest.GetV2();

            // Assert
            _mockDeviceConfigurationService.VerifyAll();
            result.Should().BeAssignableTo<OkObjectResult>();
        }
  
        [TestMethod]
        public void GetV2_WhenServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var response = new GetConfigurationResultV2.InternalServerError();
            _mockDeviceConfigurationService.Setup(x => x.GetConfiguration()).Returns(response);
            
            // Act
            var result = _systemUnderTest.GetV2();

            // Assert
            _mockDeviceConfigurationService.VerifyAll();
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }
}