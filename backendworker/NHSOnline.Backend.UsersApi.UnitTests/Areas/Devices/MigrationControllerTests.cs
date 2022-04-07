using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Users.Areas.Devices.Models;
using NHSOnline.Backend.Users.Notifications.Migration;
using NHSOnline.Backend.UsersApi.Areas.Devices;

namespace NHSOnline.Backend.UsersApi.UnitTests.Areas.Devices
{
    [TestClass]
    public sealed class MigrationControllerTests : IDisposable
    {
        private const string ErrorMessage = "ErrorMessage";
        private const string NewInstallationId = "NewInstallationId";

        private MigrationController _systemUnderTest;

        private Mock<IMigrationService> _mockMigrationService;

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
            _mockMigrationService = new Mock<IMigrationService>(MockBehavior.Strict);

            _systemUnderTest = new MigrationController(
                new Mock<ILogger<MigrationController>>().Object,
                _mockMigrationService.Object
            );
        }

        [TestMethod]
        public async Task Migrate_MigrationServiceThrowsError_ReturnsInternalServerError()
        {
            _mockMigrationService
                .Setup(x => x.Migrate(_migrationRequest))
                .Throws<Exception>();

            var result = await _systemUnderTest.Migrate(_migrationRequest) as StatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, result.StatusCode);

            VerifySetups();
        }

        [TestMethod]
        public async Task Migrate_MigrationServiceReturnsStarted_ReturnsOK()
        {
            _mockMigrationService
                .Setup(x => x.Migrate(_migrationRequest))
                .ReturnsAsync(new MigrationResult.Success(NewInstallationId));

            var result = await _systemUnderTest.Migrate(_migrationRequest) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

            var responseBody = result.Value as MigrationResponse;

            Assert.IsNotNull(responseBody);
            Assert.IsNull(responseBody.ErrorMessage);
            Assert.AreEqual(NewInstallationId, responseBody.InstallationId);

            VerifySetups();
        }

        [TestMethod]
        public async Task Migrate_MigrationServiceReturnsBadGateway_ReturnsBadGateway()
        {
            _mockMigrationService
                .Setup(x => x.Migrate(_migrationRequest))
                .ReturnsAsync(new MigrationResult.BadGateway());

            var result = await _systemUnderTest.Migrate(_migrationRequest) as StatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status502BadGateway, result.StatusCode);

            VerifySetups();
        }

        [TestMethod]
        public async Task Migrate_MigrationServiceReturnsBadRequest_ReturnsBadRequest()
        {
            _mockMigrationService
                .Setup(x => x.Migrate(_migrationRequest))
                .ReturnsAsync(new MigrationResult.BadRequest(ErrorMessage));

            var result = await _systemUnderTest.Migrate(_migrationRequest) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);

            var responseBody = result.Value as MigrationResponse;

            Assert.IsNotNull(responseBody);
            Assert.AreEqual(ErrorMessage, responseBody.ErrorMessage);
            Assert.IsNull(responseBody.InstallationId);

            VerifySetups();
        }

        [TestMethod]
        public async Task Migrate_MigrationServiceReturnsInternalServerError_ReturnsInternalServerError()
        {
            _mockMigrationService
                .Setup(x => x.Migrate(_migrationRequest))
                .ReturnsAsync(new MigrationResult.InternalServerError());

            var result = await _systemUnderTest.Migrate(_migrationRequest) as StatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, result.StatusCode);

            VerifySetups();
        }

        private void VerifySetups()
        {
            _mockMigrationService.VerifyAll();
        }

        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}
