using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Areas.MyRecord;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.MyRecord
{
    [TestClass]
    public sealed class TestResultControllerTests: IDisposable
    {
        private DetailedTestResultController _systemUnderTest;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private P9UserSession _userSession;
        private Mock<IAuditor> _mockAuditor;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IPatientRecordService> _mockPatientRecordService;
        
        private const string RequestAuditType = "TestResult_Get_Request";
        private const string ResponseAuditType = "TestResult_Get_Response";
        private const string RequestAuditMessage = "Attempting to view test result";
        private const string TestResultId = "testId";
        private readonly Guid _patientGuid = Guid.NewGuid();
        
        [TestInitialize]
        public void TestInitialize()
        {
            _mockAuditor = new Mock<IAuditor>();

            _userSession = new P9UserSession("csrfToken", new CitizenIdUserSession(), new EmisUserSession(), "im1token");

            _mockPatientRecordService = new Mock<IPatientRecordService>();
                
            _mockGpSystem = new Mock<IGpSystem>();
            _mockGpSystem.Setup(x => x.GetPatientRecordService())
                .Returns(_mockPatientRecordService.Object);
            
            _mockGpSystemFactory = new Mock<IGpSystemFactory>();
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(_mockGpSystem.Object);
            
            _systemUnderTest = new DetailedTestResultController(
                new Mock<ILogger<DetailedTestResultController>>().Object,
                _mockGpSystemFactory.Object,
                _mockAuditor.Object);
        }
        
        [TestMethod]
        public async Task GetTestResult_Returns_SuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            // Arrange
            var testResultResponse = new TestResultResponse();
            var testResult = new GetDetailedTestResult.Success(testResultResponse);
            
            _mockPatientRecordService.Setup(x => x.GetDetailedTestResult(It.Is<GpLinkedAccountModel>(
                    d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientGuid), TestResultId))
                .Returns(Task.FromResult((GetDetailedTestResult) testResult));

            // Act
            var result = await _systemUnderTest.GetTestResult(_patientGuid, TestResultId, _userSession);

            // Assert
            _mockPatientRecordService.Verify();
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<GetDetailedTestResult.Success>();
            
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, "Test result successfully viewed" ));
        }
        
        [TestMethod]
        public async Task GetTestResult_ReturnsStatus502BadGateway_WhenServiceReturnsBadGateway()
        {
            // Arrange
            var testResult = new GetDetailedTestResult.BadGateway();

            _mockPatientRecordService.Setup(x => x.GetDetailedTestResult(It.Is<GpLinkedAccountModel>(
                    d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientGuid), TestResultId))
                .Returns(Task.FromResult((GetDetailedTestResult) testResult));

            // Act
            var result = await _systemUnderTest.GetTestResult(_patientGuid, TestResultId, _userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            _mockPatientRecordService.Verify();
            
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, "Error viewing test result: bad gateway" ));
        }

        public void Dispose() => _systemUnderTest?.Dispose();
    }
}