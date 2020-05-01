using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Areas.Demographics;
using NHSOnline.Backend.GpSystems.Demographics.Models;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Demographics
{
    [TestClass]
    public sealed class DemographicsControllerTests: IDisposable
    {
        private DemographicsController _systemUnderTest;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private P9UserSession _userSession;
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
            
            _mockAuditor = new Mock<IAuditor>();

            _userSession = new P9UserSession("csrfToken", new CitizenIdUserSession(), new EmisUserSession(), "im1token");

            _mockDemographicsService = new Mock<IDemographicsService>();
                
            _mockGpSystem = new Mock<IGpSystem>();
            _mockGpSystem.Setup(x => x.GetDemographicsService())
                .Returns(_mockDemographicsService.Object);
            
            _mockGpSystemFactory = new Mock<IGpSystemFactory>();
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(_mockGpSystem.Object);
            
            _visitor = new DemographicsResultVisitor(new SuccessfulDemographicsResultMapper());

            _systemUnderTest = new DemographicsController(
                new Mock<ILogger<DemographicsController>>().Object,
                _mockGpSystemFactory.Object,
                _visitor,
                _mockAuditor.Object);
        }
        
        [TestMethod]
        public async Task Get_Returns_SuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            // Arrange
            var demographicsResponse = new DemographicsResponse();
            var demographicsResult = new DemographicsResult.Success(demographicsResponse);

            _mockDemographicsService.Setup(x => x.GetDemographics(
                    It.Is<GpLinkedAccountModel>(
                        d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientGuid)))
                .Returns(Task.FromResult((DemographicsResult) demographicsResult));

            // Act
            var result = await _systemUnderTest.Get(_patientGuid, _userSession);

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
            var result = await _systemUnderTest.Get(_patientGuid, _userSession);

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
            var result = await _systemUnderTest.Get(_patientGuid, _userSession);

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
            var result = await _systemUnderTest.Get(_patientGuid, _userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            _mockDemographicsService.Verify();
            
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, "Error viewing Demographics: internal server error" ));
        }

        public void Dispose() => _systemUnderTest?.Dispose();
    }
}