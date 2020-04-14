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
using NHSOnline.Backend.PfsApi.Areas.MyRecord;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.MyRecord
{
    [TestClass]
    public class TestResultControllerTests
    {
        private DetailedTestResultController _systemUnderTest;
        private IFixture _fixture;
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
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();

            _userSession = _fixture.Create<P9UserSession>();
            
            _mockPatientRecordService = new Mock<IPatientRecordService>();
                
            _mockGpSystem = _fixture.Freeze<Mock<IGpSystem>>();
            _mockGpSystem.Setup(x => x.GetPatientRecordService())
                .Returns(_mockPatientRecordService.Object);
            
            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(_mockGpSystem.Object);
            
            var httpContextItems = new Dictionary<object, object>
            {
                { Constants.HttpContextItems.UserSession, _userSession }
            };

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(x => x.Items).Returns(httpContextItems);

            _fixture.Freeze<IDetailedTestResultVisitor<IActionResult>>();

            _systemUnderTest = _fixture.Create<DetailedTestResultController>();

            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }
        
        [TestMethod]
        public async Task GetTestResult_Returns_SuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            // Arrange
            var testResultResponse = _fixture.Create<TestResultResponse>();
            var testResult = new GetDetailedTestResult.Success(testResultResponse);
            
            _mockPatientRecordService.Setup(x => x.GetDetailedTestResult(It.Is<GpLinkedAccountModel>(
                    d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientGuid), TestResultId))
                .Returns(Task.FromResult((GetDetailedTestResult) testResult));

            // Act
            var result = await _systemUnderTest.GetTestResult(_patientGuid, TestResultId);

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
            var result = await _systemUnderTest.GetTestResult(_patientGuid, TestResultId);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            _mockPatientRecordService.Verify();
            
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, "Error viewing test result: bad gateway" ));
        }
    }
}