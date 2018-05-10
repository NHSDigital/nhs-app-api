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
using NHSOnline.Backend.Worker.Router.Course;

namespace NHSOnline.Backend.Worker.UnitTests.Bridges.Emis
{
    [TestClass]
    public class EmisCourseServiceTests
    {
        private EmisCourseService _systemUnderTest;
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
            _systemUnderTest = _fixture.Create<EmisCourseService>();
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromEmis()
        {
            // Arrange
            var coursesResponse = _fixture.Create<CoursesGetResponse>();

            _emisClient.Setup(x => x.CoursesGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<CoursesGetResponse>(HttpStatusCode.OK)
                    {
                        Body = coursesResponse,
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            // Act
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            _emisClient.Verify(x => x.CoursesGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId));
            result.Should().BeAssignableTo<GetCoursesResult.SuccessfullyRetrieved>();
            ((GetCoursesResult.SuccessfullyRetrieved)result).Response.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Get_ReturnsBadRequest_WhenErrorReceivedFromEmis()
        {
            // Arrange
            const string alreadyLinkedErrorMessage = "Error occurred";
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = alreadyLinkedErrorMessage;
            
            _emisClient.Setup(x => x.CoursesGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<CoursesGetResponse>(HttpStatusCode.InternalServerError)
                        { ErrorResponse = errorResponse }));

            // Act
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            result.Should().BeAssignableTo<GetCoursesResult.Unsuccessful>();
        }

        [TestMethod]
        public async Task Get_ReturnsBadRequest_WhenHttpExceptionOccursCallingEmis()
        {
            // Arrange
            const string alreadyLinkedErrorMessage = "Error occurred";
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = alreadyLinkedErrorMessage;

            _emisClient.Setup(x => x.CoursesGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            result.Should().BeAssignableTo<GetCoursesResult.Unsuccessful>();
            _emisClient.Verify();
        }
    }
}