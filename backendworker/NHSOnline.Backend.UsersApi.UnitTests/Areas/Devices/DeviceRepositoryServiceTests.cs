using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.UsersApi.Areas.Devices;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.UsersApi.UnitTests.Areas.Devices
{
    [TestClass]
    public class DeviceRepositoryServiceTests
    {
        private IFixture _fixture;
        private DeviceRepositoryService _systemUnderTest;
        private Mock<IUserDeviceRepository> _mockDeviceRepository;
        private Mock<IDeviceIdGenerator> _mockDeviceIdGenerator;
        private AccessToken _accessToken;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockDeviceRepository = _fixture.Freeze<Mock<IUserDeviceRepository>>();
            _mockDeviceIdGenerator = _fixture.Freeze<Mock<IDeviceIdGenerator>>();
            var mockLogger = _fixture.Freeze<Mock<ILogger<DeviceRepositoryService>>>();
            var accessTokenString = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _fixture.Create<string>()),
                new Claim("nhs_number", _fixture.Create<string>()),
            });

            _accessToken = AccessToken.Parse(mockLogger.Object, accessTokenString);

            _systemUnderTest = _fixture.Create<DeviceRepositoryService>();
        }

        [TestMethod]
        public async Task Create_Success()
        {
            // Arrange
            var request = _fixture.Create<RegisterDeviceRequest>();
            var registration = _fixture.Create<NotificationRegistrationResult>();
            var expectedDeviceId = _fixture.Create<string>();

            _mockDeviceIdGenerator.Setup(x => x.Generate(_accessToken, request)).Returns(expectedDeviceId);
            _mockDeviceRepository.Setup(x => x.Create(It.IsAny<UserDevice>())).Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Create(registration, request, _accessToken);

            // Assert
            _mockDeviceIdGenerator.VerifyAll();
            _mockDeviceRepository.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<DeviceRegistrationResult.Created>();
            objectResult.Subject.UserDevice.Should().NotBeNull();
            objectResult.Subject.UserDevice.DeviceId.Should().Be(expectedDeviceId);
            objectResult.Subject.UserDevice.NhsLoginId.Should().Be(_accessToken.Subject);
            objectResult.Subject.UserDevice.PnsToken.Should().Be(request.DevicePns);
            objectResult.Subject.UserDevice.RegistrationId.Should().Be(registration.RegistrationId);
            objectResult.Subject.UserDevice.RegistrationExpiry.Should().Be(registration.RegistrationExpiry);
        }

        [TestMethod]
        public async Task Create_RepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var request = _fixture.Create<RegisterDeviceRequest>();
            var expectedDeviceId = _fixture.Create<string>();

            _mockDeviceIdGenerator.Setup(x => x.Generate(_accessToken, request)).Returns(expectedDeviceId);
            _mockDeviceRepository.Setup(x => x.Create(It.IsAny<UserDevice>())).Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Create(_fixture.Create<NotificationRegistrationResult>(), request,
                _accessToken);

            // Assert
            _mockDeviceIdGenerator.VerifyAll();
            _mockDeviceRepository.VerifyAll();

            result.Should().BeAssignableTo<DeviceRegistrationResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Create_RepositoryThrowsMongoException_ReturnsBadGateway()
        {
            // Arrange
            var request = _fixture.Create<RegisterDeviceRequest>();
            var expectedDeviceId = _fixture.Create<string>();

            _mockDeviceIdGenerator.Setup(x => x.Generate(_accessToken, request)).Returns(expectedDeviceId);
            _mockDeviceRepository.Setup(x => x.Create(It.IsAny<UserDevice>())).Throws(new MongoException("Test"));

            // Act
            var result = await _systemUnderTest.Create(_fixture.Create<NotificationRegistrationResult>(), request,
                _accessToken);

            // Assert
            _mockDeviceIdGenerator.VerifyAll();
            _mockDeviceRepository.VerifyAll();

            result.Should().BeAssignableTo<DeviceRegistrationResult.BadGateway>();
        }

        [TestMethod]
        public async Task Find_Success()
        {
            // Arrange
            var devicePns = _fixture.Create<string>();
            var expectedDeviceId = _fixture.Create<string>();
            var expectedUserDevice = _fixture.Build<UserDevice>()
                .With(u => u.DeviceId, expectedDeviceId)
                .Create();

            _mockDeviceIdGenerator.Setup(x => x.Generate(_accessToken, devicePns)).Returns(expectedDeviceId);
            _mockDeviceRepository.Setup(x => x.Find(_accessToken.Subject, expectedDeviceId))
                .ReturnsAsync(expectedUserDevice);

            // Act
            var result = await _systemUnderTest.Find(devicePns, _accessToken);

            // Assert
            _mockDeviceIdGenerator.VerifyAll();
            _mockDeviceRepository.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<SearchDeviceResult.Found>();
            objectResult.Subject.UserDevice.Should().Be(expectedUserDevice);
        }

        [TestMethod]
        public async Task Find_RepositoryDoesNotFindRecord_ReturnNotFound()
        {
            // Arrange
            var devicePns = _fixture.Create<string>();
            var expectedDeviceId = _fixture.Create<string>();

            _mockDeviceIdGenerator.Setup(x => x.Generate(_accessToken, devicePns)).Returns(expectedDeviceId);
            _mockDeviceRepository.Setup(x => x.Find(_accessToken.Subject, expectedDeviceId))
                .ReturnsAsync((UserDevice) null);

            // Act
            var result = await _systemUnderTest.Find(devicePns, _accessToken);

            // Assert
            _mockDeviceIdGenerator.VerifyAll();
            _mockDeviceRepository.VerifyAll();

            result.Should().BeAssignableTo<SearchDeviceResult.NotFound>();
        }

        [TestMethod]
        public async Task Find_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var devicePns = _fixture.Create<string>();
            var expectedDeviceId = _fixture.Create<string>();

            _mockDeviceIdGenerator.Setup(x => x.Generate(_accessToken, devicePns)).Returns(expectedDeviceId);
            _mockDeviceRepository.Setup(x => x.Find(_accessToken.Subject, It.IsAny<string>()))
                .Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Find(devicePns, _accessToken);

            // Assert
            _mockDeviceIdGenerator.VerifyAll();
            _mockDeviceRepository.VerifyAll();

            result.Should().BeAssignableTo<SearchDeviceResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Find_RepositoryThrowsMongoException_ReturnsBadGateway()
        {
            // Arrange
            var devicePns = _fixture.Create<string>();
            var expectedDeviceId = _fixture.Create<string>();

            _mockDeviceIdGenerator.Setup(x => x.Generate(_accessToken, devicePns)).Returns(expectedDeviceId);
            _mockDeviceRepository.Setup(x => x.Find(_accessToken.Subject, It.IsAny<string>()))
                .Throws(new MongoException("Test"));

            // Act
            var result = await _systemUnderTest.Find(devicePns, _accessToken);

            // Assert
            _mockDeviceIdGenerator.VerifyAll();
            _mockDeviceRepository.VerifyAll();

            result.Should().BeAssignableTo<SearchDeviceResult.BadGateway>();
        }

        [TestMethod]
        public async Task Delete_Success()
        {
            // Arrange
            var deviceId = _fixture.Create<string>();
            _mockDeviceRepository.Setup(x => x.Delete(_accessToken.Subject, deviceId)).Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Delete(deviceId, _accessToken);

            // Assert
            _mockDeviceRepository.VerifyAll();

            result.Should().BeAssignableTo<DeleteDeviceResult.Success>();
        }

        [TestMethod]
        public async Task Delete_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var deviceId = _fixture.Create<string>();
            _mockDeviceRepository.Setup(x => x.Delete(_accessToken.Subject, deviceId))
                .Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Delete(deviceId, _accessToken);

            // Assert
            _mockDeviceRepository.VerifyAll();

            result.Should().BeAssignableTo<DeleteDeviceResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Delete_RepositoryThrowsMongoException_ReturnsBadGateway()
        {
            // Arrange
            var deviceId = _fixture.Create<string>();
            _mockDeviceRepository.Setup(x => x.Delete(_accessToken.Subject, deviceId))
                .Throws(new MongoException("Test"));

            // Act
            var result = await _systemUnderTest.Delete(deviceId, _accessToken);

            // Assert
            _mockDeviceRepository.VerifyAll();

            result.Should().BeAssignableTo<DeleteDeviceResult.BadGateway>();
        }
    }
}