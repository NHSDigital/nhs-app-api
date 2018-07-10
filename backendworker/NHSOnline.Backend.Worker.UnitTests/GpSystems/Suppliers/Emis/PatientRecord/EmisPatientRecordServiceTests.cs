using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.PatientRecord
{
    [TestClass]
    public class EmisPatientRecordServiceTests
    {
        private EmisPatientRecordService _systemUnderTest;
        private Mock<IEmisClient> _emisClient;
        private EmisUserSession _userSession;
        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _emisClient = _fixture.Freeze<Mock<IEmisClient>>();
            _userSession = _fixture.Freeze<EmisUserSession>();
            _systemUnderTest = _fixture.Create<EmisPatientRecordService>();
        }
        
        [TestMethod]
        public async Task Get_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromEmis()
        {
            var allergiesResponse = _fixture.Create<MedicationRootObject>();
            var medicationsResponse = _fixture.Create<MedicationRootObject>();
            var immunisationsResponse = _fixture.Create<MedicationRootObject>();
            var testResultsResponse = _fixture.Create<MedicationRootObject>();
            var problemsResponse = _fixture.Create<MedicationRootObject>();
            var consultationsResponse = _fixture.Create<MedicationRootObject>();
            
            _emisClient.Setup(x => x.MedicalRecordGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId, RecordType.Medication))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK)
                    {
                        Body = medicationsResponse,
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));
            
            _emisClient.Setup(x => x.MedicalRecordGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId, RecordType.Allergies))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK)
                    {
                        Body = allergiesResponse,
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));
            
            _emisClient.Setup(x => x.MedicalRecordGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId, RecordType.Immunisations))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK)
                    {
                        Body = immunisationsResponse,
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));
            
            _emisClient.Setup(x => x.MedicalRecordGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId, RecordType.TestResults))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK)
                    {
                        Body = testResultsResponse,
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));
            
            _emisClient.Setup(x => x.MedicalRecordGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId, RecordType.Problems))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK)
                    {
                        Body = problemsResponse,
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));
            
            _emisClient.Setup(x => x.MedicalRecordGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId, RecordType.Consultations))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK)
                    {
                        Body = consultationsResponse,
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            // Act
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            _emisClient.Verify(x => x.MedicalRecordGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId, RecordType.Allergies));
            result.Should().BeAssignableTo<GetMyRecordResult.SuccessfullyRetrieved>();
            ((GetMyRecordResult.SuccessfullyRetrieved)result).Response.Should().NotBeNull();
        }
    }
}