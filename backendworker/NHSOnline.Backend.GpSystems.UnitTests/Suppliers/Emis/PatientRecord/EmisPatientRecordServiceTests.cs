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

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.PatientRecord
{
    [TestClass]
    public class EmisPatientRecordServiceTests
    {
        private EmisPatientRecordService _systemUnderTest;
        private Mock<IEmisClient> _emisClient;
        private EmisUserSession _emisUserSession;
        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _emisUserSession = _fixture.Create<EmisUserSession>();
            _emisClient = _fixture.Freeze<Mock<IEmisClient>>();
            
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
            
            _emisClient.Setup(x => x.MedicalRecordGet(_emisUserSession.UserPatientLinkToken, _emisUserSession.SessionId, _emisUserSession.EndUserSessionId, RecordType.Medication))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK)
                    {
                        Body = medicationsResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));
            
            _emisClient.Setup(x => x.MedicalRecordGet(_emisUserSession.UserPatientLinkToken, _emisUserSession.SessionId, _emisUserSession.EndUserSessionId, RecordType.Allergies))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK)
                    {
                        Body = allergiesResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));
            
            _emisClient.Setup(x => x.MedicalRecordGet(_emisUserSession.UserPatientLinkToken, _emisUserSession.SessionId, _emisUserSession.EndUserSessionId, RecordType.Immunisations))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK)
                    {
                        Body = immunisationsResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));
            
            _emisClient.Setup(x => x.MedicalRecordGet(_emisUserSession.UserPatientLinkToken, _emisUserSession.SessionId, _emisUserSession.EndUserSessionId, RecordType.TestResults))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK)
                    {
                        Body = testResultsResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));
            
            _emisClient.Setup(x => x.MedicalRecordGet(_emisUserSession.UserPatientLinkToken, _emisUserSession.SessionId, _emisUserSession.EndUserSessionId, RecordType.Problems))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK)
                    {
                        Body = problemsResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));
            
            _emisClient.Setup(x => x.MedicalRecordGet(_emisUserSession.UserPatientLinkToken, _emisUserSession.SessionId, _emisUserSession.EndUserSessionId, RecordType.Consultations))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK)
                    {
                        Body = consultationsResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            _emisClient.Setup(x => x.MedicalRecordGet(_emisUserSession.UserPatientLinkToken, _emisUserSession.SessionId, _emisUserSession.EndUserSessionId, RecordType.Documents))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK)
                    {
                        Body = documentsResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            // Act
            var result = await _systemUnderTest.GetMyRecord(_emisUserSession);

            // Assert
            _emisClient.Verify(x => x.MedicalRecordGet(_emisUserSession.UserPatientLinkToken, _emisUserSession.SessionId, _emisUserSession.EndUserSessionId, RecordType.Allergies));

            result.Should().BeAssignableTo<GetMyRecordResult.Success>()
                .Subject.Response.Should().NotBeNull();
        }
    }
}