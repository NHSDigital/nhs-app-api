using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.PfsApi.Areas.Session;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Session
{
    [TestClass]
    public class SessionExtendControllerPostTests
    {
        private SessionExtendController _systemUnderTest;
        private IFixture _fixture;
        private Mock<IGpSystem> _mockGpSystem;
        private P9UserSession _userSession;
        private Guid _patientGuid;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<ISessionExtendService> _mockSessionExtendService;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());
            
            _fixture.Customize<P9UserSession>(c => c
                .With(u => u.GpUserSession, _fixture.Create<EmisUserSession>()));
            
            _userSession = _fixture.Create<P9UserSession>();
            _patientGuid = _fixture.Create<Guid>();

            _mockSessionExtendService = _fixture.Freeze<Mock<ISessionExtendService>>();

            _mockGpSystem = _fixture.Freeze<Mock<IGpSystem>>();
            _mockGpSystem
                .Setup(x => x.GetSessionExtendService())
                .Returns(_mockSessionExtendService.Object);

            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _mockGpSystemFactory
                .Setup(x => x.CreateGpSystem(Supplier.Emis))
                .Returns(_mockGpSystem.Object);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.Items[Constants.HttpContextItems.UserSession]).Returns(_userSession);

            _systemUnderTest = _fixture.Create<SessionExtendController>();

            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
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
            var result = await _systemUnderTest.Post(_patientGuid);

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
            var result = await _systemUnderTest.Post(_patientGuid);

            // Arrange
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            _mockSessionExtendService.Verify();
        }
    }
}