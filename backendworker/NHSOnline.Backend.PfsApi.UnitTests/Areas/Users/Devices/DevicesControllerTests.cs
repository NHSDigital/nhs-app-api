using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Auth;
using NHSOnline.Backend.PfsApi.Areas.Users.Devices;
using NHSOnline.Backend.Users.Areas.Devices.Models;
using NHSOnline.Backend.Users.Notifications;
using NHSOnline.Backend.Users.Registrations;
using NHSOnline.Backend.Users.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Users.Devices
{
    [TestClass]
    public sealed class DevicesControllerTests : IDisposable
    {
        private DevicesController _systemUnderTest;
        private Mock<IRegistrationService> _mockRegistrationService;
        private Mock<IMetricLogger<AccessTokenMetricContext>> _mockMetricLogger;
        private Mock<ILogger<DevicesController>> _mockLogger;
        private Mock<IAuditor> _mockAuditor;
        private const string DevicePns = "PNS";
        private static readonly string InstallationId = Guid.NewGuid().ToString();
        private const DeviceType DeviceType = Backend.Users.Areas.Devices.Models.DeviceType.Android;
        private const string DeviceId = "DeviceId";
        private const string NhsNumber = "NhsNumber";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger<DevicesController>>();
            _mockMetricLogger = new Mock<IMetricLogger<AccessTokenMetricContext>>();
            _mockRegistrationService = new Mock<IRegistrationService>();
            _mockAuditor = new Mock<IAuditor>();

            var mockAccessTokenProvider = new Mock<IAccessTokenProvider>();
            mockAccessTokenProvider.SetupGet(x => x.AccessToken)
                .Returns(AccessTokenMock.Generate(nhsNumber: NhsNumber));

            _systemUnderTest = new DevicesController(
                _mockRegistrationService.Object,
                _mockLogger.Object,
                _mockMetricLogger.Object,
                mockAccessTokenProvider.Object);
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
            _mockMetricLogger.VerifyNoOtherCalls();
            _mockAuditor.VerifyNoOtherCalls();
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
            _mockMetricLogger.Verify(x => x.NotificationsDisabled());

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
            _mockMetricLogger.VerifyNoOtherCalls();
            _mockAuditor.VerifyNoOtherCalls();

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
            _mockMetricLogger.VerifyNoOtherCalls();
            _mockAuditor.VerifyNoOtherCalls();

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
            _mockMetricLogger.VerifyNoOtherCalls();
            _mockAuditor.VerifyNoOtherCalls();

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
            _mockMetricLogger.VerifyNoOtherCalls();
            _mockAuditor.VerifyNoOtherCalls();

            result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Post_NullRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _systemUnderTest.Post(null);

            // Assert
            _mockMetricLogger.VerifyNoOtherCalls();
            _mockAuditor.VerifyNoOtherCalls();
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_Success_ReturnsStatus200Created()
        {
            // Arrange
            var registerRequest = CreateValidRegisterDeviceRequest();
            var expectedResult = new Device
            {
                DeviceId = DeviceId,
                DeviceType = DeviceType,
                InstallationId = InstallationId,
            };

            _mockRegistrationService
                .Setup(x => x.CreateRegistration(registerRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(new RegisterDeviceResult.Created(new UserDevice { DeviceId = DeviceId, RegistrationId = InstallationId }));

            // Act
            var result = await _systemUnderTest.Post(registerRequest);

            // Assert
            _mockRegistrationService.VerifyAll();
            _mockMetricLogger.Verify(x => x.NotificationsEnabled());

            var subject = result.Should().BeOfType<ObjectResult>().Subject;
            subject.StatusCode.Should().Be(StatusCodes.Status200OK);
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
            _mockMetricLogger.VerifyNoOtherCalls();

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
            _mockMetricLogger.VerifyNoOtherCalls();
            _mockAuditor.VerifyNoOtherCalls();

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
            _mockMetricLogger.VerifyNoOtherCalls();
            _mockAuditor.VerifyNoOtherCalls();

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
                InstallationId = InstallationId,
                DevicePns = devicePns,
                DeviceType = DeviceType.Android
            };

            // Act
            var result = await _systemUnderTest.Post(registerRequest);

            // Assert
            _mockMetricLogger.VerifyNoOtherCalls();
            _mockAuditor.VerifyNoOtherCalls();
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_NullDeviceType_ReturnsBadRequest()
        {
            // Arrange
            var registerRequest = new RegisterDeviceRequest
            {
                InstallationId = InstallationId,
                DevicePns = DevicePns,
                DeviceType = null
            };

            // Act
            var result = await _systemUnderTest.Post(registerRequest);

            // Assert
            _mockMetricLogger.VerifyNoOtherCalls();
            _mockAuditor.VerifyNoOtherCalls();
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [DataTestMethod]
        [DataRow("   ")]
        [DataRow("34234")]
        public async Task Post_InvalidInstallationId_ReturnsBadRequest(string installationId)
        {
            // Arrange
            var registerRequest = new RegisterDeviceRequest
            {
                InstallationId = installationId,
                DevicePns = DevicePns,
                DeviceType = DeviceType.Android
            };

            // Act
            var result = await _systemUnderTest.Post(registerRequest);

            // Assert
            _mockMetricLogger.VerifyNoOtherCalls();
            _mockAuditor.VerifyNoOtherCalls();
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public async Task Post_InstallationIdWithNullEmpty_ReturnsOK(string installationId)
        {
            // Arrange
            var registerRequest = new RegisterDeviceRequest
            {
                InstallationId = installationId,
                DevicePns = DevicePns,
                DeviceType = DeviceType.Android
            };
            var expectedResult = new Device
            {
                DeviceId = DeviceId,
                DeviceType = DeviceType
            };

            _mockRegistrationService
                .Setup(x => x.CreateRegistration(registerRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(new RegisterDeviceResult.Created(new UserDevice { DeviceId = DeviceId }));

            // Act
            var result = await _systemUnderTest.Post(registerRequest);

            // Assert
            _mockRegistrationService.VerifyAll();
            _mockMetricLogger.Verify(x => x.NotificationsEnabled());

            var subject = result.Should().BeOfType<ObjectResult>().Subject;
            subject.StatusCode.Should().Be(StatusCodes.Status200OK);
            subject.Value.Should().BeOfType<Device>().Subject.Should().BeEquivalentTo(expectedResult);
        }

        private RegisterDeviceRequest CreateValidRegisterDeviceRequest()
            => new RegisterDeviceRequest
            {
                InstallationId = InstallationId,
                DevicePns = DevicePns,
                DeviceType = DeviceType
            };

        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}