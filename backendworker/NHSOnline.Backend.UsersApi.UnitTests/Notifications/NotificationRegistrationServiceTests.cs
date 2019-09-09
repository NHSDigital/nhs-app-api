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
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;
using UnitTestHelper;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications
{
    [TestClass]
    public class NotificationRegistrationServiceTests
    {
        private Fixture _fixture;
        private NotificationRegistrationService _systemUnderTests;
        private Mock<INotificationService> _mockNotificationsService;
        private AccessToken _accessToken;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());

            _mockNotificationsService = _fixture.Freeze<Mock<INotificationService>>();
            
            var mockLogger = _fixture.Freeze<Mock<ILogger<NotificationRegistrationService>>>();
            var accessTokenString = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _fixture.Create<string>()),
                new Claim("nhs_number", _fixture.Create<string>()),
            });

            _accessToken = AccessToken.Parse(mockLogger.Object, accessTokenString);

            _systemUnderTests = _fixture.Create<NotificationRegistrationService>();
        }

        [TestMethod]
        public async Task Register_WhenRegistrationSuccessful_ReturnsSuccessResult()
        {
            // Arrange
            var request = _fixture.Create<RegisterDeviceRequest>();
            var expectedResponse = new RegistrationResult.Success(_fixture.Create<NotificationRegistrationResult>());
            
            _mockNotificationsService
                .Setup(x => x.Register(It.IsAny<NotificationRegistrationRequest>()))
                .ReturnsAsync(expectedResponse)
                .Verifiable();

            // Act
            var result = await _systemUnderTests.Register(request, _accessToken);

            // Assert
            _mockNotificationsService.Verify();
            result.Should().Be(expectedResponse);
        }
        
        [TestMethod]
        public async Task Register_WhenRegistrationReturnsBadGateway_ReturnsBadGatewayResult()
        {
            // Arrange
            var request = _fixture.Create<RegisterDeviceRequest>();
            var expectedResponse = new RegistrationResult.BadGateway();
            
            _mockNotificationsService
                .Setup(x => x.Register(It.IsAny<NotificationRegistrationRequest>()))
                .ReturnsAsync(expectedResponse)
                .Verifiable();

            // Act
            var result = await _systemUnderTests.Register(request, _accessToken);

            // Assert
            _mockNotificationsService.Verify();
            result.Should().Be(expectedResponse);
        }
    }
}