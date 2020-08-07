using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.UsersApi.Areas.Devices;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.UnitTests.Areas.Devices
{
    [TestClass]
    public sealed class MigrationControllerTests : IDisposable
    {
        private MigrationController _systemUnderTest;
        private Mock<IMigrationService> _mockMigrationService;
        private Mock<IDeviceRepositoryService> _mockDeviceRepositoryService;
        private Mock<INotificationRegistrationService> _mockNotificationRegistrationService;

        private const string IosPnsToken = "5FF059B3C2D177A970764D9A727D72F8856BC5B507B1AA2D38D3D556D1FC7772";
        private const string AndroidPnsToken =
            "fnxIbNLPSGw:APA91bEGNUyVL-1KU8Rek6_xYyyLZYsVVrpzBq3xhxithhxPbh3cA4RnVQ4zpVDzbj0gVZ2mSG6duevMhDG96aUnd89Ov8kw39qcMX-ztTno0vQYl_p9nkLjFSOc0605-ok3nAW5HyRO";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockMigrationService = new Mock<IMigrationService>();
            _mockDeviceRepositoryService = new Mock<IDeviceRepositoryService>();
            _mockNotificationRegistrationService = new Mock<INotificationRegistrationService>();

            _systemUnderTest = new MigrationController(
                new Mock<ILogger<MigrationController>>().Object,
                _mockMigrationService.Object,
                _mockNotificationRegistrationService.Object,
                _mockDeviceRepositoryService.Object);
        }

        [TestMethod]
        public async Task MigrateRegistration_WhenNoUserDevicesLeft_ReturnsNotFound()
        {
            // Arrange
            _mockDeviceRepositoryService.Setup(x => x.FindRegistrations(It.IsAny<int>()))
                .ReturnsAsync(new SearchDeviceResult.NotFound());

            // Act
            var result = await _systemUnderTest.MigrateRegistration(50);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            objectResult.Value.Should().BeEquivalentTo(new MigrationController.MigrationResult());
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task MigrateRegistration_WhenInvalidMigrationCount_ReturnsBadRequest(int migrationCount)
        {
            // Act
            var result = await _systemUnderTest.MigrateRegistration(migrationCount);

            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task MigrateRegistration_WhenThereAreUserDevicesToMigrate_ReturnsOk()
        {
            // Arrange
            var expectedResult = new MigrationController.MigrationResult
            {
                TotalCount = 3,
                SuccessCount = 3
            };

            var userDevices = new[]
            {
                new UserDevice { DeviceId = "firstDeviceId", RegistrationId = "firstOriginalRegistrationId", PnsToken = AndroidPnsToken, NhsLoginId = "firstNhsLoginId" },
                new UserDevice { DeviceId = "secondDeviceId", RegistrationId = "secondOriginalRegistrationId", PnsToken = IosPnsToken, NhsLoginId = "secondNhsLoginId" },
                new UserDevice { DeviceId = "thirdDeviceId", RegistrationId = "thirdOriginalRegistrationId", PnsToken = AndroidPnsToken, NhsLoginId = "thirdNhsLoginId" }
            };

            _mockDeviceRepositoryService.Setup(x => x.FindRegistrations(It.IsAny<int>()))
                .ReturnsAsync(new SearchDeviceResult.FoundMany(userDevices));

            SetupSuccessfulMockSequence(userDevices[0], DeviceType.Android, "firstRegistrationId");
            SetupSuccessfulMockSequence(userDevices[1], DeviceType.Ios, "secondRegistrationId");
            SetupSuccessfulMockSequence(userDevices[2], DeviceType.Android, "thirdRegistrationId");

            // Act
            var result = await _systemUnderTest.MigrateRegistration(50);

            // Assert,
            _mockDeviceRepositoryService.VerifyAll();
            _mockMigrationService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            objectResult.Value.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task MigrateRegistration_WhenOrphanedRegistrationFoundAndDeleted_ReturnsOkWithCorrectCounts()
        {
            // Arrange
            var expectedResult = new MigrationController.MigrationResult
            {
                TotalCount = 3,
                SuccessCount = 2,
                DeletedOrphanCount = 1
            };

            var userDevices = new[]
            {
                new UserDevice { DeviceId = "firstDeviceId", RegistrationId = "firstOriginalRegistrationId", PnsToken = AndroidPnsToken, NhsLoginId = "firstNhsLoginId" },
                new UserDevice { DeviceId = "secondDeviceId", RegistrationId = "secondOriginalRegistrationId", PnsToken = IosPnsToken, NhsLoginId = "secondNhsLoginId" },
                new UserDevice { DeviceId = "thirdDeviceId", RegistrationId = "thirdOriginalRegistrationId", PnsToken = AndroidPnsToken, NhsLoginId = "thirdNhsLoginId" }
            };

            _mockDeviceRepositoryService.Setup(x => x.FindRegistrations(It.IsAny<int>()))
                .ReturnsAsync(new SearchDeviceResult.FoundMany(userDevices));

            SetupSuccessfulMockSequence(userDevices[0], DeviceType.Android, "firstRegistrationId");

            _mockNotificationRegistrationService
                .Setup(x => x.Exists(userDevices[1]))
                .ReturnsAsync(new RegistrationExistsResult.NotFound());

            _mockDeviceRepositoryService
                .Setup(x => x.Delete(userDevices[1].DeviceId, userDevices[1].NhsLoginId))
                .ReturnsAsync(new DeleteDeviceResult.Success(userDevices[1].DeviceId));

            SetupSuccessfulMockSequence(userDevices[2], DeviceType.Android, "thirdRegistrationId");

            // Act
            var result = await _systemUnderTest.MigrateRegistration(50);

            // Assert
            _mockMigrationService.VerifyAll();
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationRegistrationService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            objectResult.Value.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task MigrateRegistration_WhenOrphanedRegistrationFoundAndDeleteFails_ReturnsOkWithCorrectCounts()
        {
            // Arrange
            var expectedResult = new MigrationController.MigrationResult
            {
                TotalCount = 3,
                SuccessCount = 2,
                FailedDeleteOrphanCount = 1
            };

            var userDevices = new[]
            {
                new UserDevice { DeviceId = "firstDeviceId", RegistrationId = "firstOriginalRegistrationId", PnsToken = AndroidPnsToken, NhsLoginId = "firstNhsLoginId" },
                new UserDevice { DeviceId = "secondDeviceId", RegistrationId = "secondOriginalRegistrationId", PnsToken = IosPnsToken, NhsLoginId = "secondNhsLoginId" },
                new UserDevice { DeviceId = "thirdDeviceId", RegistrationId = "thirdOriginalRegistrationId", PnsToken = AndroidPnsToken, NhsLoginId = "thirdNhsLoginId" }
            };

            _mockDeviceRepositoryService.Setup(x => x.FindRegistrations(It.IsAny<int>()))
                .ReturnsAsync(new SearchDeviceResult.FoundMany(userDevices));

            SetupSuccessfulMockSequence(userDevices[0], DeviceType.Android, "firstRegistrationId");

            _mockNotificationRegistrationService
                .Setup(x => x.Exists(userDevices[1]))
                .ReturnsAsync(new RegistrationExistsResult.NotFound());

            _mockDeviceRepositoryService
                .Setup(x => x.Delete(userDevices[1].DeviceId, userDevices[1].NhsLoginId))
                .ReturnsAsync(new DeleteDeviceResult.InternalServerError());

            SetupSuccessfulMockSequence(userDevices[2], DeviceType.Android, "thirdRegistrationId");

            // Act
            var result = await _systemUnderTest.MigrateRegistration(50);

            // Assert
            _mockMigrationService.VerifyAll();
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationRegistrationService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            objectResult.Value.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task MigrateRegistration_WhenMigrationRegistrationFails_ReturnsOkWithCorrectCounts()
        {
            // Arrange
            var expectedResult = new MigrationController.MigrationResult
            {
                TotalCount = 3,
                SuccessCount = 2,
                FailedCount = 1
            };

            var userDevices = new[]
            {
                new UserDevice { DeviceId = "firstDeviceId", RegistrationId = "firstOriginalRegistrationId", PnsToken = AndroidPnsToken, NhsLoginId = "firstNhsLoginId" },
                new UserDevice { DeviceId = "secondDeviceId", RegistrationId = "secondOriginalRegistrationId", PnsToken = IosPnsToken, NhsLoginId = "secondNhsLoginId" },
                new UserDevice { DeviceId = "thirdDeviceId", RegistrationId = "thirdOriginalRegistrationId", PnsToken = AndroidPnsToken, NhsLoginId = "thirdNhsLoginId" }
            };

            _mockDeviceRepositoryService.Setup(x => x.FindRegistrations(It.IsAny<int>()))
                .ReturnsAsync(new SearchDeviceResult.FoundMany(userDevices));

            SetupSuccessfulMockSequence(userDevices[0], DeviceType.Android, "firstRegistrationId");

            _mockNotificationRegistrationService
                .Setup(x => x.Exists(userDevices[1]))
                .ReturnsAsync(new RegistrationExistsResult.Found());

            _mockMigrationService
                .Setup(x => x.Register(userDevices[1].PnsToken, DeviceType.Ios, userDevices[1].NhsLoginId))
                .ReturnsAsync(new RegistrationResult.BadGateway());

            SetupSuccessfulMockSequence(userDevices[2], DeviceType.Android, "thirdRegistrationId");

            // Act
            var result = await _systemUnderTest.MigrateRegistration(50);

            // Assert
            _mockMigrationService.VerifyAll();
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationRegistrationService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            objectResult.Value.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task MigrateRegistration_WhenMigrationRepositoryUpdateFails_ReturnsOkWithCorrectCounts()
        {
            // Arrange
            var expectedResult = new MigrationController.MigrationResult
            {
                TotalCount = 3,
                SuccessCount = 2,
                FailedCount = 1
            };

            var userDevices = new[]
            {
                new UserDevice { DeviceId = "firstDeviceId", RegistrationId = "firstOriginalRegistrationId", PnsToken = AndroidPnsToken, NhsLoginId = "firstNhsLoginId" },
                new UserDevice { DeviceId = "secondDeviceId", RegistrationId = "secondOriginalRegistrationId", PnsToken = IosPnsToken, NhsLoginId = "secondNhsLoginId" },
                new UserDevice { DeviceId = "thirdDeviceId", RegistrationId = "thirdOriginalRegistrationId", PnsToken = AndroidPnsToken, NhsLoginId = "thirdNhsLoginId" }
            };

            _mockDeviceRepositoryService.Setup(x => x.FindRegistrations(It.IsAny<int>()))
                .ReturnsAsync(new SearchDeviceResult.FoundMany(userDevices));

            SetupSuccessfulMockSequence(userDevices[0], DeviceType.Android, "firstRegistrationId");

            _mockNotificationRegistrationService
                .Setup(x => x.Exists(userDevices[1]))
                .ReturnsAsync(new RegistrationExistsResult.Found());

            _mockMigrationService
                .Setup(x => x.Register(userDevices[1].PnsToken, DeviceType.Ios, userDevices[1].NhsLoginId))
                .ReturnsAsync(new RegistrationResult.Success(new NotificationRegistrationResult
                    { Id = "secondRegistrationId" }));

            _mockDeviceRepositoryService
                .Setup(x => x.Update(userDevices[1].DeviceId, userDevices[1].NhsLoginId, "secondRegistrationId"))
                .ReturnsAsync(new UpdateDeviceResult.InternalServerError());

            _mockMigrationService
                .Setup(x => x.Delete("secondRegistrationId"))
                .ReturnsAsync(new DeleteRegistrationResult.Success());

            SetupSuccessfulMockSequence(userDevices[2], DeviceType.Android, "thirdRegistrationId");

            // Act
            var result = await _systemUnderTest.MigrateRegistration(50);

            // Assert
            _mockMigrationService.VerifyAll();
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationRegistrationService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            objectResult.Value.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task MigrateRegistration_WhenMigrationRepositoryUpdateFailsAndDeleteNotFound_ReturnsOkWithCorrectCounts()
        {
            // Arrange
            var expectedResult = new MigrationController.MigrationResult
            {
                TotalCount = 3,
                SuccessCount = 2,
                FailedCount = 1
            };

            var userDevices = new[]
            {
                new UserDevice { DeviceId = "firstDeviceId", RegistrationId = "firstOriginalRegistrationId", PnsToken = AndroidPnsToken, NhsLoginId = "firstNhsLoginId" },
                new UserDevice { DeviceId = "secondDeviceId", RegistrationId = "secondOriginalRegistrationId", PnsToken = IosPnsToken, NhsLoginId = "secondNhsLoginId" },
                new UserDevice { DeviceId = "thirdDeviceId", RegistrationId = "thirdOriginalRegistrationId", PnsToken = AndroidPnsToken, NhsLoginId = "thirdNhsLoginId" }
            };

            _mockDeviceRepositoryService.Setup(x => x.FindRegistrations(It.IsAny<int>()))
                .ReturnsAsync(new SearchDeviceResult.FoundMany(userDevices));

            SetupSuccessfulMockSequence(userDevices[0], DeviceType.Android, "firstRegistrationId");

            _mockNotificationRegistrationService
                .Setup(x => x.Exists(userDevices[1]))
                .ReturnsAsync(new RegistrationExistsResult.Found());

            _mockMigrationService
                .Setup(x => x.Register(userDevices[1].PnsToken, DeviceType.Ios, userDevices[1].NhsLoginId))
                .ReturnsAsync(new RegistrationResult.Success(new NotificationRegistrationResult
                    { Id = "secondRegistrationId" }));

            _mockDeviceRepositoryService
                .Setup(x => x.Update(userDevices[1].DeviceId, userDevices[1].NhsLoginId, "secondRegistrationId"))
                .ReturnsAsync(new UpdateDeviceResult.InternalServerError());

            _mockMigrationService
                .Setup(x => x.Delete("secondRegistrationId"))
                .ReturnsAsync(new DeleteRegistrationResult.NotFound());

            SetupSuccessfulMockSequence(userDevices[2], DeviceType.Android, "thirdRegistrationId");

            // Act
            var result = await _systemUnderTest.MigrateRegistration(50);

            // Assert
            _mockMigrationService.VerifyAll();
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationRegistrationService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            objectResult.Value.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task MigrateRegistration_WhenRepositoryUpdateAndRegistrationDeleteFails_ReturnsOkWithCorrectCounts()
        {
            // Arrange
            var expectedResult = new MigrationController.MigrationResult
            {
                TotalCount = 3,
                SuccessCount = 2,
                FailedCount = 1,
                FailedDeleteCount = 1,
            };

            var userDevices = new[]
            {
                new UserDevice { DeviceId = "firstDeviceId", RegistrationId = "firstOriginalRegistrationId", PnsToken = AndroidPnsToken, NhsLoginId = "firstNhsLoginId" },
                new UserDevice { DeviceId = "secondDeviceId", RegistrationId = "secondOriginalRegistrationId", PnsToken = IosPnsToken, NhsLoginId = "secondNhsLoginId" },
                new UserDevice { DeviceId = "thirdDeviceId", RegistrationId = "thirdOriginalRegistrationId", PnsToken = AndroidPnsToken, NhsLoginId = "thirdNhsLoginId" }
            };

            _mockDeviceRepositoryService.Setup(x => x.FindRegistrations(It.IsAny<int>()))
                .ReturnsAsync(new SearchDeviceResult.FoundMany(userDevices));

            SetupSuccessfulMockSequence(userDevices[0], DeviceType.Android, "firstRegistrationId");

            _mockNotificationRegistrationService
                .Setup(x => x.Exists(userDevices[1]))
                .ReturnsAsync(new RegistrationExistsResult.Found());

            _mockMigrationService
                .Setup(x => x.Register(userDevices[1].PnsToken, DeviceType.Ios, userDevices[1].NhsLoginId))
                .ReturnsAsync(new RegistrationResult.Success(new NotificationRegistrationResult
                    { Id = "secondRegistrationId" }));

            _mockDeviceRepositoryService
                .Setup(x => x.Update(userDevices[1].DeviceId, userDevices[1].NhsLoginId, "secondRegistrationId"))
                .ReturnsAsync(new UpdateDeviceResult.InternalServerError());

            _mockMigrationService
                .Setup(x => x.Delete("secondRegistrationId"))
                .ReturnsAsync(new DeleteRegistrationResult.BadGateway());

            SetupSuccessfulMockSequence(userDevices[2], DeviceType.Android, "thirdRegistrationId");

            // Act
            var result = await _systemUnderTest.MigrateRegistration(50);

            // Assert
            _mockMigrationService.VerifyAll();
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationRegistrationService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            objectResult.Value.Should().BeEquivalentTo(expectedResult);
        }

        private void SetupSuccessfulMockSequence(UserDevice userDevice, DeviceType deviceType, string registrationId)
        {
            _mockNotificationRegistrationService
                .Setup(x => x.Exists(userDevice))
                .ReturnsAsync(new RegistrationExistsResult.Found());

            _mockMigrationService
                .Setup(x => x.Register(userDevice.PnsToken, deviceType, userDevice.NhsLoginId))
                .ReturnsAsync(new RegistrationResult.Success(new NotificationRegistrationResult
                    { Id = registrationId }));

            _mockDeviceRepositoryService
                .Setup(x => x.Update(userDevice.DeviceId, userDevice.NhsLoginId, registrationId))
                .ReturnsAsync(new UpdateDeviceResult.Updated());

            _mockMigrationService
                .Setup(x => x.Delete(userDevice.RegistrationId))
                .ReturnsAsync(new DeleteRegistrationResult.Success());
        }

        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}
