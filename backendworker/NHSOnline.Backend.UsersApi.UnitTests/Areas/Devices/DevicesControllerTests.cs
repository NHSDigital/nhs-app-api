using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.UsersApi.Areas.Devices;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.UsersApi.UnitTests.Areas.Devices
{
    [TestClass]
    public sealed class DevicesControllerTests : IDisposable
    {
        private IFixture _fixture;
        private DevicesController _systemUnderTest;
        private RegisterDeviceRequest _validRegisterDeviceRequest;
        private Mock<INotificationRegistrationService> _hubService;
        private Mock<IDeviceRepositoryService> _deviceRepositoryService;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            var identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _fixture.Create<string>()),
                new Claim("nhs_number", _fixture.Create<string>())
            });

            var mockHttpContext = _fixture.Create<Mock<HttpContext>>();
            mockHttpContext.Setup(x => x.User)
                .Returns(new ClaimsPrincipal(identity));

            _hubService = _fixture.Freeze<Mock<INotificationRegistrationService>>();
            _deviceRepositoryService = _fixture.Freeze<Mock<IDeviceRepositoryService>>();
            _validRegisterDeviceRequest = _fixture.Create<RegisterDeviceRequest>();

            _systemUnderTest = _fixture.Create<DevicesController>();
            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };
        }

        [TestMethod]
        public async Task Post_Success()
        {
            // Arrange
            _hubService.Setup(x => x.Register(_validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(_fixture.Create<RegistrationResult.Success>());

            var userDevice = _fixture.Create<UserDevice>();

            _deviceRepositoryService.Setup(x => x.Create(
                    It.IsAny<NotificationRegistrationResult>(),
                    _validRegisterDeviceRequest,
                    It.IsAny<AccessToken>()))
                .ReturnsAsync(new DeviceRepositoryResult.Created(userDevice))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(_validRegisterDeviceRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<ObjectResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status201Created);
            
            _deviceRepositoryService.Verify();

            var resultBody = objectResult.Subject.Value.Should().BeAssignableTo<Device>().Subject;
            resultBody.DeviceType.Should().Be(_validRegisterDeviceRequest.DeviceType);
            resultBody.DeviceId.Should().Be(userDevice.DeviceId);
        }

        [TestMethod]
        public async Task Post_NullRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _systemUnderTest.Post(null);

            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_NullDeviceType_ReturnsBadRequest()
        {
            // Arrange
            var registerDeviceRequest = _fixture.Build<RegisterDeviceRequest>()
                .With(x => x.DeviceType, (DeviceType?) null).Create();

            // Act
            var result = await _systemUnderTest.Post(registerDeviceRequest);

            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_NullPnsToken_ReturnsBadRequest()
        {
            // Arrange
            var registerDeviceRequest = _fixture.Build<RegisterDeviceRequest>()
                .With(x => x.DevicePns, (string) null).Create();

            // Act
            var result = await _systemUnderTest.Post(registerDeviceRequest);

            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_HubServiceRegistrationFailure_ReturnsServiceUnavailable()
        {
            // Arrange
            _hubService.Setup(x => x.Register(_validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(_fixture.Create<RegistrationResult.BadGateway>());

            // Act
            var result = await _systemUnderTest.Post(_validRegisterDeviceRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Post_HubServiceRegistrationException_ReturnsInternalServerError()
        {
            // Arrange
            _hubService.Setup(x => x.Register(_validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Post(_validRegisterDeviceRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Post_DeviceRegistrationFailure_ReturnsServiceUnavailable()
        {
            // Arrange
            _hubService.Setup(x => x.Register(_validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(_fixture.Create<RegistrationResult.Success>());

            _deviceRepositoryService.Setup(x => x.Create(
                    It.IsAny<NotificationRegistrationResult>(),
                    _validRegisterDeviceRequest,
                    It.IsAny<AccessToken>()))
                .ReturnsAsync(new DeviceRepositoryResult.Failure());

            // Act
            var result = await _systemUnderTest.Post(_validRegisterDeviceRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Post_DeviceRegistrationException_ReturnsInternalServerError()
        {
            // Arrange
            _hubService.Setup(x => x.Register(_validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(_fixture.Create<RegistrationResult.Success>());

            _deviceRepositoryService.Setup(x => x.Create(
                    It.IsAny<NotificationRegistrationResult>(),
                    _validRegisterDeviceRequest,
                    It.IsAny<AccessToken>()))
                .Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Post(_validRegisterDeviceRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestCleanup]
        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}