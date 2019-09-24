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
    public sealed class DevicesControllerPostTests : IDisposable
    {
        private IFixture _fixture;
        private DevicesController _systemUnderTest;
        private RegisterDeviceRequest _validRegisterDeviceRequest;
        private Mock<INotificationService> _mockNotificationService;
        private Mock<IDeviceRepositoryService> _mockDeviceRepositoryService;

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

            _mockNotificationService = _fixture.Freeze<Mock<INotificationService>>();
            _mockDeviceRepositoryService = _fixture.Freeze<Mock<IDeviceRepositoryService>>();
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
            _mockNotificationService.Setup(x => x.Register(_validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(_fixture.Create<RegistrationResult.Success>());

            var userDevice = _fixture.Create<UserDevice>();

            _mockDeviceRepositoryService.Setup(x => x.Create(
                    It.IsAny<NotificationRegistrationResult>(),
                    _validRegisterDeviceRequest,
                    It.IsAny<AccessToken>()))
                .ReturnsAsync(new DeviceRegistrationResult.Created(userDevice));

            // Act
            var result = await _systemUnderTest.Post(_validRegisterDeviceRequest);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<ObjectResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status201Created);

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
            _mockNotificationService.Setup(x => x.Register(_validRegisterDeviceRequest, It.IsAny<AccessToken>()))
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
            _mockNotificationService.Setup(x => x.Register(_validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Post(_validRegisterDeviceRequest);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Post_DeviceRegistrationInternalServerError_ReturnsInternalServerError()
        {
            // Arrange
            _mockNotificationService.Setup(x => x.Register(_validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(_fixture.Create<RegistrationResult.Success>());

            _mockDeviceRepositoryService.Setup(x => x.Create(
                    It.IsAny<NotificationRegistrationResult>(),
                    _validRegisterDeviceRequest,
                    It.IsAny<AccessToken>()))
                .ReturnsAsync(new DeviceRegistrationResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Post(_validRegisterDeviceRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Post_DeviceRegistrationBadGateway_ReturnsServiceUnavailable()
        {
            // Arrange
            _mockNotificationService.Setup(x => x.Register(_validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(_fixture.Create<RegistrationResult.Success>());

            _mockDeviceRepositoryService.Setup(x => x.Create(
                    It.IsAny<NotificationRegistrationResult>(),
                    _validRegisterDeviceRequest,
                    It.IsAny<AccessToken>()))
                .ReturnsAsync(new DeviceRegistrationResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Post(_validRegisterDeviceRequest);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Post_DeviceRegistrationException_ReturnsInternalServerError()
        {
            // Arrange
            _mockNotificationService.Setup(x => x.Register(_validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(_fixture.Create<RegistrationResult.Success>());

            _mockDeviceRepositoryService.Setup(x => x.Create(
                    It.IsAny<NotificationRegistrationResult>(),
                    _validRegisterDeviceRequest,
                    It.IsAny<AccessToken>())
                )
                .Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Post(_validRegisterDeviceRequest);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestCleanup]
        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}