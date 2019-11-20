using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.UsersApi.Areas.Devices;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.UsersApi.UnitTests.Areas.Devices
{
    [TestClass]
    public sealed class DevicesControllerPostTests : IDisposable
    {
        private IFixture _fixture;
        private DevicesController _systemUnderTest;
        private RegisterDeviceRequest _validRegisterDeviceRequest;
        private Mock<INotificationService> _mockNotificationService;
        private Mock<IDeviceRepositoryService> _mockDeviceRepositoryService;
        private Mock<IAuditor> _mockAuditor;

        private const string RegisterUsersDeviceAuditTypeRequest = "Users_Device_Registration_Request";
        private const string RegisterUsersDeviceAuditTypeResponse = "Users_Device_Registration_Response";

        private const string RequestAuditMessage = "Attempting to register user notification registration";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            var mockHttpContext = HttpContextGetAccessTokenHelper.CreateMockHttpContext(_fixture);

            _mockNotificationService = _fixture.Freeze<Mock<INotificationService>>();
            _mockDeviceRepositoryService = _fixture.Freeze<Mock<IDeviceRepositoryService>>();
            _validRegisterDeviceRequest = _fixture.Create<RegisterDeviceRequest>();

            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();

            _systemUnderTest = _fixture.Create<DevicesController>();
            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };
        }

        [TestMethod]
        public async Task Post_Success()
        {
            // Arrange
            _mockNotificationService.Setup(x => x.Register(_validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(_fixture.Create<RegistrationResult.Success>());

            var userDevice = _fixture.Create<UserDevice>();

            _mockDeviceRepositoryService.Setup(x => x.Create(
                    It.IsAny<NotificationRegistrationResult>(),
                    _validRegisterDeviceRequest,
                    It.IsAny<AccessToken>()))
                .ReturnsAsync(new DeviceRegistrationResult.Created(userDevice));

            // Act
            var result = await _systemUnderTest.Post(_validRegisterDeviceRequest);

            // Assert
            _mockDeviceRepositoryService.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<ObjectResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status201Created);

            var resultBody = objectResult.Subject.Value.Should().BeAssignableTo<Device>().Subject;
            resultBody.DeviceType.Should().Be(_validRegisterDeviceRequest.DeviceType);
            resultBody.DeviceId.Should().Be(userDevice.DeviceId);

            AssertAudit(RegisterUsersDeviceAuditTypeRequest, RequestAuditMessage);
            AssertAudit(RegisterUsersDeviceAuditTypeResponse, "User device successfully added to the repository for push notifications");
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
        public async Task Post_NullDeviceType_ReturnsBadRequest()
        {
            // Arrange
            var registerDeviceRequest = _fixture.Build<RegisterDeviceRequest>()
                .With(x => x.DeviceType, (DeviceType?) null).Create();

            // Act
            var result = await _systemUnderTest.Post(registerDeviceRequest);

            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_NullPnsToken_ReturnsBadRequest()
        {
            // Arrange
            var registerDeviceRequest = _fixture.Build<RegisterDeviceRequest>()
                .With(x => x.DevicePns, (string) null).Create();

            // Act
            var result = await _systemUnderTest.Post(registerDeviceRequest);

            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_HubServiceRegistrationFailure_ReturnsBadGateway()
        {
            // Arrange
            _mockNotificationService.Setup(x => x.Register(_validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(_fixture.Create<RegistrationResult.BadGateway>());

            // Act
            var result = await _systemUnderTest.Post(_validRegisterDeviceRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);

            AssertAudit(RegisterUsersDeviceAuditTypeRequest, RequestAuditMessage);
            AssertAudit(RegisterUsersDeviceAuditTypeResponse, "User device failed to register for push notifications due to BadGateway");
        }

        [TestMethod]
        public async Task Post_HubServiceInternalServerError_ReturnsInternalServerError()
        {
            // Arrange
            _mockNotificationService.Setup(x => x.Register(_validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(_fixture.Create<RegistrationResult.InternalServerError>());

            // Act
            var result = await _systemUnderTest.Post(_validRegisterDeviceRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            AssertAudit(RegisterUsersDeviceAuditTypeRequest, RequestAuditMessage);
            AssertAudit(RegisterUsersDeviceAuditTypeResponse, "User device failed to register for push notifications due to InternalServerError");
        }

        [TestMethod]
        public async Task Post_HubServiceRegistrationException_ReturnsInternalServerError()
        {
            // Arrange
            _mockNotificationService.Setup(x => x.Register(_validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Post(_validRegisterDeviceRequest);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            AssertAudit(RegisterUsersDeviceAuditTypeRequest, RequestAuditMessage);
         }

        [TestMethod]
        public async Task Post_DeviceRegistrationInternalServerError_ReturnsInternalServerError()
        {
            // Arrange
            _mockNotificationService.Setup(x => x.Register(_validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(_fixture.Create<RegistrationResult.Success>());

            _mockDeviceRepositoryService.Setup(x => x.Create(
                    It.IsAny<NotificationRegistrationResult>(),
                    _validRegisterDeviceRequest,
                    It.IsAny<AccessToken>()))
                .ReturnsAsync(new DeviceRegistrationResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Post(_validRegisterDeviceRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            AssertAudit(RegisterUsersDeviceAuditTypeRequest, RequestAuditMessage);
            AssertAudit(RegisterUsersDeviceAuditTypeResponse, "User device failed to be added to the repository for push notifications due to InternalServerError");
        }

        [TestMethod]
        public async Task Post_DeviceRegistrationBadGateway_ReturnsBadGateway()
        {
            // Arrange
            _mockNotificationService.Setup(x => x.Register(_validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(_fixture.Create<RegistrationResult.Success>());

            _mockDeviceRepositoryService.Setup(x => x.Create(
                    It.IsAny<NotificationRegistrationResult>(),
                    _validRegisterDeviceRequest,
                    It.IsAny<AccessToken>()))
                .ReturnsAsync(new DeviceRegistrationResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Post(_validRegisterDeviceRequest);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);

            AssertAudit(RegisterUsersDeviceAuditTypeRequest, RequestAuditMessage);
            AssertAudit(RegisterUsersDeviceAuditTypeResponse, "User device failed to be added to the repository for push notifications due to BadGateway");
        }

        [TestMethod]
        public async Task Post_DeviceRegistrationException_ReturnsInternalServerError()
        {
            // Arrange
            _mockNotificationService.Setup(x => x.Register(_validRegisterDeviceRequest, It.IsAny<AccessToken>()))
                .ReturnsAsync(_fixture.Create<RegistrationResult.Success>());

            _mockDeviceRepositoryService.Setup(x => x.Create(
                    It.IsAny<NotificationRegistrationResult>(),
                    _validRegisterDeviceRequest,
                    It.IsAny<AccessToken>())
                )
                .Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Post(_validRegisterDeviceRequest);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            AssertAudit(RegisterUsersDeviceAuditTypeRequest, RequestAuditMessage);
        }

        private void AssertAudit(string request, string message)
        {
            _mockAuditor.Verify(x => x.AuditSecureTokenEvent(It.IsAny<AccessToken>(), Supplier.Microsoft, request, message));
        }

        [TestCleanup]
        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}