using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
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
    public class PatientDocumentControllerTests
    {
        private PatientDocumentController _systemUnderTest;
        private IFixture _fixture;

        private Mock<IPatientRecordService> _mockPatientRecordService;
        private Mock<IAuditor> _mockAuditor;

        private P9UserSession _userSession;
        private Guid _patientId;

        private const string DocumentId = "1";
        private const string DocumentType = "jpg";
        private const string DocumentName = "document";
        private const string DocumentMimeType = "image/jpg";

        private const string ViewDocumentAuditTypeRequest = "Document_View_Request";
        private const string ViewDocumentAuditTypeResponse = "Document_View_Response";
        private const string ViewDocumentAuditMessageRequest = "Viewing patient document";

        private const string DownloadDocumentAuditTypeRequest = "Document_Download_Request";
        private const string DownloadDocumentAuditTypeResponse = "Document_Download_Response";
        private const string DownloadDocumentAuditMessageRequest = "Downloading patient document";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _mockPatientRecordService = _fixture.Freeze<Mock<IPatientRecordService>>();

            var mockGpSystem = _fixture.Freeze<Mock<IGpSystem>>();
            var mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();

            _userSession = _fixture.Create<P9UserSession>();
            _patientId = Guid.NewGuid();

            var httpContextItems = new Dictionary<object, object>
            {
                { Constants.HttpContextItems.UserSession, _userSession }
            };

            var httpContextResponse = new DefaultHttpResponse(new DefaultHttpContext());

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(x => x.Items).Returns(httpContextItems);
            httpContextMock.SetupGet(x => x.Response).Returns(httpContextResponse);

            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();
            _systemUnderTest = _fixture.Create<PatientDocumentController>();

            mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPatientRecordService())
                .Returns(_mockPatientRecordService.Object);

            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object,
            };
        }

        [TestMethod]
        public async Task GetPatientDocument_WhenServiceReturnsSuccessResult_ReturnsOkPatientDocumentResult()
        {
            // Arrange
            var successResponse = _fixture.Create<PatientDocument>();
            var successResult = new GetPatientDocumentResult.Success(successResponse);


            Func<GpLinkedAccountModel, bool> check = d =>
            {
                return d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientId;
            };

            _mockPatientRecordService
                .Setup(x => x.GetPatientDocument(
                    It.Is<GpLinkedAccountModel>(d => check(d)), DocumentId, DocumentType, DocumentName))
                .ReturnsAsync(successResult)
                .Verifiable();
            MockAuditor(ViewDocumentAuditTypeRequest, ViewDocumentAuditMessageRequest);
            MockAuditor(ViewDocumentAuditTypeResponse, "Successfully retrieved patient document for viewing");

            var documentInfo = new DocumentInfo
            {
                Type = DocumentType,
                Name = DocumentName
            };

            // Act
            var result = await _systemUnderTest.GetPatientDocument(_patientId, DocumentId, documentInfo);

            // Assert
            _mockPatientRecordService.Verify();
            VerifyMockAuditor();

            result
                .Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().Be(successResponse);
        }

        [TestMethod]
        public async Task GetPatientDocument_WhenServiceReturnsBadGateway_ReturnsBadGateway()
        {
            // Arrange
            var badResult = new GetPatientDocumentResult.BadGateway();

            _mockPatientRecordService
                .Setup(x => x.GetPatientDocument(It.Is<GpLinkedAccountModel>(
                        d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientId),
                        DocumentId, DocumentType, DocumentName))
                .ReturnsAsync(badResult)
                .Verifiable();
            MockAuditor(ViewDocumentAuditTypeRequest, ViewDocumentAuditMessageRequest);
            MockAuditor(ViewDocumentAuditTypeResponse, "Error retrieving patient document for viewing: Bad Gateway");

            var documentInfo = new DocumentInfo
            {
                Type = DocumentType,
                Name = DocumentName
            };

            // Act
            var result = await _systemUnderTest.GetPatientDocument(_patientId, DocumentId, documentInfo);

            // Assert
            _mockPatientRecordService.Verify();
            VerifyMockAuditor();

            result
                .Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task GetPatientDocumentForDownload_WhenServiceReturnsSuccessResult_ReturnsFileContentResult()
        {
            // Arrange
            var fileContents = _fixture.Create<byte[]>();
            var fileName = _fixture.Create<string>();

            var successResponse = new FileContentResult(fileContents, DocumentMimeType)
            {
                FileDownloadName = fileName
            };
            var successResult = new GetPatientDocumentDownloadResult.Success(successResponse);

            _mockPatientRecordService
                .Setup(x => x.GetPatientDocumentForDownload(It.Is<GpLinkedAccountModel>(
                    d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientId),
                    DocumentId, DocumentType, DocumentName))
                .ReturnsAsync(successResult)
                .Verifiable();
            MockAuditor(DownloadDocumentAuditTypeRequest, DownloadDocumentAuditMessageRequest);
            MockAuditor(DownloadDocumentAuditTypeResponse, "Successfully retrieved patient document for downloading");

            var documentInfo = new DocumentInfo
            {
                Type = DocumentType,
                Name = DocumentName
            };

            // Act
            var result = await _systemUnderTest.GetPatientDocumentForDownload(_patientId, DocumentId, documentInfo);

            // Assert
            _mockPatientRecordService.Verify();
            VerifyMockAuditor();

            result
                .Should().BeAssignableTo<FileContentResult>()
                .Subject.Should().Be(successResponse);
        }

        [TestMethod]
        public async Task GetPatientDocumentForDownload_WhenServiceReturnsBadGateway_ReturnsBadGateway()
        {
            // Arrange
            var badResult = new GetPatientDocumentDownloadResult.BadGateway();

            _mockPatientRecordService
                .Setup(x => x.GetPatientDocumentForDownload(It.Is<GpLinkedAccountModel>(
                    d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientId),
                    DocumentId, DocumentType, DocumentName))
                .ReturnsAsync(badResult)
                .Verifiable();
            MockAuditor(DownloadDocumentAuditTypeRequest, DownloadDocumentAuditMessageRequest);
            MockAuditor(DownloadDocumentAuditTypeResponse, "Error retrieving patient document for downloading: Bad Gateway");

            var documentInfo = new DocumentInfo
            {
                Type = DocumentType,
                Name = DocumentName
            };

            // Act
            var result = await _systemUnderTest.GetPatientDocumentForDownload(_patientId, DocumentId, documentInfo);

            // Assert
            _mockPatientRecordService.Verify();
            VerifyMockAuditor();

            result
                .Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        private void MockAuditor(string operation, string details)
        {
            _mockAuditor
                .Setup(x => x.Audit(operation, details))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        private void VerifyMockAuditor()
        {
            _mockAuditor.Verify();
            _mockAuditor.Verify(
                a => a.Audit(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Exactly(2));
        }
    }
}