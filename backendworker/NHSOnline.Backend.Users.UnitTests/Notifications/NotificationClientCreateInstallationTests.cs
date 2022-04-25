using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Users.Notifications;
using NHSOnline.Backend.Users.Notifications.Models;

namespace NHSOnline.Backend.Users.UnitTests.Notifications
{
    [TestClass]
    public class NotificationClientCreateInstallationTests
    {
        private Mock<ILogger<NotificationClient>> _mockLogger;
        private Mock<IAzureNotificationHubWrapperService> _mockWrapperService;
        private Mock<IAzureNotificationHubWrapper> _mockWrapper;

        private INotificationClient _systemUnderTest;

        private const string InstallationId = "InstallationId";
        private const string NhsLoginId = "NhsLoginId";

        private readonly InstallationRequest _request = new InstallationRequest
        {
            NhsLoginId = NhsLoginId
        };

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger<NotificationClient>>();
            _mockWrapper = new Mock<IAzureNotificationHubWrapper>(MockBehavior.Strict);
            _mockWrapperService = new Mock<IAzureNotificationHubWrapperService>(MockBehavior.Strict);

            _systemUnderTest = new NotificationClient(
                _mockLogger.Object,
                _mockWrapperService.Object
            );
        }

        [TestMethod]
        public void CreateInstallation_WrapperServiceThrowsError_ThrowsError()
        {
            _mockWrapperService
                .Setup(x => x.CurrentFor(NhsLoginId))
                .Throws<ArgumentOutOfRangeException>();

            Func<Task> task = async () => await _systemUnderTest.CreateInstallation(_request);

            task.Should().ThrowAsync<ArgumentOutOfRangeException>();

            VerifySetups();
        }

        [TestMethod]
        public async Task CreateInstallation_WrapperCompletesSuccessfully_CompletesSuccessfully()
        {
            _mockWrapper
                .Setup(x => x.CreateInstallation(_request))
                .ReturnsAsync(InstallationId);

            _mockWrapperService
                .Setup(x => x.CurrentFor(NhsLoginId))
                .Returns(_mockWrapper.Object);

            var result = await _systemUnderTest.CreateInstallation(_request);

            VerifySetups();
            Assert.AreEqual(InstallationId, result);
        }

        private void VerifySetups()
        {
            _mockWrapperService.VerifyAll();
            _mockWrapper.VerifyAll();
        }
    }
}