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
    public sealed class RegistrationServiceTests
    {
        private RegistrationService _systemUnderTest;
        private Mock<INotificationRegistrationService> _mockNotificationService;
        private Mock<IDeviceRepositoryService> _mockDeviceRepositoryService;
        private const string NhsNumber = "NhsNumber";
        private const string NhsLoginId = "NhsLoginId";
        private const string DevicePns = "device PNS";
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
        public async Task GetRegistration_SuccessResult()
        {
            // Arrange
            var userDevice = new UserDevice { PnsToken = DevicePns };

            _mockDeviceRepositoryService.Setup(x => x.Find(DevicePns, _accessToken))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockNotificationService.Setup(x => x.Exists(userDevice))
                .ReturnsAsync(new RegistrationExistsResult.Found());

            // Act
            var result = await _systemUnderTest.GetRegistration(DevicePns, _accessToken);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            result.Should().BeOfType<RegistrationExistsResult.Found>();
        }

        [TestMethod]
        public async Task GetRegistration_NotFoundUserDevice_ReturnsNotFoundResult()
        {
            // Arrange
            _mockDeviceRepositoryService.Setup(x => x.Find(DevicePns, _accessToken))
                .ReturnsAsync(new SearchDeviceResult.NotFound());

            // Act
            var result = await _systemUnderTest.GetRegistration(DevicePns, _accessToken);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();

            result.Should().BeOfType<RegistrationExistsResult.NotFound>();
        }

        [TestMethod]
        public async Task GetRegistration_FoundOrphanUserDevice_DeletesOrphanAndReturnsNotFoundResult()
        {
            // Arrange
            var userDevice = new UserDevice { PnsToken = DevicePns };

            _mockDeviceRepositoryService.Setup(x => x.Find(DevicePns, _accessToken))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockDeviceRepositoryService.Setup(x => x.Delete(userDevice.DeviceId, NhsLoginId))
                .ReturnsAsync(new DeleteDeviceResult.Success());

            _mockNotificationService.Setup(x => x.Exists(userDevice))
                .ReturnsAsync(new RegistrationExistsResult.NotFound());

            // Act
            var result = await _systemUnderTest.GetRegistration(DevicePns, _accessToken);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            result.Should().BeOfType<RegistrationExistsResult.NotFound>();
        }

        [TestMethod]
        public async Task GetRegistration_FoundOrphanUserDevice_DeleteBadGateway_ReturnsBadGatewayResult()
        {
            // Arrange
            var userDevice = new UserDevice { PnsToken = DevicePns };

            _mockDeviceRepositoryService.Setup(x => x.Find(DevicePns, _accessToken))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockNotificationService.Setup(x => x.Exists(userDevice))
                .ReturnsAsync(new RegistrationExistsResult.NotFound());

            _mockDeviceRepositoryService.Setup(x => x.Delete(userDevice.DeviceId, NhsLoginId))
                .ReturnsAsync(new DeleteDeviceResult.BadGateway());

            // Act
            var result = await _systemUnderTest.GetRegistration(DevicePns, _accessToken);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            result.Should().BeOfType<RegistrationExistsResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetRegistration_FindDeviceException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            _mockDeviceRepositoryService.Setup(x => x.Find(DevicePns, _accessToken))
                .ReturnsAsync(new SearchDeviceResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.GetRegistration(DevicePns, _accessToken);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();

            result.Should().BeOfType<RegistrationExistsResult.InternalServerError>();
       }

        [TestMethod]
        public async Task GetRegistration_FindDeviceBadGateway_ReturnsBadGatewayResult()
        {
            // Arrange
            _mockDeviceRepositoryService.Setup(x => x.Find(DevicePns, _accessToken))
                .ReturnsAsync(new SearchDeviceResult.BadGateway());

            // Act
            var result = await _systemUnderTest.GetRegistration(DevicePns, _accessToken);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();

            result.Should().BeOfType<RegistrationExistsResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetRegistration_RegistrationExistsException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            var userDevice = new UserDevice { PnsToken = DevicePns };

            _mockDeviceRepositoryService.Setup(x => x.Find(DevicePns, _accessToken))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockNotificationService.Setup(x => x.Exists(userDevice))
                .ReturnsAsync(new RegistrationExistsResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.GetRegistration(DevicePns, _accessToken);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            result.Should().BeOfType<RegistrationExistsResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetRegistration_RegistrationExistsBadGateway_ReturnsBadGatewayResult()
        {
            // Arrange
            var userDevice = new UserDevice { PnsToken = DevicePns };

            _mockDeviceRepositoryService.Setup(x => x.Find(DevicePns, _accessToken))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockNotificationService.Setup(x => x.Exists(userDevice))
                .ReturnsAsync(new RegistrationExistsResult.BadGateway());

            // Act
            var result = await _systemUnderTest.GetRegistration(DevicePns, _accessToken);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            result.Should().BeOfType<RegistrationExistsResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetRegistration_DeviceDeletionException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            _mockDeviceRepositoryService.Setup(x => x.Find(DevicePns, _accessToken))
                .Throws(new ArgumentException("Test"));

            // Act
            Func<Task> act = async () => await _systemUnderTest.GetRegistration(DevicePns, _accessToken);

            //Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Test");

            _mockDeviceRepositoryService.VerifyAll();
        }
    }
}