using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private DevicesController _systemUnderTest;
        private Mock<INotificationService> _mockNotificationService;
        private Mock<IDeviceRepositoryService> _mockDeviceRepositoryService;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockNotificationService = new Mock<INotificationService>();
            _mockDeviceRepositoryService = new Mock<IDeviceRepositoryService>();

            _systemUnderTest = new DevicesController(
                _mockNotificationService.Object,
                _mockDeviceRepositoryService.Object,
                new Mock<ILogger<DevicesController>>().Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = HttpContextGetAccessTokenHelper.CreateMockHttpContext().Object
                }
            };
        }

        [TestMethod]
        public async Task Post_Success()
        {
            // Arrange
            var validRegisterDeviceRequest = CreateValidRegisterDeviceRequest();
            _mockNotificationService.Setup(x => x.Register(validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(new RegistrationResult.Success(new NotificationRegistrationResult()));

            var userDevice = new UserDevice();

            _mockDeviceRepositoryService.Setup(x => x.Create(
                    It.IsAny<NotificationRegistrationResult>(),
                    validRegisterDeviceRequest,
                    It.IsAny<AccessToken>()))
                .ReturnsAsync(new DeviceRegistrationResult.Created(userDevice));

            // Act
            var result = await _systemUnderTest.Post(validRegisterDeviceRequest);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<ObjectResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status201Created);

            var resultBody = objectResult.Subject.Value.Should().BeAssignableTo<Device>().Subject;
            resultBody.DeviceType.Should().Be(validRegisterDeviceRequest.DeviceType);
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
            var registerDeviceRequest = CreateValidRegisterDeviceRequest();
            registerDeviceRequest.DeviceType = null;

            // Act
            var result = await _systemUnderTest.Post(registerDeviceRequest);

            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_NullPnsToken_ReturnsBadRequest()
        {
            // Arrange
            var registerDeviceRequest = CreateValidRegisterDeviceRequest();
            registerDeviceRequest.DevicePns = null;

            // Act
            var result = await _systemUnderTest.Post(registerDeviceRequest);

            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_HubServiceRegistrationFailure_ReturnsBadGateway()
        {
            // Arrange
            var validRegisterDeviceRequest = CreateValidRegisterDeviceRequest();
            _mockNotificationService.Setup(x => x.Register(validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(new RegistrationResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Post(validRegisterDeviceRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Post_HubServiceInternalServerError_ReturnsInternalServerError()
        {
            // Arrange
            var validRegisterDeviceRequest = CreateValidRegisterDeviceRequest();
            _mockNotificationService.Setup(x => x.Register(validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(new RegistrationResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Post(validRegisterDeviceRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Post_HubServiceRegistrationException_ReturnsInternalServerError()
        {
            // Arrange
            var validRegisterDeviceRequest = CreateValidRegisterDeviceRequest();
            _mockNotificationService.Setup(x => x.Register(validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Post(validRegisterDeviceRequest);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Post_DeviceRegistrationInternalServerError_ReturnsInternalServerError()
        {
            // Arrange
            var validRegisterDeviceRequest = CreateValidRegisterDeviceRequest();
            _mockNotificationService.Setup(x => x.Register(validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(new RegistrationResult.Success(new NotificationRegistrationResult()));

            _mockDeviceRepositoryService.Setup(x => x.Create(
                    It.IsAny<NotificationRegistrationResult>(),
                    validRegisterDeviceRequest,
                    It.IsAny<AccessToken>()))
                .ReturnsAsync(new DeviceRegistrationResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Post(validRegisterDeviceRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Post_DeviceRegistrationBadGateway_ReturnsBadGateway()
        {
            // Arrange
            var validRegisterDeviceRequest = CreateValidRegisterDeviceRequest();
            _mockNotificationService.Setup(x => x.Register(validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(new RegistrationResult.Success(new NotificationRegistrationResult()));

            _mockDeviceRepositoryService.Setup(x => x.Create(
                    It.IsAny<NotificationRegistrationResult>(),
                    validRegisterDeviceRequest,
                    It.IsAny<AccessToken>()))
                .ReturnsAsync(new DeviceRegistrationResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Post(validRegisterDeviceRequest);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Post_DeviceRegistrationException_ReturnsInternalServerError()
        {
            // Arrange
            var validRegisterDeviceRequest = CreateValidRegisterDeviceRequest();
            _mockNotificationService.Setup(x => x.Register(validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(new RegistrationResult.Success(new NotificationRegistrationResult()));

            _mockDeviceRepositoryService.Setup(x => x.Create(
                    It.IsAny<NotificationRegistrationResult>(),
                    validRegisterDeviceRequest,
                    It.IsAny<AccessToken>())
                )
                .Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Post(validRegisterDeviceRequest);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        private static RegisterDeviceRequest CreateValidRegisterDeviceRequest()
            => new RegisterDeviceRequest
            {
                DevicePns = "PNS",
                DeviceType = DeviceType.Android
            };

        [TestCleanup]
        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}