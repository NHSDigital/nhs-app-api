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
using NHSOnline.Backend.Worker.Bridges.Emis.Models.Prescriptions;
using NHSOnline.Backend.Worker.Router.Prescription;

namespace NHSOnline.Backend.Worker.UnitTests.Bridges.Emis
{
    [TestClass]
    public class EmisPrescriptionServiceTests
    {
        private EmisPrescriptionService _systemUnderTest;
        private Mock<IEmisClient> _emisClient;
        private EmisUserSession _userSession;
        private static IFixture _fixture;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _emisClient = _fixture.Freeze<Mock<IEmisClient>>();
            _userSession = _fixture.Freeze<EmisUserSession>();
            _systemUnderTest = _fixture.Create<EmisPrescriptionService>();
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromEmis()
        {
            // Arrange
            var date = DateTimeOffset.Now;
            var toDate = DateTimeOffset.Now;

            var prescriptionsResponse = _fixture.Create<PrescriptionRequestsGetResponse>();

            _emisClient.Setup(x => x.PrescriptionsGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId, date, toDate))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<PrescriptionRequestsGetResponse>(HttpStatusCode.OK)
                    {
                        Body = prescriptionsResponse,
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            // Act
            var result = await _systemUnderTest.Get(_userSession, date, toDate);

            // Assert
            _emisClient.Verify(x => x.PrescriptionsGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId, date, toDate));
            result.Should().BeAssignableTo<GetPrescriptionsResult.SuccessfullyRetrieved>();
            ((GetPrescriptionsResult.SuccessfullyRetrieved)result).Response.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Get_ReturnsBadRequest_WhenErrorReceivedFromEmis()
        {
            // Arrange
            var date = DateTimeOffset.Now;
            var toDate = DateTimeOffset.Now;
            const string alreadyLinkedErrorMessage = "Error occurred";
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = alreadyLinkedErrorMessage;
            
            _emisClient.Setup(x => x.PrescriptionsGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId, date, toDate))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<PrescriptionRequestsGetResponse>(HttpStatusCode.InternalServerError)
                        { ErrorResponse = errorResponse }));

            // Act
            var result = await _systemUnderTest.Get(_userSession, date, toDate);

            // Assert
            result.Should().BeAssignableTo<GetPrescriptionsResult.Unsuccessful>();
        }

        [TestMethod]
        public async Task Get_ReturnsBadRequest_WhenHttpExceptionOccursCallingEmis()
        {
            // Arrange
            var date = DateTimeOffset.Now;
            var toDate = DateTimeOffset.Now;
            const string alreadyLinkedErrorMessage = "Error occurred";
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = alreadyLinkedErrorMessage;

            _emisClient.Setup(x => x.PrescriptionsGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId, date, toDate))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Get(_userSession, date, toDate);

            // Assert
            result.Should().BeAssignableTo<GetPrescriptionsResult.Unsuccessful>();
            _emisClient.Verify();
        }
    }
}