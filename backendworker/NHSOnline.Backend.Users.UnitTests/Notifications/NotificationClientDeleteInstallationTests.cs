using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Users.Notifications;

namespace NHSOnline.Backend.Users.UnitTests.Notifications
{
    [TestClass]
    public class NotificationClientDeleteInstallationTests
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
        public async Task DeleteInstallation_NoMatchingWrappers_NoActionTaken()
        {
            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(Enumerable.Empty<IAzureNotificationHubWrapper>());

            await _systemUnderTest.DeleteInstallation(InstallationId, NhsLoginId);

            VerifySetups();
        }

        [TestMethod]
        public async Task DeleteInstallation_OneMatchingWrapper_InstallationNotFound_NoActionTaken()
        {
            SetupMockWrapper(_mockWrapper, false);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object });

            await _systemUnderTest.DeleteInstallation(InstallationId, NhsLoginId);

            VerifySetups();
        }

        [TestMethod]
        public async Task DeleteInstallation_OneMatchingWrapper_InstallationFound_CompletesSuccessfully()
        {
            SetupMockWrapper(_mockWrapper, true);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object });

            await _systemUnderTest.DeleteInstallation(InstallationId, NhsLoginId);

            VerifySetups();
        }

        [TestMethod]
        public async Task DeleteInstallation_MultipleMatchingWrappers_InstallationNotFoundInAny_NoActionTaken()
        {
            SetupMockWrapper(_mockWrapper, false);
            SetupMockWrapper(_mockWrapper2, false);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            await _systemUnderTest.DeleteInstallation(InstallationId, NhsLoginId);

            VerifySetups();
        }

        [TestMethod]
        public async Task DeleteInstallation_MultipleMatchingWrappers_InstallationFoundInOne_CompletesSuccessfully()
        {
            SetupMockWrapper(_mockWrapper, true);
            SetupMockWrapper(_mockWrapper2, false);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            await _systemUnderTest.DeleteInstallation(InstallationId, NhsLoginId);

            VerifySetups();
        }

        [TestMethod]
        public async Task DeleteInstallation_MultipleMatchingWrappers_InstallationFoundInAll_CompletesSuccessfully()
        {
            SetupMockWrapper(_mockWrapper, true);
            SetupMockWrapper(_mockWrapper2, true);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            await _systemUnderTest.DeleteInstallation(InstallationId, NhsLoginId);

            VerifySetups();
        }

        private static void SetupMockWrapper(Mock<IAzureNotificationHubWrapper> wrapper, bool installationExists)
        {
            wrapper
                .Setup(x => x.InstallationExists(InstallationId))
                .ReturnsAsync(installationExists);

            if (installationExists)
            {
                wrapper
                    .Setup(x => x.DeleteInstallation(InstallationId))
                    .Returns(Task.CompletedTask);
            }
        }

        private void VerifySetups()
        {
            _mockWrapperService.VerifyAll();
            _mockWrapper.VerifyAll();
            _mockWrapper2.VerifyAll();
        }
    }
}
