using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private IFixture _fixture;
        private DevicesController _systemUnderTest;
        private Mock<INotificationService> _mockNotificationService;
        private Mock<IDeviceRepositoryService> _mockDeviceRepositoryService;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _mockNotificationService = _fixture.Freeze<Mock<INotificationService>>();
            _mockDeviceRepositoryService = _fixture.Freeze<Mock<IDeviceRepositoryService>>();

            _systemUnderTest = _fixture.Create<DevicesController>();
            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = HttpContextGetAccessTokenHelper.CreateMockHttpContext(_fixture).Object
            };
        }

        [TestMethod]
        public async Task Delete_Success()
        {
            // Arrange
            var devicePns = _fixture.Create<string>();
            var userDevice = _fixture.Build<UserDevice>()
                .With(u => u.PnsToken, devicePns)
                .Create();

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockDeviceRepositoryService.Setup(x => x.Delete(userDevice.DeviceId, It.IsAny<AccessToken>()))
                .ReturnsAsync(_fixture.Create<DeleteDeviceResult.Success>());

            _mockNotificationService.Setup(x => x.Delete(userDevice.RegistrationId))
                .ReturnsAsync(_fixture.Create<DeleteRegistrationResult.Success>());

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
            var devicePns = _fixture.Create<string>();

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
            var devicePns = _fixture.Create<string>();

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
        public async Task Delete_FindDeviceBadGateway_ReturnsServiceUnavailable()
        {
            // Arrange
            var devicePns = _fixture.Create<string>();

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
            var devicePns = _fixture.Create<string>();
            var userDevice = _fixture.Build<UserDevice>()
                .With(u => u.PnsToken, devicePns)
                .Create();

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockNotificationService.Setup(x => x.Delete(userDevice.RegistrationId))
                .ReturnsAsync(_fixture.Create<DeleteRegistrationResult.InternalServerError>());

            // Act
            var result = await _systemUnderTest.Delete(devicePns);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Delete_DeleteRegistrationBadGateway_ReturnsServiceUnavailable()
        {
            // Arrange
            var devicePns = _fixture.Create<string>();
            var userDevice = _fixture.Build<UserDevice>()
                .With(u => u.PnsToken, devicePns)
                .Create();

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockNotificationService.Setup(x => x.Delete(userDevice.RegistrationId))
                .ReturnsAsync(_fixture.Create<DeleteRegistrationResult.BadGateway>());

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
            var devicePns = _fixture.Create<string>();
            var userDevice = _fixture.Build<UserDevice>()
                .With(u => u.PnsToken, devicePns)
                .Create();

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockDeviceRepositoryService.Setup(x => x.Delete(userDevice.DeviceId, It.IsAny<AccessToken>()))
                .ReturnsAsync(_fixture.Create<DeleteDeviceResult.InternalServerError>());

            _mockNotificationService.Setup(x => x.Delete(userDevice.RegistrationId))
                .ReturnsAsync(_fixture.Create<DeleteRegistrationResult.Success>());

            // Act
            var result = await _systemUnderTest.Delete(devicePns);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Delete_DeleteDeviceBadGateway_ReturnsServiceUnavailable()
        {
            // Arrange
            var devicePns = _fixture.Create<string>();
            var userDevice = _fixture.Build<UserDevice>()
                .With(u => u.PnsToken, devicePns)
                .Create();

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockDeviceRepositoryService.Setup(x => x.Delete(userDevice.DeviceId, It.IsAny<AccessToken>()))
                .ReturnsAsync(_fixture.Create<DeleteDeviceResult.BadGateway>());

            _mockNotificationService.Setup(x => x.Delete(userDevice.RegistrationId))
                .ReturnsAsync(_fixture.Create<DeleteRegistrationResult.Success>());

            // Act
            var result = await _systemUnderTest.Delete(devicePns);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestCleanup]
        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}