using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.UsersApi.Areas.Devices;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.UnitTests.Areas.Devices
{
    [TestClass]
    public sealed class DevicesControllerTests : IDisposable
    {
        private IFixture _fixture;
        private DevicesController _systemUnderTest;
        private Mock<INotificationRegistrationService> _hubService;

        private RegisterDeviceRequest _validRegisterDeviceRequest;
        private Mock<IDeviceRepositoryService> _deviceRepositoryService;

        private const string DummyNhsLoginId = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());

            _hubService = new Mock<INotificationRegistrationService>();
            _deviceRepositoryService = new Mock<IDeviceRepositoryService>();
            var logger = new Mock<ILogger<DevicesController>>();

            _systemUnderTest = new DevicesController(_hubService.Object,
                _deviceRepositoryService.Object,
                logger.Object);

            _validRegisterDeviceRequest = _fixture.Create<RegisterDeviceRequest>();
        }

        [TestMethod]
        public async Task Post_Success()
        {
            // Arrange
            _hubService.Setup(x => x.Register(_validRegisterDeviceRequest, DummyNhsLoginId)).ReturnsAsync(_fixture.Create<RegistrationResult.Success>());

            var userDevice = _fixture.Create<UserDevice>();

            _deviceRepositoryService.Setup(x => x.Create(
                    It.IsAny<NotificationRegistrationResult>(), 
                    _validRegisterDeviceRequest))
                .ReturnsAsync(new DeviceRepositoryResult.Created(userDevice));

            // Act
            var result = await _systemUnderTest.Post(_validRegisterDeviceRequest);

            // Assert
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
            var objectResult = result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_NullDeviceType_ReturnsBadRequest()
        {
            // Arrange
            var registerDeviceRequest = _fixture.Build<RegisterDeviceRequest>()
                .With(x => x.DeviceType, (DeviceType?)null).Create();

            // Act
            var result = await _systemUnderTest.Post(registerDeviceRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_NullPnsToken_ReturnsBadRequest()
        {
            // Arrange
            var registerDeviceRequest =_fixture.Build<RegisterDeviceRequest>()
                    .With(x => x.DevicePns, (string) null).Create();

            // Act
            var result = await _systemUnderTest.Post(registerDeviceRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_HubServiceRegistrationFailure_ReturnsServiceUnavailable()
        {
            // Arrange
            _hubService.Setup(x => x.Register(_validRegisterDeviceRequest, DummyNhsLoginId)).ReturnsAsync(_fixture.Create<RegistrationResult.BadGateway>());
            
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
            _hubService.Setup(x => x.Register(_validRegisterDeviceRequest, DummyNhsLoginId)).Throws(new ArgumentException("Test"));

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
            _hubService.Setup(x => x.Register(_validRegisterDeviceRequest, DummyNhsLoginId)).ReturnsAsync(_fixture.Create<RegistrationResult.Success>());

            _deviceRepositoryService.Setup(x => x.Create(
                    It.IsAny<NotificationRegistrationResult>(),
                    _validRegisterDeviceRequest))
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
            _hubService.Setup(x => x.Register(_validRegisterDeviceRequest, DummyNhsLoginId)).ReturnsAsync(_fixture.Create<RegistrationResult.Success>());

            _deviceRepositoryService.Setup(x => x.Create(
                    It.IsAny<NotificationRegistrationResult>(),
                    _validRegisterDeviceRequest))
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
            _systemUnderTest.Dispose();
        }
    }
}
