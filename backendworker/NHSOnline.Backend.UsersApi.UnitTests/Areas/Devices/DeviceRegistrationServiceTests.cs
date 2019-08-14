using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.UsersApi.Azure;
using NHSOnline.Backend.UsersApi.Areas.Devices;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.UnitTests.Areas.Devices
{
    [TestClass]
    public class DeviceRegistrationServiceTests
    {
        private IFixture _fixture;
        private DeviceRepositoryService _systemUnderTest;
        private Mock<IUserDeviceRepository> _deviceRepository;
        private Mock<IDeviceIdGenerator> _deviceIdGenerator;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());

            _deviceRepository = new Mock<IUserDeviceRepository>();
            _deviceIdGenerator = new Mock<IDeviceIdGenerator>();

            var logger = new Mock<ILogger<DevicesController>>();
            _systemUnderTest = new DeviceRepositoryService(
                _deviceRepository.Object,
                _deviceIdGenerator.Object,
                logger.Object);
        }

        [TestMethod]
        public async Task Create_Success()
        {
            // Arrange
            var request = _fixture.Create<RegisterDeviceRequest>();
            _deviceIdGenerator.Setup(x => x.Generate(It.IsAny<string>(), request)).Returns(_fixture.Create<string>());
            _deviceRepository.Setup(x => x.Create(It.IsAny<UserDevice>())).Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Create(
                _fixture.Create<AzureRegistrationResponse>(),
                request);

            // Assert
            var objectResult = result.Should().BeAssignableTo<DeviceRepositoryResult.Created>();
            objectResult.Subject.UserDevice.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Create_RepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var request = _fixture.Create<RegisterDeviceRequest>();
            _deviceRepository.Setup(x => x.Create(It.IsAny<UserDevice>())).Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Create(
                _fixture.Create<AzureRegistrationResponse>(),
                request);

            // Assert
            result.Should().BeAssignableTo<DeviceRepositoryResult.Failure>();
        }
    }
}
