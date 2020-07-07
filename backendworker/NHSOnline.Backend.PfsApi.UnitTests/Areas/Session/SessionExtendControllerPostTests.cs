using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Session
{
    [TestClass]
    public sealed class SessionExtendControllerPostTests: IDisposable
    {
        private SessionExtendController _systemUnderTest;
        private Mock<IGpSystem> _mockGpSystem;
        private P9UserSession _userSession;
        private Guid _patientGuid;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<ISessionExtendService> _mockSessionExtendService;

        [TestInitialize]
        public void TestInitialize()
        {
            _userSession = new P9UserSession("csrfToken", "nhsNumber", new CitizenIdUserSession(), new EmisUserSession(), "im1token");

            _patientGuid = Guid.NewGuid();

            _mockSessionExtendService = new Mock<ISessionExtendService>();

            _mockGpSystem = new Mock<IGpSystem>();
            _mockGpSystem
                .Setup(x => x.GetSessionExtendService())
                .Returns(_mockSessionExtendService.Object);

            _mockGpSystemFactory = new Mock<IGpSystemFactory>();
            _mockGpSystemFactory
                .Setup(x => x.CreateGpSystem(Supplier.Emis))
                .Returns(_mockGpSystem.Object);

            _systemUnderTest = new SessionExtendController(
                _mockGpSystemFactory.Object,
                new Mock<ILogger<SessionExtendController>>().Object);
        }

        [TestMethod]
        public async Task Post_WhenServiceReturnsSuccessfully_ReturnsSuccessResponse()
        {
            // Arrange
            var extendedResult = (SessionExtendResult)new SessionExtendResult.Success();

            _mockSessionExtendService
                .Setup(x => x.Extend(It.Is<GpLinkedAccountModel>(
                    d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientGuid)))
                .Returns(Task.FromResult(extendedResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(_patientGuid, _userSession);

            // Assert
            result.Should().BeEquivalentTo(new StatusCodeResult(StatusCodes.Status200OK));
            _mockSessionExtendService.Verify();
        }

        [TestMethod]
        public async Task Post_WhenServiceReturnsBadGateway_ReturnsBadGateway()
        {
            // Arrange
            var extendedResult = (SessionExtendResult)new SessionExtendResult.BadGateway();

            _mockSessionExtendService
                .Setup(x => x.Extend(It.Is<GpLinkedAccountModel>(
                    d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientGuid)))
                .Returns(Task.FromResult(extendedResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(_patientGuid, _userSession);

            // Arrange
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            _mockSessionExtendService.Verify();
        }

        [TestMethod]
        public async Task Post_P5UserSession_ReturnsSuccessResponse()
        {
            // Arrange
            var userSession = new P5UserSession("csrf", new CitizenIdUserSession());

            // Act
            var result = await _systemUnderTest.Post(_patientGuid, userSession);

            // Assert
            result.Should().BeEquivalentTo(new StatusCodeResult(StatusCodes.Status200OK));
            _mockSessionExtendService.Verify();
        }

        public void Dispose() => _systemUnderTest?.Dispose();
    }
}