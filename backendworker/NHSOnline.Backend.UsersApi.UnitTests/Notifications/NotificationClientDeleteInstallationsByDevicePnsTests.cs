using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications
{
    [TestClass]
    public class NotificationClientDeleteInstallationByDevicePnsTests
    {
        private Mock<ILogger<NotificationClient>> _mockLogger;
        private Mock<IAzureNotificationHubWrapperService> _mockWrapperService;
        private Mock<IAzureNotificationHubWrapper> _mockWrapper;
        private Mock<IAzureNotificationHubWrapper> _mockWrapper2;

        private INotificationClient _systemUnderTest;

        private const string DevicePns = "DevicePns";
        private const string InstallationId = "InstallationId";

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
        public async Task DeleteInstallations_NoMatchingWrappers_NoActionTaken()
        {
            _mockWrapperService
                .Setup(x => x.All())
                .Returns(Enumerable.Empty<IAzureNotificationHubWrapper>());

            await _systemUnderTest.DeleteInstallationsByDevicePns(DevicePns);

            VerifySetups();
        }

        [TestMethod]
        public async Task DeleteInstallations_OneMatchingWrapper_InstallationNotFound_NoActionTaken()
        {
            SetupMockWrapper(_mockWrapper, false);

            _mockWrapperService
                .Setup(x => x.All())
                .Returns(new[] { _mockWrapper.Object });

            await _systemUnderTest.DeleteInstallationsByDevicePns(DevicePns);

            VerifySetups();
        }

        [TestMethod]
        public async Task DeleteInstallations_OneMatchingWrapper_InstallationFound_CompletesSuccessfully()
        {
            SetupMockWrapper(_mockWrapper, true);

            _mockWrapperService
                .Setup(x => x.All())
                .Returns(new[] { _mockWrapper.Object });

            await _systemUnderTest.DeleteInstallationsByDevicePns(DevicePns);

            VerifySetups();
        }

        [TestMethod]
        public async Task DeleteInstallations_MultipleMatchingWrappers_InstallationNotFoundInAny_NoActionTaken()
        {
            SetupMockWrapper(_mockWrapper, false);
            SetupMockWrapper(_mockWrapper2, false);

            _mockWrapperService
                .Setup(x => x.All())
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            await _systemUnderTest.DeleteInstallationsByDevicePns(DevicePns);

            VerifySetups();
        }

        [TestMethod]
        public async Task DeleteInstallations_MultipleMatchingWrappers_InstallationFoundInOne_CompletesSuccessFully()
        {
            SetupMockWrapper(_mockWrapper, true);
            SetupMockWrapper(_mockWrapper2, false);

            _mockWrapperService
                .Setup(x => x.All())
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            await _systemUnderTest.DeleteInstallationsByDevicePns(DevicePns);

            VerifySetups();
        }

        [TestMethod]
        public async Task DeleteInstallations_MultipleMatchingWrappers_InstallationFoundInAll_CompletesSuccessFully()
        {
            SetupMockWrapper(_mockWrapper, true);
            SetupMockWrapper(_mockWrapper2, true);

            _mockWrapperService
                .Setup(x => x.All())
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            await _systemUnderTest.DeleteInstallationsByDevicePns(DevicePns);

            VerifySetups();
        }

        private static void SetupMockWrapper(Mock<IAzureNotificationHubWrapper> wrapper, bool installationsExist)
        {
            var installations = installationsExist
                ? new[] { InstallationId }
                : Array.Empty<string>();

            wrapper
                .Setup(x => x.GetInstallationIdsByDevicePns(DevicePns))
                .ReturnsAsync(installations);

            if (installationsExist)
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