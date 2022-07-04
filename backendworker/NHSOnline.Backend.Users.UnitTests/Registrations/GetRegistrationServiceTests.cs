using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Users.Notifications;
using NHSOnline.Backend.Users.Registrations;
using NHSOnline.Backend.Users.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.Users.UnitTests.Registrations
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
            _mockNotificationService = new Mock<INotificationRegistrationService>(MockBehavior.Strict);
            _mockDeviceRepositoryService = new Mock<IDeviceRepositoryService>(MockBehavior.Strict);

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

            // Act
            var result = await _systemUnderTest.GetRegistration(DevicePns, _accessToken);

            // Assert
            VerifyMocks();

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
            VerifyMocks();

            result.Should().BeOfType<RegistrationExistsResult.NotFound>();
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
            VerifyMocks();

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
            VerifyMocks();

            result.Should().BeOfType<RegistrationExistsResult.BadGateway>();
        }

        private void VerifyMocks()
        {
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();
        }
    }
}