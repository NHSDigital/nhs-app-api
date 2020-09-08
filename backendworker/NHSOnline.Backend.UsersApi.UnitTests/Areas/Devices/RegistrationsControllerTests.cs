using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.UsersApi.Areas.Devices;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Registrations;

namespace NHSOnline.Backend.UsersApi.UnitTests.Areas.Devices
{
    [TestClass]
    public sealed class RegistrationsControllerTests : IDisposable
    {
        private RegistrationsController _systemUnderTest;
        private Mock<IRegistrationService> _mockRegistrationService;
        private const string NhsLoginId = "NhsLoginId";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRegistrationService = new Mock<IRegistrationService>(MockBehavior.Strict);

            _systemUnderTest = new RegistrationsController(_mockRegistrationService.Object,
                new Mock<ILogger<RegistrationsController>>().Object);
        }

        [TestMethod]
        public async Task Get_FoundRegistrations_Returns200WithRegistrationIds()
        {
            // Arrange
            var registrationIds = new[] { "Registration1", "Registration2" };

            _mockRegistrationService
                .Setup(x => x.Find(NhsLoginId))
                .ReturnsAsync(new FindRegistrationsResult.Found(registrationIds));

            // Act
            var result = await _systemUnderTest.Get(NhsLoginId);

            // Assert
            _mockRegistrationService.VerifyAll();

            var subject = result.Should().BeOfType<OkObjectResult>().Subject;
            subject.Value.Should().BeEquivalentTo(registrationIds);
        }

        [TestMethod]
        public async Task Get_NotFoundRegistrations_Returns404NotFound()
        {
            // Arrange
            _mockRegistrationService
                .Setup(x => x.Find(NhsLoginId))
                .ReturnsAsync(new FindRegistrationsResult.NotFound());

            // Act
            var result = await _systemUnderTest.Get(NhsLoginId);

            // Assert
            _mockRegistrationService.VerifyAll();

            var subject = result.Should().BeOfType<StatusCodeResult>().Subject;
            subject.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [TestMethod]
        public async Task Get_RegistrationServiceInternalServerErrorResult_Returns500InternalServerError()
        {
            // Arrange
            _mockRegistrationService
                .Setup(x => x.Find(NhsLoginId))
                .ReturnsAsync(new FindRegistrationsResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Get(NhsLoginId);

            // Assert
            _mockRegistrationService.VerifyAll();

            var subject = result.Should().BeOfType<StatusCodeResult>().Subject;
            subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Get_RegistrationServiceBadGatewayResult_Returns502BadGateway()
        {
            // Arrange
            _mockRegistrationService
                .Setup(x => x.Find(NhsLoginId))
                .ReturnsAsync(new FindRegistrationsResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Get(NhsLoginId);

            // Assert
            _mockRegistrationService.VerifyAll();

            var subject = result.Should().BeOfType<StatusCodeResult>().Subject;
            subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}