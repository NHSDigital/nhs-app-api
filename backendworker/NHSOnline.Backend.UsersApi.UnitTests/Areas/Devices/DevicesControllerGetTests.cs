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
        private Mock<IAuditor> _mockAuditor;

        private const string GetUsersDeviceAuditTypeRequest = "Users_Device_Get_Request";
        private const string GetUsersDeviceAuditTypeReponse = "Users_Device_Get_Response";
        private const string GetUsersDeviceAuditTypeResponse = "Users_Device_Get_Response";

        private const string RequestAuditMessage = "Attempting to get user notification registration";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _mockNotificationService = _fixture.Freeze<Mock<INotificationService>>();
            _mockDeviceRepositoryService = _fixture.Freeze<Mock<IDeviceRepositoryService>>();

            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();

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
            
            AssertAudit(GetUsersDeviceAuditTypeRequest, RequestAuditMessage);
            AssertAudit(GetUsersDeviceAuditTypeResponse, "User device registration found");
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

            AssertAudit(GetUsersDeviceAuditTypeRequest, RequestAuditMessage);
            AssertAudit(GetUsersDeviceAuditTypeReponse, "No user device registrations for notifications found");
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

            AssertAudit(GetUsersDeviceAuditTypeRequest, RequestAuditMessage);
            AssertAudit(GetUsersDeviceAuditTypeReponse, "No user device registrations for notifications found");
        }

        [TestMethod]
        public async Task Get_FoundOrphanUserDevice_DeleteBadGateway_ReturnsBadGateway()
        {
            // Arrange
            var devicePns = _fixture.Create<string>();
            var userDevice = _fixture.Build<UserDevice>()
                .With(u => u.PnsToken, devicePns)
                .Create();

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .ReturnsAsync(new SearchDeviceResult.Found(userDevice));

            _mockNotificationService.Setup(x => x.Exists(userDevice))
                .ReturnsAsync(_fixture.Create<RegistrationExistsResult.NotFound>());

            _mockDeviceRepositoryService.Setup(x => x.Delete(userDevice.DeviceId, It.IsAny<AccessToken>()))
                .ReturnsAsync(_fixture.Create<DeleteDeviceResult.BadGateway>());

            // Act
            var result = await _systemUnderTest.Get(devicePns);

            // Assert
            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);

            AssertAudit(GetUsersDeviceAuditTypeRequest, RequestAuditMessage);
            AssertAudit(GetUsersDeviceAuditTypeReponse, "User device notification registration deletion unsuccessful due to BadGateway");
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

            AssertAudit(GetUsersDeviceAuditTypeRequest, RequestAuditMessage);
            AssertAudit(GetUsersDeviceAuditTypeReponse, "User devices registrations for notifications search unsuccessful due to InternalServerError");
        }

        [TestMethod]
        public async Task Get_FindDeviceBadGateway_ReturnsBadGateway()
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

            AssertAudit(GetUsersDeviceAuditTypeRequest, RequestAuditMessage);
            AssertAudit(GetUsersDeviceAuditTypeReponse, "User devices registrations for notifications search unsuccessful due to BadGateway");

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

            AssertAudit(GetUsersDeviceAuditTypeRequest, RequestAuditMessage);
            AssertAudit(GetUsersDeviceAuditTypeResponse, "User device registration search unsuccessful due to InternalServerError");
        }

        [TestMethod]
        public async Task Get_RegistrationExistsBadGateway_ReturnsBadGateway()
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

            AssertAudit(GetUsersDeviceAuditTypeRequest, RequestAuditMessage);
            AssertAudit(GetUsersDeviceAuditTypeResponse, "User device registration search unsuccessful due to BadGateway");
        }

        [TestMethod]
        public async Task Get_DeviceDeletionException_ReturnsInternalServerError()
        {
            // Arrange
            var devicePns = _fixture.Create<string>();

            _mockDeviceRepositoryService.Setup(x => x.Find(devicePns, It.IsAny<AccessToken>()))
                .Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Get(devicePns);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            AssertAudit(GetUsersDeviceAuditTypeRequest, RequestAuditMessage);
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