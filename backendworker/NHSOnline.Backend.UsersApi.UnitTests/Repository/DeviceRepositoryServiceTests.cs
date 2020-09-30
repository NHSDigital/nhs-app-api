using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.UsersApi.Areas.Devices;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.UsersApi.UnitTests.Repository
{
    [TestClass]
    public class DeviceRepositoryServiceTests
    {
        private DeviceRepositoryService _systemUnderTest;
        private Mock<IUserDeviceRepository> _mockDeviceRepository;
        private Mock<IDeviceIdGenerator> _mockDeviceIdGenerator;
        private AccessToken _accessToken;
        private const string DevicePns = "DevicePns";
        private const string DeviceId = "deviceId";
        private const string NhsLoginId = "nhsLoginId";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockDeviceRepository = new Mock<IUserDeviceRepository>();
            _mockDeviceIdGenerator = new Mock<IDeviceIdGenerator>();
            var mockLogger = new Mock<ILogger<DevicesController>>();
            var accessTokenString = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, NhsLoginId),
                new Claim("nhs_number", "NHSNumber"),
            });

            _accessToken = AccessToken.Parse(mockLogger.Object, accessTokenString);

            _systemUnderTest = new DeviceRepositoryService(_mockDeviceRepository.Object, _mockDeviceIdGenerator.Object, mockLogger.Object);
        }

        private RegisterDeviceRequest CreateRegisterDeviceRequest()
        {
            return new RegisterDeviceRequest { DevicePns = DevicePns, DeviceType = DeviceType.Android };
        }

        [TestMethod]
        public async Task Create_Success()
        {
            // Arrange
            var request = CreateRegisterDeviceRequest();
            var registrationId = "RegistrationId";
            var registrationExpiry = DateTime.Now;
            var registration = new NotificationRegistrationResult
                { Id = registrationId };
            var expectedUserDevice = new UserDevice { DeviceId = DeviceId };

            _mockDeviceIdGenerator.Setup(x => x.Generate(_accessToken, request)).Returns(DeviceId);
            _mockDeviceRepository.Setup(x => x.Create(It.IsAny<UserDevice>()))
                .ReturnsAsync(new RepositoryCreateResult<UserDevice>.Created(expectedUserDevice));

            // Act
            var result = await _systemUnderTest.Create(registration, request, _accessToken);

            // Assert
            _mockDeviceIdGenerator.VerifyAll();
            _mockDeviceRepository.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<RegisterDeviceResult.Created>();
            objectResult.Subject.UserDevice.Should().NotBeNull();
            objectResult.Subject.UserDevice.Should().Be(expectedUserDevice);
        }

        [TestMethod]
        public async Task Create_RepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var request = CreateRegisterDeviceRequest();

            _mockDeviceIdGenerator.Setup(x => x.Generate(_accessToken, request)).Returns(DeviceId);
            _mockDeviceRepository.Setup(x => x.Create(It.IsAny<UserDevice>())).Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Create(new NotificationRegistrationResult(), request, _accessToken);

            // Assert
            _mockDeviceIdGenerator.VerifyAll();
            _mockDeviceRepository.VerifyAll();

            result.Should().BeAssignableTo<RegisterDeviceResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Create_RepositoryError_ReturnsBadGateway()
        {
            // Arrange
            var request = CreateRegisterDeviceRequest();

            _mockDeviceIdGenerator.Setup(x => x.Generate(_accessToken, request)).Returns(DeviceId);
            _mockDeviceRepository.Setup(x => x.Create(It.IsAny<UserDevice>()))
                .ReturnsAsync(new RepositoryCreateResult<UserDevice>.RepositoryError());

            // Act
            var result = await _systemUnderTest.Create(new NotificationRegistrationResult(), request, _accessToken);

            // Assert
            _mockDeviceIdGenerator.VerifyAll();
            _mockDeviceRepository.VerifyAll();

            result.Should().BeAssignableTo<RegisterDeviceResult.BadGateway>();
        }

        [TestMethod]
        public async Task Find_Success()
        {
            // Arrange
            var userDevice = new UserDevice { DeviceId = DeviceId };

            _mockDeviceIdGenerator.Setup(x => x.Generate(_accessToken, DevicePns)).Returns(DeviceId);
            _mockDeviceRepository.Setup(x => x.Find(NhsLoginId, DeviceId))
                .ReturnsAsync(new RepositoryFindResult<UserDevice>.Found(new []{ userDevice }));

            // Act
            var result = await _systemUnderTest.Find(DevicePns, _accessToken);

            // Assert
            _mockDeviceIdGenerator.VerifyAll();
            _mockDeviceRepository.VerifyAll();

            var objectResult = result.Should().BeAssignableTo<SearchDeviceResult.Found>();
            objectResult.Subject.UserDevice.Should().Be(userDevice);
        }

        [TestMethod]
        public async Task Find_RepositoryDoesNotFindRecord_ReturnNotFound()
        {
            // Arrange
            _mockDeviceIdGenerator.Setup(x => x.Generate(_accessToken, DevicePns)).Returns(DeviceId);
            _mockDeviceRepository.Setup(x => x.Find(NhsLoginId, DeviceId))
                .ReturnsAsync(new RepositoryFindResult<UserDevice>.NotFound());

            // Act
            var result = await _systemUnderTest.Find(DevicePns, _accessToken);

            // Assert
            _mockDeviceIdGenerator.VerifyAll();
            _mockDeviceRepository.VerifyAll();

            result.Should().BeAssignableTo<SearchDeviceResult.NotFound>();
        }

        [TestMethod]
        public async Task Find_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockDeviceIdGenerator.Setup(x => x.Generate(_accessToken, DevicePns)).Returns(DeviceId);
            _mockDeviceRepository.Setup(x => x.Find(NhsLoginId, It.IsAny<string>()))
                .Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Find(DevicePns, _accessToken);

            // Assert
            _mockDeviceIdGenerator.VerifyAll();
            _mockDeviceRepository.VerifyAll();

            result.Should().BeAssignableTo<SearchDeviceResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Delete_Success()
        {
            // Arrange
            _mockDeviceRepository.Setup(x => x.Delete(NhsLoginId, DeviceId))
                .ReturnsAsync(new RepositoryDeleteResult<UserDevice>.Deleted());

            // Act
            var result = await _systemUnderTest.Delete(DeviceId, NhsLoginId);

            // Assert
            _mockDeviceRepository.VerifyAll();

            result.Should().BeAssignableTo<DeleteDeviceResult.Success>();
        }

        [TestMethod]
        public async Task Delete_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockDeviceRepository.Setup(x => x.Delete(NhsLoginId, DeviceId))
                .Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Delete(DeviceId, NhsLoginId);

            // Assert
            _mockDeviceRepository.VerifyAll();

            result.Should().BeAssignableTo<DeleteDeviceResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Delete_RepositoryError_ReturnsBadGateway()
        {
            // Arrange
            _mockDeviceRepository.Setup(x => x.Delete(NhsLoginId, DeviceId))
                .ReturnsAsync(new RepositoryDeleteResult<UserDevice>.RepositoryError());

            // Act
            var result = await _systemUnderTest.Delete(DeviceId, NhsLoginId);

            // Assert
            _mockDeviceRepository.VerifyAll();

            result.Should().BeAssignableTo<DeleteDeviceResult.BadGateway>();
        }
    }
}