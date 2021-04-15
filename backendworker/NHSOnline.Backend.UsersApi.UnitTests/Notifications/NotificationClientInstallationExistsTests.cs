using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications
{
    [TestClass]
    public class NotificationClientInstallationExistsTests
    {
        private Mock<ILogger<NotificationClient>> _mockLogger;
        private Mock<IAzureNotificationHubWrapperService> _mockWrapperService;
        private Mock<IAzureNotificationHubWrapper> _mockWrapper;
        private Mock<IAzureNotificationHubWrapper> _mockWrapper2;

        private INotificationClient _systemUnderTest;

        private const string InstallationId = "InstallationId";
        private const string NhsLoginId = "NhsLoginId";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger<NotificationClient>>();
            _mockWrapper = new Mock<IAzureNotificationHubWrapper>(MockBehavior.Strict);
            _mockWrapper2 = new Mock<IAzureNotificationHubWrapper>(MockBehavior.Strict);
            _mockWrapperService = new Mock<IAzureNotificationHubWrapperService>(MockBehavior.Strict);

            _systemUnderTest = new NotificationClient(
                _mockLogger.Object,
                _mockWrapperService.Object
            );
        }
        
        [TestMethod]
        public async Task InstallationExists_NoMatchingWrappers_ReturnsFalse()
        {
            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(Enumerable.Empty<IAzureNotificationHubWrapper>());

            var result = await _systemUnderTest.InstallationExists(InstallationId, NhsLoginId);

            VerifySetups();
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task InstallationExists_OneMatchingWrapper_InstallationNotFound_ReturnsFalse()
        {
            _mockWrapper
                .Setup(x => x.InstallationExists(InstallationId))
                .ReturnsAsync(false);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object });

            var result = await _systemUnderTest.InstallationExists(InstallationId, NhsLoginId);

            VerifySetups();
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task InstallationExists_OneMatchingWrapper_InstallationFound_ReturnsTrue()
        {
            _mockWrapper
                .Setup(x => x.InstallationExists(InstallationId))
                .ReturnsAsync(true);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object });

            var result = await _systemUnderTest.InstallationExists(InstallationId, NhsLoginId);

            VerifySetups();
            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task InstallationExists_MultipleMatchingWrappers_InstallationNotFoundInAny_ReturnsFalse()
        {
            _mockWrapper
                .Setup(x => x.InstallationExists(InstallationId))
                .ReturnsAsync(false);

            _mockWrapper2
                .Setup(x => x.InstallationExists(InstallationId))
                .ReturnsAsync(false);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            var result = await _systemUnderTest.InstallationExists(InstallationId, NhsLoginId);

            VerifySetups();
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task InstallationExists_MultipleMatchingWrappers_InstallationFoundInFirstOne_ReturnsTrue()
        {
            _mockWrapper
                .Setup(x => x.InstallationExists(InstallationId))
                .ReturnsAsync(true);

            _mockWrapper2
                .Setup(x => x.InstallationExists(InstallationId))
                .ReturnsAsync(false);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            var result = await _systemUnderTest.InstallationExists(InstallationId, NhsLoginId);

            VerifySetups(true);
            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task InstallationExists_MultipleMatchingWrappers_InstallationFoundInSecondOne_ReturnsTrue()
        {
            _mockWrapper
                .Setup(x => x.InstallationExists(InstallationId))
                .ReturnsAsync(false);

            _mockWrapper2
                .Setup(x => x.InstallationExists(InstallationId))
                .ReturnsAsync(true);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            var result = await _systemUnderTest.InstallationExists(InstallationId, NhsLoginId);

            VerifySetups();
            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task InstallationExists_MultipleMatchingWrappers_InstallationFoundInAll_ReturnsTrue()
        {
            _mockWrapper
                .Setup(x => x.InstallationExists(InstallationId))
                .ReturnsAsync(true);

            _mockWrapper2
                .Setup(x => x.InstallationExists(InstallationId))
                .ReturnsAsync(true);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            var result = await _systemUnderTest.InstallationExists(InstallationId, NhsLoginId);

            VerifySetups(true);
            result.Should().BeTrue();
        }

        private void VerifySetups(bool mockWrapper2ShouldNotBeCalled = false)
        {
            _mockWrapperService.VerifyAll();
            _mockWrapper.VerifyAll();

            if (mockWrapper2ShouldNotBeCalled)
            {
                _mockWrapper2.VerifyNoOtherCalls();
            }
            else
            {
                _mockWrapper2.VerifyAll();
            }
        }
    }
}