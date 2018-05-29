using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Bridges.Emis;
using NHSOnline.Backend.Worker.Bridges.Emis.Models;
using NHSOnline.Backend.Worker.Bridges.Emis.Models.PatientRecord;
using NHSOnline.Backend.Worker.Router.MyRecord;

namespace NHSOnline.Backend.Worker.UnitTests.Bridges.Emis
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
            var allergiesResponse = _fixture.Create<AllergyRequestsGetResponse>();

            _emisClient.Setup(x => x.AllergiesGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<AllergyRequestsGetResponse>(HttpStatusCode.OK)
                    {
                        Body = allergiesResponse,
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            // Act
            var result = await _systemUnderTest.GetPatientAllergies(_userSession);

            // Assert
            _emisClient.Verify(x => x.AllergiesGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId));
            result.Should().BeAssignableTo<GetAllergyResult.SuccessfullyRetrieved>();
            ((GetAllergyResult.SuccessfullyRetrieved)result).Response.Should().NotBeNull();
        }
        
        [TestMethod]
        public async Task Get_ReturnsBadRequest_WhenErrorReceivedFromEmis()
        {
            // Arrange
            const string alreadyLinkedErrorMessage = "Error occurred";
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = alreadyLinkedErrorMessage;
            
            _emisClient.Setup(x => x.AllergiesGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<AllergyRequestsGetResponse>(HttpStatusCode.InternalServerError)
                        { ErrorResponse = errorResponse }));

            // Act
            var result = await _systemUnderTest.GetPatientAllergies(_userSession);

            // Assert
            result.Should().BeAssignableTo<GetAllergyResult.Unsuccessful>();
        }

        [TestMethod]
        public async Task Get_ReturnsBadRequest_WhenHttpExceptionOccursCallingEmis()
        {
            // Arrange
            const string alreadyLinkedErrorMessage = "Error occurred";
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = alreadyLinkedErrorMessage;

            _emisClient.Setup(x => x.AllergiesGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetPatientAllergies(_userSession);

            // Assert
            result.Should().BeAssignableTo<GetAllergyResult.Unsuccessful>();
            _emisClient.Verify();
        }
        
        [TestMethod]
        public async Task Get_ReturnsBadRequest_WhenNullReferenceExceptionOccursCallingEmis()
        {
            // Arrange
            const string alreadyLinkedErrorMessage = "Error occurred";
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = alreadyLinkedErrorMessage;

            _emisClient.Setup(x => x.AllergiesGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId))
                .Throws<NullReferenceException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetPatientAllergies(_userSession);

            // Assert
            result.Should().BeAssignableTo<GetAllergyResult.SupplierBadData>();
            _emisClient.Verify();
        }
    }
}