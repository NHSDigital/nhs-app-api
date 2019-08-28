using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
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
    public class DeviceRepositoryServiceTests
    {
        private IFixture _fixture;
        private DeviceRepositoryService _systemUnderTest;
        private Mock<IUserDeviceRepository> _deviceRepository;
        private Mock<IDeviceIdGenerator> _deviceIdGenerator;
        private AccessToken _accessToken;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _deviceRepository = _fixture.Freeze<Mock<IUserDeviceRepository>>();
            _deviceIdGenerator = _fixture.Freeze<Mock<IDeviceIdGenerator>>();
            var mockLogger = _fixture.Freeze<Mock<ILogger<DeviceRepositoryService>>>();
            var accessTokenString = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _fixture.Create<string>()),
                new Claim("nhs_number", _fixture.Create<string>()),
            });

            _accessToken = AccessToken.Parse(mockLogger.Object, accessTokenString);

            _systemUnderTest = _fixture.Create<DeviceRepositoryService>();
        }

        [TestMethod]
        public async Task Create_Success()
        {
            // Arrange
            var request = _fixture.Create<RegisterDeviceRequest>();
            _deviceIdGenerator.Setup(x => x.Generate(_accessToken, request)).Returns(_fixture.Create<string>());
            _deviceRepository.Setup(x => x.Create(It.IsAny<UserDevice>())).Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Create(
                _fixture.Create<NotificationRegistrationResult>(), 
                request, 
                _accessToken);

            // Assert
            var objectResult = result.Should().BeAssignableTo<DeviceRepositoryResult.Created>();
            objectResult.Subject.UserDevice.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Create_RepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var request = _fixture.Create<RegisterDeviceRequest>();
            _deviceRepository.Setup(x => x.Create(It.IsAny<UserDevice>())).Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Create(
                _fixture.Create<NotificationRegistrationResult>(), 
                request, 
                _accessToken);

                // Assert
            result.Should().BeAssignableTo<DeviceRepositoryResult.Failure>();
        }
    }
}
