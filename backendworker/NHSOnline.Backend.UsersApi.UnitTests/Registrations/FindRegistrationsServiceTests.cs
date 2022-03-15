using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Registrations;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.UnitTests.Registrations
{
    [TestClass]
    public sealed class FindRegistrationsServiceTests
    {
        private RegistrationService _systemUnderTest;
        private Mock<IDeviceRepositoryService> _mockDeviceRepositoryService;
        private const string NhsLoginId = "NhsLoginId";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockDeviceRepositoryService = new Mock<IDeviceRepositoryService>(MockBehavior.Strict);

            _systemUnderTest = new RegistrationService(
                _mockDeviceRepositoryService.Object,
                null,
                new Mock<ILogger<RegistrationService>>().Object);
        }

        [TestMethod]
        public async Task Find_WhenRegistrationsExists_ReturnsFoundResult()
        {
            // Arrange
            var registrationIds = new[] { "Registration1", "Registration2" };
            _mockDeviceRepositoryService.Setup(x => x.Find(NhsLoginId))
                .ReturnsAsync(new FindRegistrationsResult.Found(registrationIds));

            // Act
            var result = await _systemUnderTest.Find(NhsLoginId);

            // Assert
            VerifyMocks();

            var subject = result.Should().BeOfType<FindRegistrationsResult.Found>().Subject;
            subject.RegistrationIds.Should().BeEquivalentTo(registrationIds);
        }

        [TestMethod]
        public async Task Find_WhenNoRegistrations_ReturnsNotFoundResult()
        {
            // Arrange
            _mockDeviceRepositoryService.Setup(x => x.Find(NhsLoginId))
                .ReturnsAsync(new FindRegistrationsResult.NotFound());

            // Act
            var result = await _systemUnderTest.Find(NhsLoginId);

            // Assert
            VerifyMocks();

            result.Should().BeOfType<FindRegistrationsResult.NotFound>();
        }

        [TestMethod]
        public async Task Find_Exception_ReturnsBadGatewayResult()
        {
            // Arrange
            _mockDeviceRepositoryService
                .Setup(x => x.Find(NhsLoginId))
                .ReturnsAsync(new FindRegistrationsResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Find(NhsLoginId);

            // Assert
            VerifyMocks();

            result.Should().BeOfType<FindRegistrationsResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetRegistration_FindDeviceException_ReturnsInternalServerErrorResult()
        {
            // Arrange
            _mockDeviceRepositoryService
                .Setup(x => x.Find(NhsLoginId))
                .ReturnsAsync(new FindRegistrationsResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Find(NhsLoginId);

            // Assert
            VerifyMocks();

            result.Should().BeOfType<FindRegistrationsResult.InternalServerError>();
        }

        private void VerifyMocks()
        {
            _mockDeviceRepositoryService.VerifyAll();
        }
    }
}