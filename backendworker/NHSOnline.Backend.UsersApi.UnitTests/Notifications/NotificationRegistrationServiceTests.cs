using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications
{
    [TestClass]
    public class NotificationRegistrationServiceTests
    {
        private Fixture _fixture;
        private NotificationRegistrationService _systemUnderTests;
        private Mock<INotificationService> _mockNotificationsService;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());

            _mockNotificationsService = _fixture.Freeze<Mock<INotificationService>>();

            _systemUnderTests = _fixture.Create<NotificationRegistrationService>();
        }

        [TestMethod]
        public async Task Register_WhenRegistrationSuccessful_ReturnsSuccessResult()
        {
            //Arrange
            var request = _fixture.Create<RegisterDeviceRequest>();
            var expectedResponse = new RegistrationResult.Success(_fixture.Create<NotificationRegistrationResult>());
            
            _mockNotificationsService
                .Setup(x => x.Register(It.IsAny<NotificationRegistrationRequest>()))
                .ReturnsAsync(expectedResponse)
                .Verifiable();

            //Act
            var result = await _systemUnderTests.Register(request, _fixture.Create<string>());

            //Assert
            _mockNotificationsService.Verify();
            result.Should().Be(expectedResponse);
        }
        
        [TestMethod]
        public async Task Register_WhenRegistrationReturnsBadGateway_ReturnsBadGatewayResult()
        {
            //Arrange
            var request = _fixture.Create<RegisterDeviceRequest>();
            var expectedResponse = new RegistrationResult.BadGateway();
            
            _mockNotificationsService
                .Setup(x => x.Register(It.IsAny<NotificationRegistrationRequest>()))
                .ReturnsAsync(expectedResponse)
                .Verifiable();

            //Act
            var result = await _systemUnderTests.Register(request, _fixture.Create<string>());

            //Assert
            _mockNotificationsService.Verify();
            result.Should().Be(expectedResponse);
        }
    }
}