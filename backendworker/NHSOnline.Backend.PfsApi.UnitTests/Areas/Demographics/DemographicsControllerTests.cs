using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Areas.Demographics;
using NHSOnline.Backend.GpSystems.Demographics.Models;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Demographics
{
    [TestClass]
    public class DemographicsControllerTests
    {
        private DemographicsController _systemUnderTest;
        private IFixture _fixture;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private UserSession _userSession;
        private IDemographicsResultVisitor<IActionResult> _visitor;
        private Mock<IAuditor> _mockAuditor;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IDemographicsService> _mockDemographicsService;

        private const string RequestAuditType = "Demographics_Get_Request";
        private const string ResponseAuditType = "Demographics_Get_Response";

        private const string RequestAuditMessage = "Attempting to view Demographics";

        private  Guid _patientGuid;

        
        [TestInitialize]
        public void TestInitialize()
        {
            _patientGuid = Guid.NewGuid();
            
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();

            _userSession = _fixture.Create<UserSession>();
            
            _mockDemographicsService = _fixture.Freeze<Mock<IDemographicsService>>();
                
            _mockGpSystem = _fixture.Freeze<Mock<IGpSystem>>();
            _mockGpSystem.Setup(x => x.GetDemographicsService())
                .Returns(_mockDemographicsService.Object);
            
            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(_mockGpSystem.Object);
            
            var httpContextItems = new Dictionary<object, object>
            {
                { Constants.HttpContextItems.UserSession, _userSession }
            };

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(x => x.Items).Returns(httpContextItems);

            _visitor = new DemographicsResultVisitor(new SuccessfulDemographicsResultMapper());
            _fixture.Inject(_visitor);

            _systemUnderTest = _fixture.Create<DemographicsController>();

            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }
        
        [TestMethod]
        public async Task Get_Returns_SuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            // Arrange
            var demographicsResponse = _fixture.Create<DemographicsResponse>();
            var demographicsResult = new DemographicsResult.Success(demographicsResponse);

            _mockDemographicsService.Setup(x => x.GetDemographics(
                    It.Is<GpLinkedAccountModel>(
                        d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientGuid)))
                .Returns(Task.FromResult((DemographicsResult) demographicsResult));

            // Act
            var result = await _systemUnderTest.Get(_patientGuid);

            // Assert
            _mockDemographicsService.Verify();
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<SuccessfulDemographicsResult>();
            
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, "Demographics successfully viewed" ));
        }
        
        [TestMethod]
        public async Task Get_ReturnsStatus403Forbidden_WhenPatientDoesNotHaveAccessToData()
        {
            // Arrange
            var demographicsResult = new DemographicsResult.Forbidden();

            _mockDemographicsService
                .Setup(x => x.GetDemographics(
                    It.Is<GpLinkedAccountModel>(
                        d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientGuid)))
                .Returns(Task.FromResult((DemographicsResult) demographicsResult));

            // Act
            var result = await _systemUnderTest.Get(_patientGuid);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
            _mockDemographicsService.Verify();
            
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, "Error viewing Demographics: patient does not have access to data" ));
        }
        
        [TestMethod]
        public async Task Get_ReturnsStatus502BadGateway_WhenServiceReturnsBadGateway()
        {
            // Arrange
            var demographicsResult = new DemographicsResult.BadGateway();
            _mockDemographicsService.Setup(x => x.GetDemographics(
                It.Is<GpLinkedAccountModel>(
                    d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientGuid)))
                .Returns(Task.FromResult((DemographicsResult) demographicsResult));

            // Act
            var result = await _systemUnderTest.Get(_patientGuid);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            _mockDemographicsService.Verify();
            
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, "Error viewing Demographics: bad gateway" ));
        }
        
        [TestMethod]
        public async Task Get_ReturnsStatus500InternalServerError_WhenServiceReturnsInternalServerError()
        {
            // Arrange
            var demographicsResult = new DemographicsResult.InternalServerError();

            _mockDemographicsService.Setup(x => x.GetDemographics(
                It.Is<GpLinkedAccountModel>(
                    d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientGuid)))
                .Returns(Task.FromResult((DemographicsResult) demographicsResult));

            // Act
            var result = await _systemUnderTest.Get(_patientGuid);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            _mockDemographicsService.Verify();
            
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, "Error viewing Demographics: internal server error" ));
        }
    }
}