using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.PatientRecord;
using Moq;
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
        private EmisUserSession _emisUserSession;
        private Guid _patientId;
        private GpLinkedAccountModel _gpLinkedAccountModel;
        private IFixture _fixture;
        private List<HttpStatusCode> _sampleSuccessStatusCodes;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _emisUserSession = _fixture.Create<EmisUserSession>();
            _patientId = _emisUserSession.Id;
            _gpLinkedAccountModel = new GpLinkedAccountModel(_emisUserSession, _patientId);
            _emisClient = _fixture.Freeze<Mock<IEmisClient>>();
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
                    new EmisClient.EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK, RequestsForSuccessOutcome.MedicalRecordGet, _sampleSuccessStatusCodes)
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
                    new EmisClient.EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK, RequestsForSuccessOutcome.MedicalRecordGet, _sampleSuccessStatusCodes)
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
                    new EmisClient.EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK, RequestsForSuccessOutcome.MedicalRecordGet, _sampleSuccessStatusCodes)
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
                    new EmisClient.EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK, RequestsForSuccessOutcome.MedicalRecordGet, _sampleSuccessStatusCodes)
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
                    new EmisClient.EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK, RequestsForSuccessOutcome.MedicalRecordGet, _sampleSuccessStatusCodes)
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
                    new EmisClient.EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK, RequestsForSuccessOutcome.MedicalRecordGet, _sampleSuccessStatusCodes)
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
                    new EmisClient.EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK, RequestsForSuccessOutcome.MedicalRecordGet, _sampleSuccessStatusCodes)
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
        public async Task GetPatientDocument_ReturnsSuccessResponseForHappyPath_WhenSuccessfulResponseFromEmis()
        {

            var patientDocumentResponse = new IndividualDocument
            {
                CompressedEncodedDocumentContent =
                    "H4sIAAAAAAAA/y1PX2+DIBz8QHtBsi7zFYtUWhGx/Eh5U1hqg6NN2mj1089ue7o/ucvlJOtRZ6Y3yfhod2HF4WFnsrjvMuVY9Z7Rj9YkvcV6dGwTK8x7hzUW2fMGgUsd3dQgHzuUk1rnlU7gqgDiykkN8PKICoKYl9aiOsXh0tHfbKHMM9dgVXOks97W42E5j34p3xUaiKGK1+GRl1QVX8zf27DJ1o6sQJ2OW2IFTQv/t5k1AwehYW7RLVrsUtm4zwP+/5OlV8+Su7xM+x9Ry9YU7AAAAA=="
            };
            _emisClient.Setup(x => x.MedicalDocumentGet(_emisUserSession.UserPatientLinkToken, _emisUserSession.SessionId, "1", _emisUserSession.EndUserSessionId))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<IndividualDocument>(HttpStatusCode.OK, RequestsForSuccessOutcome.MedicalDocumentGet, _sampleSuccessStatusCodes)
                    {
                        Body = patientDocumentResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));
            
            var result = await _systemUnderTest.GetPatientDocument(_gpLinkedAccountModel, "1", "img/jpeg", "example");
            
            _emisClient.Verify(x => x.MedicalDocumentGet(_emisUserSession.UserPatientLinkToken, _emisUserSession.SessionId, "1", _emisUserSession.EndUserSessionId));
            
            result.Should().BeAssignableTo<GetPatientDocumentResult.Success>()
                .Subject.Response.Should().NotBeNull();

        }
        
        [TestMethod]
        public async Task GetPatientDocument_ReturnsBadGateway_WhenUnsuccessfulResponseFromEmis()
        {

            var patientDocumentResponse = new IndividualDocument
            {
                CompressedEncodedDocumentContent =
                    "H4sIAAAAAAAA/y1PX2+DIBz8QHtBsi7zFYtUWhGx/Eh5U1hqg6NN2mj1089ue7o/ucvlJOtRZ6Y3yfhod2HF4WFnsrjvMuVY9Z7Rj9YkvcV6dGwTK8x7hzUW2fMGgUsd3dQgHzuUk1rnlU7gqgDiykkN8PKICoKYl9aiOsXh0tHfbKHMM9dgVXOks97W42E5j34p3xUaiKGK1+GRl1QVX8zf27DJ1o6sQJ2OW2IFTQv/t5k1AwehYW7RLVrsUtm4zwP+/5OlV8+Su7xM+x9Ry9YU7AAAAA=="
            };
            _emisClient.Setup(x => x.MedicalDocumentGet(_emisUserSession.UserPatientLinkToken, _emisUserSession.SessionId, "1", _emisUserSession.EndUserSessionId))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<IndividualDocument>(HttpStatusCode.BadRequest, RequestsForSuccessOutcome.MedicalDocumentGet, _sampleSuccessStatusCodes)
                    {
                        Body = patientDocumentResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));
            
            var result = await _systemUnderTest.GetPatientDocument(_gpLinkedAccountModel, "1", "img/jpeg", "example");
            
            _emisClient.Verify(x => x.MedicalDocumentGet(_emisUserSession.UserPatientLinkToken, _emisUserSession.SessionId, "1", _emisUserSession.EndUserSessionId));
            
            result.Should().BeAssignableTo<GetPatientDocumentResult.BadGateway>();

        }
    }
}