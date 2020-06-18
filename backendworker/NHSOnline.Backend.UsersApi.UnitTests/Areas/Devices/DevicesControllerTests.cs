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
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Registrations;
using NHSOnline.Backend.UsersApi.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.UsersApi.UnitTests.Areas.Devices
{
    [TestClass]
    public sealed class DevicesControllerTests : IDisposable
    {
        private DevicesController _systemUnderTest;
        private Mock<IRegistrationService> _mockRegistrationService;
        private const string DevicePns = "PNS";
        private readonly DeviceType _deviceType = DeviceType.Android;
        private const string DeviceId = "DeviceId";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRegistrationService = new Mock<IRegistrationService>();

            _systemUnderTest = new DevicesController(
                    _mockRegistrationService.Object,
                    new Mock<ILogger<DevicesController>>().Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = HttpContextGetAccessTokenHelper.CreateMockHttpContext().Object
                }
            };
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
        public async Task Get_Success_ReturnsStatus204NoContent()
        {
            // Arrange
            _mockRegistrationService
                .Setup(x => x.GetRegistration(DevicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new RegistrationExistsResult.Found());

            // Act
            var result = await _systemUnderTest.Get(DevicePns);

            // Assert
            _mockRegistrationService.VerifyAll();

            result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }

        [TestMethod]
        public async Task Get_RegistrationNotFound_ReturnsStatus404NotFound()
        {
            // Arrange
            _mockRegistrationService
                .Setup(x => x.GetRegistration(DevicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new RegistrationExistsResult.NotFound());

            // Act
            var result = await _systemUnderTest.Get(DevicePns);

            // Assert
            _mockRegistrationService.VerifyAll();

            result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [TestMethod]
        public async Task Get_RegistrationBadGateway_ReturnsStatus502BadGateway()
        {
            // Arrange
            _mockRegistrationService
                .Setup(x => x.GetRegistration(DevicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new RegistrationExistsResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Get(DevicePns);

            // Assert
            _mockRegistrationService.VerifyAll();

            result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Get_RegistrationInternalServerError_ReturnsStatus500InternalServerError()
        {
            // Arrange
            _mockRegistrationService
                .Setup(x => x.GetRegistration(DevicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new RegistrationExistsResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Get(DevicePns);

            // Assert
            _mockRegistrationService.VerifyAll();

            result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Get_RegistrationThrowsAnException_ReturnsStatus500InternalServerError()
        {
            // Arrange
            _mockRegistrationService
                .Setup(x => x.GetRegistration(DevicePns, It.IsAny<AccessToken>()))
                .ThrowsAsync(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Get(DevicePns);

            // Assert
            _mockRegistrationService.VerifyAll();

            result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
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
        public async Task Delete_Success_ReturnsStatus204NoContent()
        {
            // Arrange
            _mockRegistrationService
                .Setup(x => x.DeleteRegistration(DevicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new DeleteRegistrationResult.Success());

            // Act
            var result = await _systemUnderTest.Delete(DevicePns);

            // Assert
            _mockRegistrationService.VerifyAll();

            result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }

        [TestMethod]
        public async Task Delete_RegistrationNotFound_ReturnsStatus404NotFound()
        {
            // Arrange
            _mockRegistrationService
                .Setup(x => x.DeleteRegistration(DevicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new DeleteRegistrationResult.NotFound());

            // Act
            var result = await _systemUnderTest.Delete(DevicePns);

            // Assert
            _mockRegistrationService.VerifyAll();

            result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [TestMethod]
        public async Task Delete_RegistrationBadGateway_ReturnsStatus502BadGateway()
        {
            // Arrange
            _mockRegistrationService
                .Setup(x => x.DeleteRegistration(DevicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new DeleteRegistrationResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Delete(DevicePns);

            // Assert
            _mockRegistrationService.VerifyAll();

            result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Delete_RegistrationInternalServerError_ReturnsStatus500InternalServerError()
        {
            // Arrange
            _mockRegistrationService
                .Setup(x => x.DeleteRegistration(DevicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new DeleteRegistrationResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Delete(DevicePns);

            // Assert
            _mockRegistrationService.VerifyAll();

            result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Delete_RegistrationThrowsException_ReturnsStatus500InternalServerError()
        {
            // Arrange
            _mockRegistrationService
                .Setup(x => x.DeleteRegistration(DevicePns, It.IsAny<AccessToken>()))
                .ThrowsAsync(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Delete(DevicePns);

            // Assert
            _mockRegistrationService.VerifyAll();

            result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Post_NullRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _systemUnderTest.Post(null);

            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_Success_ReturnsStatus201Created()
        {
            // Arrange
            var registerRequest = CreateValidRegisterDeviceRequest();
            var expectedResult = new Device
            {
                DeviceId = DeviceId,
                DeviceType = _deviceType
            };

            _mockRegistrationService
                .Setup(x => x.CreateRegistration(registerRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(new RegisterDeviceResult.Created(new UserDevice { DeviceId = DeviceId }));

            // Act
            var result = await _systemUnderTest.Post(registerRequest);

            // Assert
            _mockRegistrationService.VerifyAll();

            var subject = result.Should().BeOfType<ObjectResult>().Subject;
            subject.StatusCode.Should().Be(StatusCodes.Status201Created);
            subject.Value.Should().BeOfType<Device>().Subject.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task Post_RegistrationBadGateway_ReturnsStatus502BadGateway()
        {
            // Arrange
            var registerRequest = CreateValidRegisterDeviceRequest();

            _mockRegistrationService
                .Setup(x => x.CreateRegistration(registerRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(new RegisterDeviceResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Post(registerRequest);

            // Assert
            _mockRegistrationService.VerifyAll();

            result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Post_RegistrationInternalServerError_ReturnsStatus500InternalServerError()
        {
            // Arrange
            var registerRequest = CreateValidRegisterDeviceRequest();

            _mockRegistrationService
                .Setup(x => x.CreateRegistration(registerRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(new RegisterDeviceResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Post(registerRequest);

            // Assert
            _mockRegistrationService.VerifyAll();

            result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Post_RegistrationThrowsException_ReturnsStatus500InternalServerError()
        {
            // Arrange
            var registerRequest = CreateValidRegisterDeviceRequest();

            _mockRegistrationService
                .Setup(x => x.CreateRegistration(registerRequest, It.IsAny<AccessToken>()))
                .ThrowsAsync(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Post(registerRequest);

            // Assert
            _mockRegistrationService.VerifyAll();

            result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Post_InvalidDevicePns_ReturnsBadRequest(string devicePns)
        {
            // Arrange
            var registerRequest = new RegisterDeviceRequest
            {
                DevicePns = devicePns,
                DeviceType = DeviceType.Android
            };

            // Act
            var result = await _systemUnderTest.Post(registerRequest);

            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_NullDeviceType_ReturnsBadRequest()
        {
            // Arrange
            var registerRequest = new RegisterDeviceRequest
            {
                DevicePns = DevicePns,
                DeviceType = null
            };

            // Act
            var result = await _systemUnderTest.Post(registerRequest);

            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        private RegisterDeviceRequest CreateValidRegisterDeviceRequest()
            => new RegisterDeviceRequest
            {
                DevicePns = DevicePns,
                DeviceType = _deviceType
            };

        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}