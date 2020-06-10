using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.PatientRecord;
using Moq;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Strategies.ResponseSuccessOutcome;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.PatientRecord
{
    [TestClass]
    public class EmisPatientRecordServiceTests
    {
        private EmisPatientRecordService _systemUnderTest;
        private Mock<IEmisClient> _emisClient;
        private Mock<IGetPatientDocumentTaskChecker> _getPatientDocumentTaskChecker;
        private EmisUserSession _emisUserSession;
        private Guid _patientId;
        private GpLinkedAccountModel _gpLinkedAccountModel;
        private IFixture _fixture;
        private List<HttpStatusCode> _sampleSuccessStatusCodes;

        private const string DocumentId = "1";
        private const string DocumentType = "image/jpeg";
        private const string DocumentName = "document";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _emisUserSession = _fixture.Create<EmisUserSession>();
            _patientId = _emisUserSession.Id;
            _gpLinkedAccountModel = new GpLinkedAccountModel(_emisUserSession, _patientId);
            _emisClient = _fixture.Freeze<Mock<IEmisClient>>();
            _getPatientDocumentTaskChecker = _fixture.Freeze<Mock<IGetPatientDocumentTaskChecker>>();
            _sampleSuccessStatusCodes = new List<HttpStatusCode>
            {
                HttpStatusCode.OK
            };

            _systemUnderTest = _fixture.Create<EmisPatientRecordService>();
        }

        [TestMethod]
        public async Task GetMyRecord_ReturnsSuccessResponseForHappyPath_WhenSuccessfulResponseFromEmis()
        {
            var allergiesResponse = _fixture.Create<MedicationRootObject>();
            var medicationsResponse = _fixture.Create<MedicationRootObject>();
            var immunisationsResponse = _fixture.Create<MedicationRootObject>();
            var testResultsResponse = _fixture.Create<MedicationRootObject>();
            var problemsResponse = _fixture.Create<MedicationRootObject>();
            var consultationsResponse = _fixture.Create<MedicationRootObject>();
            var documentsResponse = _fixture.Create<MedicationRootObject>();

            _emisClient.Setup(x => x.MedicalRecordGet(
                    It.Is<EmisRequestParameters>(
                        e => e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal) &&
                             e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                             e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal)),
                    RecordType.Medication))
                .Returns(Task.FromResult(
                    new EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK, RequestsForSuccessOutcome.MedicalRecordGet, _sampleSuccessStatusCodes)
                    {
                        Body = medicationsResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            _emisClient.Setup(x => x.MedicalRecordGet(It.Is<EmisRequestParameters>(
                    e => e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal) &&
                         e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                         e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal))
                    , RecordType.Allergies))
                .Returns(Task.FromResult(
                    new EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK, RequestsForSuccessOutcome.MedicalRecordGet, _sampleSuccessStatusCodes)
                    {
                        Body = allergiesResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            _emisClient.Setup(x => x.MedicalRecordGet(It.Is<EmisRequestParameters>(
                    e => e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal) &&
                         e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                         e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal)),
                    RecordType.Immunisations))
                .Returns(Task.FromResult(
                    new EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK, RequestsForSuccessOutcome.MedicalRecordGet, _sampleSuccessStatusCodes)
                    {
                        Body = immunisationsResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            _emisClient.Setup(x => x.MedicalRecordGet(It.Is<EmisRequestParameters>(
                    e => e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal) &&
                         e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                         e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal)),
                    RecordType.TestResults))
                .Returns(Task.FromResult(
                    new EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK, RequestsForSuccessOutcome.MedicalRecordGet, _sampleSuccessStatusCodes)
                    {
                        Body = testResultsResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            _emisClient.Setup(x => x.MedicalRecordGet(It.Is<EmisRequestParameters>(
                    e => e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal) &&
                         e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                         e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal)),
                    RecordType.Problems))
                .Returns(Task.FromResult(
                    new EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK, RequestsForSuccessOutcome.MedicalRecordGet, _sampleSuccessStatusCodes)
                    {
                        Body = problemsResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            _emisClient.Setup(x => x.MedicalRecordGet(It.Is<EmisRequestParameters>(
                    e => e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal) &&
                         e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                         e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal)),
                    RecordType.Consultations))
                .Returns(Task.FromResult(
                    new EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK, RequestsForSuccessOutcome.MedicalRecordGet, _sampleSuccessStatusCodes)
                    {
                        Body = consultationsResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            _emisClient.Setup(x => x.MedicalRecordGet(It.Is<EmisRequestParameters>(
                    e => e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal) &&
                         e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                         e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal)),
                    RecordType.Documents))
                .Returns(Task.FromResult(
                    new EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK, RequestsForSuccessOutcome.MedicalRecordGet, _sampleSuccessStatusCodes)
                    {
                        Body = documentsResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            // Act
            var result = await _systemUnderTest.GetMyRecord(new GpLinkedAccountModel(_emisUserSession));

            // Assert
            _emisClient.Verify(x => x.MedicalRecordGet(It.Is<EmisRequestParameters>(
                e => e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal) &&
                     e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                     e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal)),
                RecordType.Allergies));

            result.Should().BeAssignableTo<GetMyRecordResult.Success>()
                .Subject.Response.Should().NotBeNull();
        }

        [TestMethod]
        public async Task GetPatientDocument_WhenMappedPatientDocumentIsValid_ReturnsSuccessful()
        {
            var mappedPatientDocument = _fixture.Create<PatientDocument>();

            SetupGetDocumentForViewing(mappedPatientDocument);

            var result = await _systemUnderTest.GetPatientDocument(_gpLinkedAccountModel, DocumentId, DocumentType, DocumentName);

            _emisClient.Verify();
            _getPatientDocumentTaskChecker.Verify();

            var value = result.Should().BeAssignableTo<GetPatientDocumentResult.Success>();
            value.Subject.Response.Should().NotBeNull();
            value.Subject.Response.Should().Be(mappedPatientDocument);
        }

        [TestMethod]
        public async Task GetPatientDocument_WhenMappedPatientDocumentHasErrored_ReturnsBadGateway()
        {
            var mappedPatientDocument = _fixture.Create<PatientDocument>();
            mappedPatientDocument.HasErrored = true;

            SetupGetDocumentForViewing(mappedPatientDocument);

            var result = await _systemUnderTest.GetPatientDocument(_gpLinkedAccountModel, DocumentId, DocumentType, DocumentName);

            _emisClient.Verify();
            _getPatientDocumentTaskChecker.Verify();

            result.Should().BeAssignableTo<GetPatientDocumentResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetPatientDocumentForDownload_WhenMappedFileContentResultIsValid_ReturnsSuccessful()
        {
            var mappedFileContentResult = new FileContentResult(_fixture.Create<byte[]>(), "img/jpeg");

            SetupGetDocumentForDownload(mappedFileContentResult);

            var result = await _systemUnderTest.GetPatientDocumentForDownload(_gpLinkedAccountModel, DocumentId, DocumentType, DocumentName);

            _emisClient.Verify();
            _getPatientDocumentTaskChecker.Verify();

            var value = result.Should().BeAssignableTo<GetPatientDocumentDownloadResult.Success>();
            value.Subject.Response.Should().NotBeNull();
            value.Subject.Response.Should().Be(mappedFileContentResult);
        }

        [TestMethod]
        public async Task GetPatientDocumentForDownload_WhenMappedFileContentResultIsNull_ReturnsBadGateway()
        {
            SetupGetDocumentForDownload(null);

            var result = await _systemUnderTest.GetPatientDocumentForDownload(_gpLinkedAccountModel, DocumentId, DocumentType, DocumentName);

            _emisClient.Verify();
            _getPatientDocumentTaskChecker.Verify();

            result.Should().BeAssignableTo<GetPatientDocumentDownloadResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetPatientDocument_ForViewOrDownload_WhenMedicalDocumentGetThrowsHttpException_ReturnsBadGateway()
        {
            SetupEmisClientMedicalDocumentGetWithException(new HttpRequestException());

            var viewResult = await _systemUnderTest.GetPatientDocument(_gpLinkedAccountModel, DocumentId, DocumentType, DocumentName);
            var downloadResult = await _systemUnderTest.GetPatientDocumentForDownload(_gpLinkedAccountModel, DocumentId, DocumentType, DocumentName);

            _emisClient.Verify();

            viewResult.Should().BeAssignableTo<GetPatientDocumentResult.BadGateway>();
            downloadResult.Should().BeAssignableTo<GetPatientDocumentDownloadResult.BadGateway>();
        }

        private void SetupGetDocumentForViewing(PatientDocument mappedPatientDocument)
        {
            var individualDocumentResponse =
                new EmisApiObjectResponse<IndividualDocument>(
                    HttpStatusCode.OK,
                    RequestsForSuccessOutcome.MedicalDocumentGet,
                    _sampleSuccessStatusCodes)
                {
                    Body = _fixture.Create<IndividualDocument>(),
                    ExceptionErrorResponse = null,
                    ErrorResponseBadRequest = null
                };

            SetupEmisClientMedicalDocumentGetWithResponse(individualDocumentResponse);

            _getPatientDocumentTaskChecker
                .Setup(c => c.CheckForViewing(individualDocumentResponse, DocumentType, DocumentName))
                .Returns(mappedPatientDocument)
                .Verifiable();
        }

        private void SetupGetDocumentForDownload(FileContentResult mappedFileContentResult)
        {
            var individualDocumentResponse =
                new EmisApiObjectResponse<IndividualDocument>(
                    HttpStatusCode.OK,
                    RequestsForSuccessOutcome.MedicalDocumentGet,
                    _sampleSuccessStatusCodes)
                {
                    Body = _fixture.Create<IndividualDocument>(),
                    ExceptionErrorResponse = null,
                    ErrorResponseBadRequest = null
                };

            SetupEmisClientMedicalDocumentGetWithResponse(individualDocumentResponse);

            _getPatientDocumentTaskChecker
                .Setup(c => c.CheckForDownload(individualDocumentResponse, DocumentType, DocumentName))
                .Returns(mappedFileContentResult)
                .Verifiable();
        }

        private void SetupEmisClientMedicalDocumentGetWithResponse(EmisApiObjectResponse<IndividualDocument> response)
        {
            _emisClient
                .Setup(x => x.MedicalDocumentGet(_emisUserSession.UserPatientLinkToken, _emisUserSession.SessionId, DocumentId, _emisUserSession.EndUserSessionId))
                .ReturnsAsync(response)
                .Verifiable();
        }

        private void SetupEmisClientMedicalDocumentGetWithException<T>(T e) where T: Exception
        {
            _emisClient
                .Setup(x => x.MedicalDocumentGet(_emisUserSession.UserPatientLinkToken, _emisUserSession.SessionId, DocumentId, _emisUserSession.EndUserSessionId))
                .Throws(e)
                .Verifiable();
        }
    }
}