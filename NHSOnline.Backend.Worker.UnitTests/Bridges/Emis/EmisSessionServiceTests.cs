using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.Bridges.Emis;
using NHSOnline.Backend.Worker.Bridges.Emis.Models;
using NHSOnline.Backend.Worker.Router.Session;

namespace NHSOnline.Backend.Worker.UnitTests.Bridges.Emis
{
    [TestClass]
    public class EmisSessionServiceTests
    {
        private IFixture _fixture;
        private Mock<IOptions<ConfigurationSettings>> _settings;
        private Mock<IEmisClient> _mockEmisClient;
        private EmisSessionService _systemUnderTest;
        private string _connectionToken;
        private string _odsCode;
        private SessionsEndUserSessionPostResponse _endUserSessionResponse;
        private SessionsPostResponse _sessionsResponse;
        private int _defaultSessionExpiryMinutes;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockEmisClient = _fixture.Freeze<Mock<IEmisClient>>();

            _connectionToken = _fixture.Create<string>();
            _odsCode = _fixture.Create<string>();
            _defaultSessionExpiryMinutes = _fixture.Create<int>();
            _endUserSessionResponse = _fixture.Create<SessionsEndUserSessionPostResponse>();

            _settings = new Mock<IOptions<ConfigurationSettings>>();
            _settings.Setup(x => x.Value).Returns(
                new ConfigurationSettings
                {
                    DefaultSessionExpiryMinutes = _defaultSessionExpiryMinutes
                });
            
            _mockEmisClient.Setup(x => x.SessionsEndUserSessionPost()).Returns(
                Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<SessionsEndUserSessionPostResponse>(HttpStatusCode.OK)
                    {
                        Body = _endUserSessionResponse,
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            _sessionsResponse = _fixture.Create<SessionsPostResponse>();

            _mockEmisClient
                .Setup(x => x.SessionsPost(_endUserSessionResponse.EndUserSessionId,
                    It.Is<SessionsPostRequest>(y =>
                        y.AccessIdentityGuid == _connectionToken && y.NationalPracticeCode == _odsCode)))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<SessionsPostResponse>(HttpStatusCode.OK)
                    {
                        Body = _sessionsResponse,
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            _systemUnderTest = _fixture.Create<EmisSessionService>();
        }

        [TestMethod]
        public async Task
            Create_EmisClientThrowsHttpRequestExceptionFromEndUserSession_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            // emis client throws HttpRequestException
            _mockEmisClient
                .Setup(x => x.SessionsEndUserSessionPost())
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<SessionCreateResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Create_EmisClientThrowsHttpRequestExceptionFromSession_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            // emis client throws HttpRequestException
            _mockEmisClient
                .Setup(x => x.SessionsPost(It.IsAny<string>(), It.IsAny<SessionsPostRequest>()))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<SessionCreateResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Create_EmisClientEndUserSessionPostUnsuccessful_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            var unsuccessfulEndUserSession = _fixture
                .Build<EmisClient.EmisApiObjectResponse<SessionsEndUserSessionPostResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .With(x => x.Body, null)
                .Create();

            _mockEmisClient
                .Setup(x => x.SessionsEndUserSessionPost())
                .Returns(Task.FromResult(unsuccessfulEndUserSession))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<SessionCreateResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Create_EmisClientSessionsPostReturnsForbidden_ReturnsInvalidIm1ConnectionToken()
        {
            // Arrange
            var forbiddenSession = _fixture
                .Build<EmisClient.EmisApiObjectResponse<SessionsPostResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.Forbidden)
                .With(x => x.Body, null)
                .With(x => x.ErrorResponseBadRequest, null)
                .Create();

            _mockEmisClient
                .Setup(x => x.SessionsPost(_endUserSessionResponse.EndUserSessionId,
                    It.Is<SessionsPostRequest>(y =>
                        y.AccessIdentityGuid == _connectionToken && y.NationalPracticeCode == _odsCode)))
                .Returns(Task.FromResult(forbiddenSession))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<SessionCreateResult.InvalidIm1ConnectionToken>();
        }

        [TestMethod]
        public async Task Create_EmisClientSessionsPostReturnsBadRequest_ReturnsInvalidIm1ConnectionToken()
        {
            // Arrange
            var badRequestSession = _fixture
                .Build<EmisClient.EmisApiObjectResponse<SessionsPostResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.BadRequest)
                .With(x => x.Body, null)
                .With(x => x.ErrorResponse, null)
                .Create();

            _mockEmisClient
                .Setup(x => x.SessionsPost(_endUserSessionResponse.EndUserSessionId,
                    It.Is<SessionsPostRequest>(y =>
                        y.AccessIdentityGuid == _connectionToken && y.NationalPracticeCode == _odsCode)))
                .Returns(Task.FromResult(badRequestSession))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<SessionCreateResult.InvalidIm1ConnectionToken>();
        }

        [TestMethod]
        public async Task Create_EmisClientSessionsPostReturnsInternalServerError_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            var forbiddenSession = _fixture
                .Build<EmisClient.EmisApiObjectResponse<SessionsPostResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .With(x => x.Body, null)
                .Create();

            _mockEmisClient
                .Setup(x => x.SessionsPost(_endUserSessionResponse.EndUserSessionId,
                    It.Is<SessionsPostRequest>(y =>
                        y.AccessIdentityGuid == _connectionToken && y.NationalPracticeCode == _odsCode)))
                .Returns(Task.FromResult(forbiddenSession))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<SessionCreateResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Create_HappyPath_ReturnsSuccessfullyCreatedWithExpectedUserData()
        {
            // Arrange
            var sessionTimeoutInSeconds = _defaultSessionExpiryMinutes * 60;
            var systemUnderTest = new EmisSessionService(_mockEmisClient.Object, _settings.Object);
            // Act
            var result = await systemUnderTest.Create(_connectionToken, _odsCode);

            // Assert
            _mockEmisClient.VerifyAll();
            result.Should().BeAssignableTo<SessionCreateResult.SuccessfullyCreated>();

            var expectedResult = new SessionCreateResult.SuccessfullyCreated(
                _sessionsResponse.FirstName, 
                _sessionsResponse.Surname,
                new EmisUserSession(),
                sessionTimeoutInSeconds
            );

            (result as SessionCreateResult.SuccessfullyCreated).Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task Create_HappyPath_ReturnsAUserSessionInTheResult()
        {
            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode);
            
            // Assert
            result
                .Should()
                .BeAssignableTo<SessionCreateResult.SuccessfullyCreated>()
                .Subject
                .UserSession
                .Should()
                .NotBeNull();
        }

        [TestMethod] 
        public async Task Create_HappyPath_ReturnsAUserSessionWithTheEmisSupplier()
        {
            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode);
            
            // Assert
            result
                .Should()
                .BeAssignableTo<SessionCreateResult.SuccessfullyCreated>()
                .Subject
                .UserSession
                .Supplier
                .Should()
                .Be(SupplierEnum.Emis);
        }
        
        [TestMethod]
        public async Task Create_HappyPath_ReturnsAEmisUserSession()
        {
            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode);
            
            // Assert
            result
                .Should()
                .BeAssignableTo<SessionCreateResult.SuccessfullyCreated>()
                .Subject
                .UserSession
                .Should()
                .BeAssignableTo<EmisUserSession>();
        }
    }
}