using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Registrations;
using NHSOnline.Backend.UsersApi.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.UsersApi.UnitTests.Registrations
{
    [TestClass]
    public sealed class CreateRegistrationServiceTests
    {
        private RegistrationService _systemUnderTest;
        private Mock<INotificationRegistrationService> _mockNotificationService;
        private Mock<IDeviceRepositoryService> _mockDeviceRepositoryService;
        private const string NhsNumber = "NhsNumber";
        private const string NhsLoginId = "NhsLoginId";
        private const string DevicePns = "PNS";
        private readonly DeviceType _deviceType = DeviceType.Android;
        private AccessToken _accessToken;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockNotificationService = new Mock<INotificationRegistrationService>();
            _mockDeviceRepositoryService = new Mock<IDeviceRepositoryService>();

            _systemUnderTest = new RegistrationService(
                _mockDeviceRepositoryService.Object,
                _mockNotificationService.Object,
                new Mock<ILogger<RegistrationService>>().Object);

            _accessToken = AccessTokenMock.Generate(NhsLoginId, NhsNumber);
        }

        [TestMethod]
        public async Task CreateRegistration_Success()
        {
            // Arrange
            var validRegisterDeviceRequest = CreateValidRegisterDeviceRequest();
            var userDevice = new UserDevice();
            var expectedResult = new RegisterDeviceResult.Created(userDevice);

            _mockNotificationService.Setup(x => x.Register(DevicePns, _deviceType, _accessToken))
                .ReturnsAsync(new RegistrationResult.Success(new NotificationRegistrationResult()));

            _mockDeviceRepositoryService.Setup(x => x.Create(
                    It.IsAny<NotificationRegistrationResult>(),
                    validRegisterDeviceRequest,
                    _accessToken))
                .ReturnsAsync(new RegisterDeviceResult.Created(userDevice));

            // Act
            var result = await _systemUnderTest.CreateRegistration(validRegisterDeviceRequest, _accessToken);

            // Assert
            _mockNotificationService.VerifyAll();
            _mockDeviceRepositoryService.VerifyAll();

            result.Should().BeOfType<RegisterDeviceResult.Created>();
            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task CreateRegistration_HubServiceRegistrationFailure_ReturnsBadGateway()
        {
            // Arrange
            var validRegisterDeviceRequest = CreateValidRegisterDeviceRequest();
            _mockNotificationService.Setup(x => x.Register(DevicePns, _deviceType, _accessToken))
                .ReturnsAsync(new RegistrationResult.BadGateway());

            // Act
            var result = await _systemUnderTest.CreateRegistration(validRegisterDeviceRequest, _accessToken);

            // Assert
            _mockNotificationService.VerifyAll();

            result.Should().BeOfType<RegisterDeviceResult.BadGateway>();
        }

        [TestMethod]
        public async Task CreateRegistration_HubServiceInternalServerError_ReturnsInternalServerError()
        {
            // Arrange
            var validRegisterDeviceRequest = CreateValidRegisterDeviceRequest();
            _mockNotificationService.Setup(x => x.Register(DevicePns, _deviceType, _accessToken))
                .ReturnsAsync(new RegistrationResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.CreateRegistration(validRegisterDeviceRequest, _accessToken);

            // Assert
            _mockNotificationService.VerifyAll();

            result.Should().BeOfType<RegisterDeviceResult.InternalServerError>();
        }

        [TestMethod]
        public async Task CreateRegistration_HubServiceRegistrationException_ReturnsInternalServerError()
        {
            // Arrange
            var validRegisterDeviceRequest = CreateValidRegisterDeviceRequest();
            _mockNotificationService.Setup(x => x.Register(DevicePns, _deviceType, _accessToken))
                .Throws(new ArgumentException("Test"));

            // Act
            Func<Task> act = async () => await _systemUnderTest.CreateRegistration(validRegisterDeviceRequest, _accessToken);

            //Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Test");

            _mockNotificationService.VerifyAll();
        }

        [TestMethod]
        public async Task CreateRegistration_DeviceRegistrationInternalServerError_ReturnsInternalServerError()
        {
            // Arrange
            var validRegisterDeviceRequest = CreateValidRegisterDeviceRequest();
            _mockNotificationService.Setup(x => x.Register(DevicePns, _deviceType, _accessToken))
                .ReturnsAsync(new RegistrationResult.Success(new NotificationRegistrationResult()));

            _mockDeviceRepositoryService.Setup(x => x.Create(
                    It.IsAny<NotificationRegistrationResult>(),
                    validRegisterDeviceRequest,
                    _accessToken))
                .ReturnsAsync(new RegisterDeviceResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.CreateRegistration(validRegisterDeviceRequest, _accessToken);

            // Assert
            _mockNotificationService.VerifyAll();
            _mockDeviceRepositoryService.VerifyAll();
            
            result.Should().BeOfType<RegisterDeviceResult.InternalServerError>();
        }

        [TestMethod]
        public async Task CreateRegistration_DeviceRegistrationBadGateway_ReturnsBadGateway()
        {
            // Arrange
            var validRegisterDeviceRequest = CreateValidRegisterDeviceRequest();
            _mockNotificationService.Setup(x => x.Register(DevicePns, _deviceType, _accessToken))
                .ReturnsAsync(new RegistrationResult.Success(new NotificationRegistrationResult()));

            _mockDeviceRepositoryService.Setup(x => x.Create(
                    It.IsAny<NotificationRegistrationResult>(),
                    validRegisterDeviceRequest,
                    _accessToken))
                .ReturnsAsync(new RegisterDeviceResult.BadGateway());

            // Act
            var result = await _systemUnderTest.CreateRegistration(validRegisterDeviceRequest, _accessToken);

            // Assert
            _mockNotificationService.VerifyAll();
            _mockDeviceRepositoryService.VerifyAll();

            result.Should().BeOfType<RegisterDeviceResult.BadGateway>();
        }

        [TestMethod]
        public async Task CreateRegistration_DeviceRegistrationException_ReturnsInternalServerError()
        {
            // Arrange
            var validRegisterDeviceRequest = CreateValidRegisterDeviceRequest();
            _mockNotificationService.Setup(x => x.Register(DevicePns, _deviceType, _accessToken))
                .ReturnsAsync(new RegistrationResult.Success(new NotificationRegistrationResult()));

            _mockDeviceRepositoryService.Setup(x => x.Create(
                    It.IsAny<NotificationRegistrationResult>(),
                    validRegisterDeviceRequest,
                    _accessToken)
                )
                .Throws(new ArgumentException("Test"));

            // Act
            Func<Task> act = async () => await _systemUnderTest.CreateRegistration(validRegisterDeviceRequest, _accessToken);

            //Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Test");
            
            _mockNotificationService.VerifyAll();
            _mockDeviceRepositoryService.VerifyAll();
        }

        private RegisterDeviceRequest CreateValidRegisterDeviceRequest()
            => new RegisterDeviceRequest
            {
                DevicePns = DevicePns,
                DeviceType = _deviceType
            };
    }
}