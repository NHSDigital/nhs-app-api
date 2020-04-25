using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.UsersApi.Areas.Devices;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.UsersApi.UnitTests.Areas.Devices
{
    [TestClass]
    public sealed class DevicesControllerDeleteTests : IDisposable
    {
        private DevicesController _systemUnderTest;
        private Mock<INotificationService> _mockNotificationService;
        private Mock<IDeviceRepositoryService> _mockDeviceRepositoryService;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockNotificationService = new Mock<INotificationService>();
            _mockDeviceRepositoryService = new Mock<IDeviceRepositoryService>();

            _systemUnderTest = new DevicesController(
                _mockNotificationService.Object,
                _mockDeviceRepositoryService.Object,
                new Mock<ILogger<DevicesController>>().Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = HttpContextGetAccessTokenHelper.CreateMockHttpContext().Object
                }
            };
        }

        [TestMethod]
        public async Task Delete_Success()
        {
            // Arrange
            var devicePns = "device PNS";
            var userDevice = new UserDevice { PnsToken = devicePns };

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockDeviceRepositoryService.Setup(x => x.Delete(userDevice.DeviceId, It.IsAny<AccessToken>()))
                .ReturnsAsync(new DeleteDeviceResult.Success("device id"));

            _mockNotificationService.Setup(x => x.Delete(userDevice.RegistrationId))
                .ReturnsAsync(new DeleteRegistrationResult.Success());

            // Act
            var result = await _systemUnderTest.Delete(devicePns);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("     ")]
        public async Task Delete_InvalidDevicePns_ReturnsBadRequest(string devicePns)
        {
            // Act
            var result = await _systemUnderTest.Delete(devicePns);

            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Delete_NotFoundUserDevice_ReturnsNotFound()
        {
            // Arrange
            var devicePns = "device PNS";

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new SearchDeviceResult.NotFound());

            // Act
            var result = await _systemUnderTest.Delete(devicePns);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [TestMethod]
        public async Task Delete_FindDeviceException_ReturnsInternalServerError()
        {
            // Arrange
            var devicePns = "device PNS";

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new SearchDeviceResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Delete(devicePns);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Delete_FindDeviceBadGateway_ReturnsBadGateway()
        {
            // Arrange
            var devicePns = "device PNS";

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new SearchDeviceResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Delete(devicePns);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Delete_DeleteRegistrationException_ReturnsInternalServerError()
        {
            // Arrange
            var devicePns = "device PNS";
            var userDevice = new UserDevice { PnsToken = devicePns };

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockNotificationService.Setup(x => x.Delete(userDevice.RegistrationId))
                .ReturnsAsync(new DeleteRegistrationResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Delete(devicePns);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
         }

        [TestMethod]
        public async Task Delete_DeleteRegistrationBadGateway_ReturnsBadGateway()
        {
            // Arrange
            var devicePns = "device PNS";
            var userDevice = new UserDevice { PnsToken = devicePns };

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockNotificationService.Setup(x => x.Delete(userDevice.RegistrationId))
                .ReturnsAsync(new DeleteRegistrationResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Delete(devicePns);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Delete_DeleteDeviceException_ReturnsInternalServerError()
        {
            // Arrange
            var devicePns = "device PNS";
            var userDevice = new UserDevice { PnsToken = devicePns };

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockDeviceRepositoryService.Setup(x => x.Delete(userDevice.DeviceId, It.IsAny<AccessToken>()))
                .ReturnsAsync(new DeleteDeviceResult.InternalServerError());

            _mockNotificationService.Setup(x => x.Delete(userDevice.RegistrationId))
                .ReturnsAsync(new DeleteRegistrationResult.Success());

            // Act
            var result = await _systemUnderTest.Delete(devicePns);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Delete_DeleteDeviceBadGateway_ReturnsBadGateway()
        {
            // Arrange
            var devicePns = "device PNS";
            var userDevice = new UserDevice { PnsToken = devicePns };

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockDeviceRepositoryService.Setup(x => x.Delete(userDevice.DeviceId, It.IsAny<AccessToken>()))
                .ReturnsAsync(new DeleteDeviceResult.BadGateway());

            _mockNotificationService.Setup(x => x.Delete(userDevice.RegistrationId))
                .ReturnsAsync(new DeleteRegistrationResult.Success());

            // Act
            var result = await _systemUnderTest.Delete(devicePns);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Delete_DeleteDeviceFromRepositoryBadGateway_ReturnsBadGateway()
        {
            // Arrange
            var devicePns = "device PNS";
            var userDevice = new UserDevice { PnsToken = devicePns };

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockNotificationService.Setup(x => x.Delete(userDevice.RegistrationId))
                .ReturnsAsync(new DeleteRegistrationResult.Success());

            _mockDeviceRepositoryService.Setup(x => x.Delete(userDevice.DeviceId, It.IsAny<AccessToken>()))
                .ReturnsAsync(new DeleteDeviceResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Delete(devicePns);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Delete_DeleteDeviceFromRepositoryInternalServerError_ReturnsInternalServerError()
        {
            // Arrange
            var devicePns = "device PNS";
            var userDevice = new UserDevice { PnsToken = devicePns };


            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockNotificationService.Setup(x => x.Delete(userDevice.RegistrationId))
                .ReturnsAsync(new DeleteRegistrationResult.Success());

            _mockDeviceRepositoryService.Setup(x => x.Delete(userDevice.DeviceId, It.IsAny<AccessToken>()))
                .ReturnsAsync(new DeleteDeviceResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Delete(devicePns);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Delete_DeviceDeletionException_ReturnsInternalServerError()
        {
            // Arrange
            var devicePns = "device PNS";

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Delete(devicePns);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestCleanup]
        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}