using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.Settings;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Extensions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Session;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.Session
{
    [TestClass]
    public class EmisSessionServiceTests
    {
        private IFixture _fixture;
        private Mock<IEmisClient> _mockEmisClient;
        private ILogger<EmisSessionService> _logger;
        private EmisSessionService _systemUnderTest;
        private SessionsEndUserSessionPostResponse _endUserSessionResponse;
        private SessionsPostResponse _sessionsResponse;
        private DemographicsGetResponse _demographicsResponse;

        private string _connectionToken;
        private string _odsCode;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = _fixture.Freeze<ILogger<EmisSessionService>>();
            _mockEmisClient = _fixture.Freeze<Mock<IEmisClient>>();
            _endUserSessionResponse = _fixture.Create<SessionsEndUserSessionPostResponse>();
            _connectionToken = _fixture.Create<string>();
            _odsCode = _fixture.Create<string>();
            
            _mockEmisClient.Setup(x => x.SessionsEndUserSessionPost()).Returns(
                Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<SessionsEndUserSessionPostResponse>(HttpStatusCode.OK)
                    {
                        Body = _endUserSessionResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            _sessionsResponse = _fixture.Create<SessionsPostResponse>();
            
            _mockEmisClient
                .Setup(x => x.SessionsPost(_endUserSessionResponse.EndUserSessionId,
                    It.Is<SessionsPostRequest>(y =>
                        y.AccessIdentityGuid.Equals(_connectionToken, StringComparison.Ordinal)
                        && y.NationalPracticeCode.Equals(_odsCode, StringComparison.Ordinal))))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<SessionsPostResponse>(HttpStatusCode.OK)
                    {
                        Body = _sessionsResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            _demographicsResponse = _fixture.Create<DemographicsGetResponse>();
            _mockEmisClient
                .Setup(x => x.DemographicsGet(_sessionsResponse.ExtractUserPatientLinkToken(), _sessionsResponse.SessionId, _endUserSessionResponse.EndUserSessionId))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK)
                    {
                        Body = _demographicsResponse,
                        ExceptionErrorResponse = null,
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
                        y.AccessIdentityGuid.Equals(_connectionToken, StringComparison.Ordinal)
                        && y.NationalPracticeCode.Equals(_odsCode, StringComparison.Ordinal))))
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
                .With(x => x.ExceptionErrorResponse, null)
                .Create();

            _mockEmisClient
                .Setup(x => x.SessionsPost(_endUserSessionResponse.EndUserSessionId,
                    It.Is<SessionsPostRequest>(y =>
                        y.AccessIdentityGuid.Equals(_connectionToken, StringComparison.Ordinal)
                        && y.NationalPracticeCode.Equals(_odsCode, StringComparison.Ordinal))))
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
                        y.AccessIdentityGuid.Equals(_connectionToken, StringComparison.Ordinal)
                        && y.NationalPracticeCode.Equals(_odsCode, StringComparison.Ordinal))))
                .Returns(Task.FromResult(forbiddenSession))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<SessionCreateResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Create_EmisClientDemographicsPostReturnsInternalServerError_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            var forbiddenSession = _fixture
                .Build<EmisClient.EmisApiObjectResponse<DemographicsGetResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .With(x => x.Body, null)
                .Create();

            _mockEmisClient
                .Setup(x => x.DemographicsGet(_sessionsResponse.ExtractUserPatientLinkToken(), _sessionsResponse.SessionId, _endUserSessionResponse.EndUserSessionId))
                .Returns(Task.FromResult(forbiddenSession))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<SessionCreateResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Create_EmisClientDemographicsPostReturnsBadRequest_ReturnsInvalidIm1ConnectionToken()
        {
            // Arrange
            var forbiddenSession = _fixture
                .Build<EmisClient.EmisApiObjectResponse<DemographicsGetResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.BadRequest)
                .With(x => x.Body, null)
                .Create();

            _mockEmisClient
                .Setup(x => x.DemographicsGet(_sessionsResponse.ExtractUserPatientLinkToken(), _sessionsResponse.SessionId, _endUserSessionResponse.EndUserSessionId))
                .Returns(Task.FromResult(forbiddenSession))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<SessionCreateResult.InvalidIm1ConnectionToken>();
        }

        [TestMethod]
        public async Task Create_EmisClientDemographicsPostReturnsForbidden_ReturnsInvalidIm1ConnectionToken()
        {
            // Arrange
            var forbiddenSession = _fixture
                .Build<EmisClient.EmisApiObjectResponse<DemographicsGetResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.Forbidden)
                .With(x => x.Body, null)
                .Create();

            _mockEmisClient
                .Setup(x => x.DemographicsGet(_sessionsResponse.ExtractUserPatientLinkToken(), _sessionsResponse.SessionId, _endUserSessionResponse.EndUserSessionId))
                .Returns(Task.FromResult(forbiddenSession))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<SessionCreateResult.InvalidIm1ConnectionToken>();
        }

        [TestMethod]
        public async Task Create_HappyPath_ReturnsSuccessfullyCreatedWithExpectedUserData()
        {
            // Arrange
            var systemUnderTest = new EmisSessionService(_mockEmisClient.Object, new EmisDemographicsMapper(),
                _logger);
            // Act
            var result = await systemUnderTest.Create(_connectionToken, _odsCode);

            // Assert
            _mockEmisClient.VerifyAll();
            result.Should().BeAssignableTo<SessionCreateResult.SuccessfullyCreated>();

            var expectedResult = new SessionCreateResult.SuccessfullyCreated(
                $"{_sessionsResponse.FirstName} {_sessionsResponse.Surname}",
                new EmisUserSession { NhsNumber = _demographicsResponse.ExtractNhsNumbers().First().NhsNumber}
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
                .Be(Supplier.Emis);
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
