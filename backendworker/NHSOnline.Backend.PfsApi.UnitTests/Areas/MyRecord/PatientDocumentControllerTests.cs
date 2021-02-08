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
    public sealed class PatientDocumentControllerTests: IDisposable
    {
        private PatientDocumentController _systemUnderTest;

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
            _mockPatientRecordService = new Mock<IPatientRecordService>();

            var mockGpSystem = new Mock<IGpSystem>();
            var mockGpSystemFactory = new Mock<IGpSystemFactory>();

            _userSession = new P9UserSession("csrfToken", "nhsNumber", new CitizenIdUserSession(), new EmisUserSession(), "im1token");
            _patientId = Guid.NewGuid();

            var httpContextResponse = new Mock<HttpResponse>();
            httpContextResponse.Setup(x => x.Headers).Returns(new HeaderDictionary());

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(x => x.Response).Returns(httpContextResponse.Object);

            _mockAuditor = new Mock<IAuditor>();
            _systemUnderTest = new PatientDocumentController(
                new Mock<ILogger<PatientDocumentController>>().Object,
                mockGpSystemFactory.Object,
                _mockAuditor.Object);

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
            var successResponse = new PatientDocument();
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
            MockPreOpAuditor(ViewDocumentAuditTypeRequest, ViewDocumentAuditMessageRequest);
            MockPostOpAuditor(ViewDocumentAuditTypeResponse, "Successfully retrieved patient document for viewing");

            var documentInfo = new DocumentInfo
            {
                Type = DocumentType,
                Name = DocumentName
            };

            // Act
            var result = await _systemUnderTest.GetPatientDocument(_patientId, DocumentId, documentInfo, _userSession);

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
            MockPreOpAuditor(ViewDocumentAuditTypeRequest, ViewDocumentAuditMessageRequest);
            MockPostOpAuditor(ViewDocumentAuditTypeResponse, "Error retrieving patient document for viewing: Bad Gateway");

            var documentInfo = new DocumentInfo
            {
                Type = DocumentType,
                Name = DocumentName
            };

            // Act
            var result = await _systemUnderTest.GetPatientDocument(_patientId, DocumentId, documentInfo, _userSession);

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
            var fileContents = Array.Empty<byte>();
            var fileName = "filename.ext";

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
            MockPreOpAuditor(DownloadDocumentAuditTypeRequest, DownloadDocumentAuditMessageRequest);
            MockPostOpAuditor(DownloadDocumentAuditTypeResponse, "Successfully retrieved patient document for downloading");

            var documentInfo = new DocumentInfo
            {
                Type = DocumentType,
                Name = DocumentName
            };

            // Act
            var result = await _systemUnderTest.GetPatientDocumentForDownload(_patientId, DocumentId, documentInfo, _userSession);

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
            MockPreOpAuditor(DownloadDocumentAuditTypeRequest, DownloadDocumentAuditMessageRequest);
            MockPostOpAuditor(DownloadDocumentAuditTypeResponse, "Error retrieving patient document for downloading: Bad Gateway");

            var documentInfo = new DocumentInfo
            {
                Type = DocumentType,
                Name = DocumentName
            };

            // Act
            var result = await _systemUnderTest.GetPatientDocumentForDownload(_patientId, DocumentId, documentInfo, _userSession);

            // Assert
            _mockPatientRecordService.Verify();
            VerifyMockAuditor();

            result
                .Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        private void MockPreOpAuditor(string operation, string details)
        {
            _mockAuditor
                .Setup(x => x.PreOperationAudit(operation, details))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        private void MockPostOpAuditor(string operation, string details)
        {
            _mockAuditor
                .Setup(x => x.PostOperationAudit(operation, details))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        private void VerifyMockAuditor()
        {
            _mockAuditor.Verify();
            _mockAuditor.Verify(
                a => a.PreOperationAudit(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Exactly(1));
            _mockAuditor.Verify(
                a => a.PostOperationAudit(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Exactly(1));
        }

        public void Dispose() => _systemUnderTest?.Dispose();
    }
}
