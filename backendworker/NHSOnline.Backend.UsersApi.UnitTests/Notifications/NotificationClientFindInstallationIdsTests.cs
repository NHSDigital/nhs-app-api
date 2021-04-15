using System;
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
    public class NotificationClientFindInstallationIdsTests
    {
        private Mock<ILogger<NotificationClient>> _mockLogger;
        private Mock<IAzureNotificationHubWrapperService> _mockWrapperService;
        private Mock<IAzureNotificationHubWrapper> _mockWrapper;
        private Mock<IAzureNotificationHubWrapper> _mockWrapper2;

        private INotificationClient _systemUnderTest;

        private const string InstallationId = "InstallationId";
        private const string NhsLoginId = "NhsLoginId";

        private readonly string[] _installationIds = { InstallationId };

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
        public async Task FindInstallationIds_NoMatchingWrappers_ReturnsFalse()
        {
            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(Enumerable.Empty<IAzureNotificationHubWrapper>());

            var result = await _systemUnderTest.FindInstallationIdsByNhsLoginId(NhsLoginId);

            VerifySetups();
            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task FindInstallationIds_OneMatchingWrapper_InstallationNotFound_ReturnsFalse()
        {
            _mockWrapper
                .Setup(x => x.GetInstallationIdsByNhsLoginId(NhsLoginId))
                .ReturnsAsync(Array.Empty<string>());

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object });

            var result = await _systemUnderTest.FindInstallationIdsByNhsLoginId(NhsLoginId);

            VerifySetups();
            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task FindInstallationIds_OneMatchingWrapper_InstallationFound_ReturnsTrue()
        {
            _mockWrapper
                .Setup(x => x.GetInstallationIdsByNhsLoginId(NhsLoginId))
                .ReturnsAsync(_installationIds);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object });

            var result = await _systemUnderTest.FindInstallationIdsByNhsLoginId(NhsLoginId);

            VerifySetups();
            result.Should().BeEquivalentTo(_installationIds);
        }

        [TestMethod]
        public async Task FindInstallationIds_MultipleMatchingWrappers_InstallationNotFoundInAny_ReturnsFalse()
        {
            _mockWrapper
                .Setup(x => x.GetInstallationIdsByNhsLoginId(NhsLoginId))
                .ReturnsAsync(Array.Empty<string>());

            _mockWrapper2
                .Setup(x => x.GetInstallationIdsByNhsLoginId(NhsLoginId))
                .ReturnsAsync(Array.Empty<string>());

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            var result = await _systemUnderTest.FindInstallationIdsByNhsLoginId(NhsLoginId);

            VerifySetups();
            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task FindInstallationIds_MultipleMatchingWrappers_InstallationFoundInFirstOne_ReturnsTrue()
        {
            _mockWrapper
                .Setup(x => x.GetInstallationIdsByNhsLoginId(NhsLoginId))
                .ReturnsAsync(_installationIds);

            _mockWrapper2
                .Setup(x => x.GetInstallationIdsByNhsLoginId(NhsLoginId))
                .ReturnsAsync(Array.Empty<string>());

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            var result = await _systemUnderTest.FindInstallationIdsByNhsLoginId(NhsLoginId);

            VerifySetups();
            result.Should().BeEquivalentTo(_installationIds);
        }

        [TestMethod]
        public async Task FindInstallationIds_MultipleMatchingWrappers_InstallationFoundInSecondOne_ReturnsTrue()
        {
            _mockWrapper
                .Setup(x => x.GetInstallationIdsByNhsLoginId(NhsLoginId))
                .ReturnsAsync(Array.Empty<string>());

            _mockWrapper2
                .Setup(x => x.GetInstallationIdsByNhsLoginId(NhsLoginId))
                .ReturnsAsync(_installationIds);

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            var result = await _systemUnderTest.FindInstallationIdsByNhsLoginId(NhsLoginId);

            VerifySetups();
            result.Should().BeEquivalentTo(_installationIds);
        }

        [TestMethod]
        public async Task FindInstallationIds_MultipleMatchingWrappers_InstallationFoundInAll_ReturnsTrue()
        {
            const string installationId1 = "installationId1";
            const string installationId2 = "installationId2";

            _mockWrapper
                .Setup(x => x.GetInstallationIdsByNhsLoginId(NhsLoginId))
                .ReturnsAsync(new []{ installationId1 });

            _mockWrapper2
                .Setup(x => x.GetInstallationIdsByNhsLoginId(NhsLoginId))
                .ReturnsAsync(new[] { installationId2 });

            _mockWrapperService
                .Setup(x => x.AllFor(NhsLoginId))
                .Returns(new[] { _mockWrapper.Object, _mockWrapper2.Object });

            var result = await _systemUnderTest.FindInstallationIdsByNhsLoginId(NhsLoginId);

            VerifySetups();
            result.Should().BeEquivalentTo(installationId1, installationId2);
        }

        private void VerifySetups()
        {
            _mockWrapperService.VerifyAll();
            _mockWrapper.VerifyAll();
            _mockWrapper2.VerifyAll();
        }
    }
}