using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Users.Areas.Devices.Models;
using NHSOnline.Backend.Users.Notifications;
using NHSOnline.Backend.Users.Notifications.Migration;

namespace NHSOnline.Backend.Users.UnitTests.Notifications.Migration
{
    [TestClass]
    public class MigrationServiceTests
    {
        private const string Message = "Message";

        private IMigrationService _systemUnderTest;

        private Mock<ILogger<MigrationService>> _mockLogger;
        private Mock<INotificationClient> _mockNotificationClient;

        private readonly MigrationRequest _migrationRequest = new MigrationRequest
        {
            DevicePns = "DevicePns",
            DeviceType = DeviceType.Android,
            InstallationId = "InstallationId",
            NhsLoginId = "NhsLoginId",
            SourcePath = "SourcePath",
            TargetPath = "TargetPath"
        };

        [TestInitialize]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<MigrationService>>(MockBehavior.Loose);

            _mockNotificationClient = new Mock<INotificationClient>(MockBehavior.Strict);

            _systemUnderTest = new MigrationService(
                _mockLogger.Object,
                _mockNotificationClient.Object
            );
        }

        [TestMethod]
        public async Task Migrate_NotificationClientThrowsException_ReturnsInternalServerError()
        {
            _mockNotificationClient
                .Setup(x => x.Migrate(_migrationRequest))
                .Throws<Exception>();

            var result = await _systemUnderTest.Migrate(_migrationRequest);

            Assert.IsInstanceOfType(result, typeof(MigrationResult.InternalServerError));

            VerifySetups();
        }

        [TestMethod]
        public async Task Migrate_NotificationClientReturnsBadGateway_ReturnsBadGateway()
        {
            _mockNotificationClient
                .Setup(x => x.Migrate(_migrationRequest))
                .ReturnsAsync((HttpStatusCode.BadGateway, Message));

            var result = await _systemUnderTest.Migrate(_migrationRequest) as MigrationResult.BadGateway;

            Assert.IsNotNull(result);

            VerifySetups();
        }

        [TestMethod]
        public async Task Migrate_NotificationClientReturnsBadRequest_ReturnsBadRequest()
        {
            _mockNotificationClient
                .Setup(x => x.Migrate(_migrationRequest))
                .ReturnsAsync((HttpStatusCode.BadRequest, Message));

            var result = await _systemUnderTest.Migrate(_migrationRequest) as MigrationResult.BadRequest;

            Assert.IsNotNull(result);
            Assert.AreEqual(Message, result.ErrorMessage);

            VerifySetups();
        }

        [TestMethod]
        public async Task Migrate_NotificationClientReturnsSuccess_ReturnsSuccess()
        {
            _mockNotificationClient
                .Setup(x => x.Migrate(_migrationRequest))
                .ReturnsAsync((HttpStatusCode.OK, Message));

            var result = await _systemUnderTest.Migrate(_migrationRequest) as MigrationResult.Success;

            Assert.IsNotNull(result);
            Assert.AreEqual(Message, result.InstallationId);

            VerifySetups();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.InternalServerError)]
        [DataRow(HttpStatusCode.Unauthorized)]
        public async Task Migrate_NotificationClientReturnsUnexpectedValue_ReturnsInternalServerError(HttpStatusCode statusCode)
        {
            _mockNotificationClient
                .Setup(x => x.Migrate(_migrationRequest))
                .ReturnsAsync((statusCode, Message));

            var result = await _systemUnderTest.Migrate(_migrationRequest) as MigrationResult.InternalServerError;

            Assert.IsNotNull(result);

            VerifySetups();
        }

        [TestMethod]
        private void VerifySetups()
        {
            _mockNotificationClient.VerifyAll();
        }
    }
}
