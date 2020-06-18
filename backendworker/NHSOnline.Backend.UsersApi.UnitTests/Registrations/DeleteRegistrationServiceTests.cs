using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Registrations;
using NHSOnline.Backend.UsersApi.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.UsersApi.UnitTests.Registrations
{
    [TestClass]
    public sealed class DeleteRegistrationServiceTests
    {
        private RegistrationService _systemUnderTest;
        private Mock<INotificationRegistrationService> _mockNotificationService;
        private Mock<IDeviceRepositoryService> _mockDeviceRepositoryService;
        private const string NhsNumber = "NhsNumber";
        private const string NhsLoginId = "NhsLoginId";
        private const string DevicePns = "device PNS";
        private const string DeviceId = "DeviceId";
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
        public async Task DeleteRegistration_SuccessResult()
        {
            // Arrange
            var userDevice = new UserDevice { PnsToken = DevicePns };

            _mockDeviceRepositoryService.Setup(x => x.Find(DevicePns, _accessToken))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockDeviceRepositoryService.Setup(x => x.Delete(userDevice.DeviceId, _accessToken))
                .ReturnsAsync(new DeleteDeviceResult.Success(DeviceId));

            _mockNotificationService.Setup(x => x.Delete(userDevice.RegistrationId))
                .ReturnsAsync(new DeleteRegistrationResult.Success());

            // Act
            var result = await _systemUnderTest.DeleteRegistration(DevicePns, _accessToken);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.Success>();
        }

        [TestMethod]
        public async Task DeleteRegistration_DeleteRegistrationNotFound_SuccessResult()
        {
            // Arrange
            var userDevice = new UserDevice { PnsToken = DevicePns };

            _mockDeviceRepositoryService.Setup(x => x.Find(DevicePns, _accessToken))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockDeviceRepositoryService.Setup(x => x.Delete(userDevice.DeviceId, _accessToken))
                .ReturnsAsync(new DeleteDeviceResult.Success(DeviceId));

            _mockNotificationService.Setup(x => x.Delete(userDevice.RegistrationId))
                .ReturnsAsync(new DeleteRegistrationResult.NotFound());

            // Act
            var result = await _systemUnderTest.DeleteRegistration(DevicePns, _accessToken);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.Success>();
        }

        [TestMethod]
        public async Task DeleteRegistration_NotFoundUserDevice_ReturnsNotFoundResult()
        {
            // Arrange
            _mockDeviceRepositoryService.Setup(x => x.Find(DevicePns, _accessToken))
                .ReturnsAsync(new SearchDeviceResult.NotFound());

            // Act
            var result = await _systemUnderTest.DeleteRegistration(DevicePns, _accessToken);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.NotFound>();
        }

        [TestMethod]
        public async Task DeleteRegistration_FindDeviceException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            _mockDeviceRepositoryService.Setup(x => x.Find(DevicePns, _accessToken))
                .ReturnsAsync(new SearchDeviceResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.DeleteRegistration(DevicePns, _accessToken);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.InternalServerError>();
        }

        [TestMethod]
        public async Task DeleteRegistration_FindDeviceBadGateway_ReturnsBadGatewayResult()
        {
            // Arrange
            _mockDeviceRepositoryService.Setup(x => x.Find(DevicePns, _accessToken))
                .ReturnsAsync(new SearchDeviceResult.BadGateway());

            // Act
            var result = await _systemUnderTest.DeleteRegistration(DevicePns, _accessToken);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.BadGateway>();
        }

        [TestMethod]
        public async Task DeleteRegistration_DeleteRegistrationException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            var userDevice = new UserDevice { PnsToken = DevicePns };

            _mockDeviceRepositoryService.Setup(x => x.Find(DevicePns, _accessToken))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockNotificationService.Setup(x => x.Delete(userDevice.RegistrationId))
                .ReturnsAsync(new DeleteRegistrationResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.DeleteRegistration(DevicePns, _accessToken);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.InternalServerError>();
         }

        [TestMethod]
        public async Task DeleteRegistration_DeleteRegistrationBadGateway_ReturnsBadGatewayResult()
        {
            // Arrange
            var userDevice = new UserDevice { PnsToken = DevicePns };

            _mockDeviceRepositoryService.Setup(x => x.Find(DevicePns, _accessToken))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockNotificationService.Setup(x => x.Delete(userDevice.RegistrationId))
                .ReturnsAsync(new DeleteRegistrationResult.BadGateway());

            // Act
            var result = await _systemUnderTest.DeleteRegistration(DevicePns, _accessToken);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.BadGateway>();
        }

        [TestMethod]
        public async Task DeleteRegistration_DeleteDeviceException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            var userDevice = new UserDevice { PnsToken = DevicePns };

            _mockDeviceRepositoryService.Setup(x => x.Find(DevicePns, _accessToken))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockDeviceRepositoryService.Setup(x => x.Delete(userDevice.DeviceId, _accessToken))
                .ReturnsAsync(new DeleteDeviceResult.InternalServerError());

            _mockNotificationService.Setup(x => x.Delete(userDevice.RegistrationId))
                .ReturnsAsync(new DeleteRegistrationResult.Success());

            // Act
            var result = await _systemUnderTest.DeleteRegistration(DevicePns, _accessToken);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.InternalServerError>();
        }

        [TestMethod]
        public async Task DeleteRegistration_DeleteDeviceBadGateway_ReturnsBadGatewayResult()
        {
            // Arrange
            var userDevice = new UserDevice { PnsToken = DevicePns };

            _mockDeviceRepositoryService.Setup(x => x.Find(DevicePns, _accessToken))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockDeviceRepositoryService.Setup(x => x.Delete(userDevice.DeviceId, _accessToken))
                .ReturnsAsync(new DeleteDeviceResult.BadGateway());

            _mockNotificationService.Setup(x => x.Delete(userDevice.RegistrationId))
                .ReturnsAsync(new DeleteRegistrationResult.Success());

            // Act
            var result = await _systemUnderTest.DeleteRegistration(DevicePns, _accessToken);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.BadGateway>();
        }

        [TestMethod]
        public async Task DeleteRegistration_DeleteDeviceFromRepositoryBadGateway_ReturnsBadGatewayResult()
        {
            // Arrange
            var userDevice = new UserDevice { PnsToken = DevicePns };

            _mockDeviceRepositoryService.Setup(x => x.Find(DevicePns, _accessToken))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockNotificationService.Setup(x => x.Delete(userDevice.RegistrationId))
                .ReturnsAsync(new DeleteRegistrationResult.Success());

            _mockDeviceRepositoryService.Setup(x => x.Delete(userDevice.DeviceId, _accessToken))
                .ReturnsAsync(new DeleteDeviceResult.BadGateway());

            // Act
            var result = await _systemUnderTest.DeleteRegistration(DevicePns, _accessToken);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.BadGateway>();
        }

        [TestMethod]
        public async Task DeleteRegistration_DeleteDeviceFromRepositoryInternalServerError_ReturnsInternalServerErrorResult()
        {
            // Arrange
            var userDevice = new UserDevice { PnsToken = DevicePns };
            
            _mockDeviceRepositoryService.Setup(x => x.Find(DevicePns, _accessToken))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockNotificationService.Setup(x => x.Delete(userDevice.RegistrationId))
                .ReturnsAsync(new DeleteRegistrationResult.Success());

            _mockDeviceRepositoryService.Setup(x => x.Delete(userDevice.DeviceId, _accessToken))
                .ReturnsAsync(new DeleteDeviceResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.DeleteRegistration(DevicePns, _accessToken);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            result.Should().BeOfType<DeleteRegistrationResult.InternalServerError>();
        }

        [TestMethod]
        public async Task DeleteRegistration_DeviceDeletionException_ThrowsAnException()
        {
            // Arrange
            _mockDeviceRepositoryService.Setup(x => x.Find(DevicePns, _accessToken))
                .ThrowsAsync(new ArgumentException("Test"));

            // Act
            Func<Task> act = async () => await _systemUnderTest.DeleteRegistration(DevicePns, _accessToken);

            //Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Test");

            _mockDeviceRepositoryService.VerifyAll();
        }
    }
}