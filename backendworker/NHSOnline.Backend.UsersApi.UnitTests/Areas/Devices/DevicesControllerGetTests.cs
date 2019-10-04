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
    public sealed class DevicesControllerGetTests : IDisposable
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
        public async Task Get_Success()
        {
            // Arrange
            var devicePns = _fixture.Create<string>();
            var userDevice = _fixture.Build<UserDevice>()
                .With(u => u.PnsToken, devicePns)
                .Create();

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockNotificationService.Setup(x => x.Exists(userDevice))
                .ReturnsAsync(_fixture.Create<RegistrationExistsResult.Found>());

            // Act
            var result = await _systemUnderTest.Get(devicePns);

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
        public async Task Get_InvalidDevicePns_ReturnsBadRequest(string devicePns)
        {
            // Act
            var result = await _systemUnderTest.Get(devicePns);

            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Get_NotFoundUserDevice_ReturnsNotFound()
        {
            // Arrange
            var devicePns = _fixture.Create<string>();

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new SearchDeviceResult.NotFound());

            // Act
            var result = await _systemUnderTest.Get(devicePns);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [TestMethod]
        public async Task Get_FoundOrphanUserDevice_DeletesOrphanAndReturnsNotFound()
        {
            // Arrange
            var devicePns = _fixture.Create<string>();
            var userDevice = _fixture.Build<UserDevice>()
                .With(u => u.PnsToken, devicePns)
                .Create();

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockDeviceRepositoryService.Setup(x => x.Delete(userDevice.DeviceId, It.IsAny<AccessToken>()))
                .ReturnsAsync(new DeleteDeviceResult.Success(devicePns));

            _mockNotificationService.Setup(x => x.Exists(userDevice))
                .ReturnsAsync(_fixture.Create<RegistrationExistsResult.NotFound>());

            // Act
            var result = await _systemUnderTest.Get(devicePns);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [TestMethod]
        public async Task Get_FindDeviceException_ReturnsInternalServerError()
        {
            // Arrange
            var devicePns = _fixture.Create<string>();

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new SearchDeviceResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Get(devicePns);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Get_FindDeviceBadGateway_ReturnsServiceUnavailable()
        {
            // Arrange
            var devicePns = _fixture.Create<string>();

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new SearchDeviceResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Get(devicePns);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Get_RegistrationExistsException_ReturnsInternalServerError()
        {
            // Arrange
            var devicePns = _fixture.Create<string>();
            var userDevice = _fixture.Build<UserDevice>()
                .With(u => u.PnsToken, devicePns)
                .Create();

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockNotificationService.Setup(x => x.Exists(userDevice))
                .ReturnsAsync(_fixture.Create<RegistrationExistsResult.InternalServerError>());

            // Act
            var result = await _systemUnderTest.Get(devicePns);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();
            _mockNotificationService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Get_RegistrationExistsBadGateway_ReturnsServiceUnavailable()
        {
            // Arrange
            var devicePns = _fixture.Create<string>();
            var userDevice = _fixture.Build<UserDevice>()
                .With(u => u.PnsToken, devicePns)
                .Create();

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockNotificationService.Setup(x => x.Exists(userDevice))
                .ReturnsAsync(_fixture.Create<RegistrationExistsResult.BadGateway>());

            // Act
            var result = await _systemUnderTest.Get(devicePns);

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