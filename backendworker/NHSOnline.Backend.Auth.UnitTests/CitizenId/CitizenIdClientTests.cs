using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using NHSOnline.Backend.Auth.CitizenId;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support.ResponseParsers;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.Auth.UnitTests.CitizenId
{
    [TestClass]
    public sealed class CitizenIdClientTests : IDisposable
    {
        private IFixture _fixture;

        private Uri _citizenIdApiBaseUrl;
        private string _clientId;
        private string _clientSecret;
        private Mock<ICitizenIdConfig> _mockConfig;
        private CitizenIdClient _systemUnderTest;
        private MockHttpMessageHandler _mockHttpHandler;
        private CitizenIdHttpClient _httpClient;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Register<IJsonResponseParser>(() => new JsonResponseParser());
            _citizenIdApiBaseUrl = _fixture.Create<Uri>();
            _clientId = _fixture.Create<string>();
            _clientSecret = _fixture.Create<string>();

            _mockHttpHandler = new MockHttpMessageHandler();
            _mockConfig = _fixture.Freeze<Mock<ICitizenIdConfig>>();

            _mockConfig.SetupGet(x => x.CitizenIdApiBaseUrl).Returns(_citizenIdApiBaseUrl);
            _mockConfig.SetupGet(x => x.ClientId).Returns(_clientId);
            _mockConfig.SetupGet(x => x.ClientSecret).Returns(_clientSecret);

            _httpClient = new CitizenIdHttpClient(new HttpClient(_mockHttpHandler), _mockConfig.Object);
            _fixture.Inject(_httpClient);

            _systemUnderTest = _fixture.Create<CitizenIdClient>();
        }

        [TestMethod]
        public async Task ExchangeAuthToken_HappyPath()
        {
            // Arrange
            var authCode = _fixture.Create<string>();
            var codeVerifier = _fixture.Create<string>();
            var redirectUrl = _fixture.Create<string>();
            var expectedTokenResponse = _fixture.Create<Token>();

            var dict = CreateTokenBody(authCode, redirectUrl, codeVerifier);

            var basicAuthString = Convert.ToBase64String(
                System.Text.Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}"));

            _mockHttpHandler
                .When(HttpMethod.Post, new Uri(_citizenIdApiBaseUrl, "token").ToString())
                .WithFormData(dict)
                .WithHeaders("Authorization", $"Basic {basicAuthString}")
                .Respond("application/json", JsonConvert.SerializeObject(expectedTokenResponse));

            // Act
            var response = await _systemUnderTest.ExchangeAuthToken(authCode, codeVerifier, redirectUrl);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedTokenResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();
            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }

        [TestMethod]
        public async Task ExchangeAuthToken_ErrorResponseReceived_ReturnsErrorMessage()
        {
            // Arrange
            var authCode = _fixture.Create<string>();
            var codeVerifier = _fixture.Create<string>();
            var redirectUrl = _fixture.Create<string>();
            var expectedErrorResponse = _fixture.Create<ErrorResponse>();

            var dict = CreateTokenBody(authCode, redirectUrl, codeVerifier);

            var basicAuthString = Convert.ToBase64String(
                System.Text.Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}"));

            _mockHttpHandler
                .When(HttpMethod.Post, new Uri(_citizenIdApiBaseUrl, "token").ToString())
                .WithFormData(dict)
                .WithHeaders("Authorization", $"Basic {basicAuthString}")
                .Respond(HttpStatusCode.Forbidden, "application/json",
                    JsonConvert.SerializeObject(expectedErrorResponse));

            // Act
            var response = await _systemUnderTest.ExchangeAuthToken(authCode, codeVerifier, redirectUrl);

            // Assert
            response.Body.Should().BeEquivalentTo(new Token());
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            response.ErrorResponse.Should().BeEquivalentTo(expectedErrorResponse);
            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }

        [TestMethod]
        public async Task GetSigningKeys_ErrorResponseReceived_ReturnsErrorMessage()
        {
            // Arrange
            var expectedErrorResponse = _fixture.Create<ErrorResponse>();

            _mockHttpHandler
                .When(HttpMethod.Get, new Uri(_citizenIdApiBaseUrl, ".well-known/jwks.json").ToString())
                .Respond(HttpStatusCode.InternalServerError, "application/json",
                    JsonConvert.SerializeObject(expectedErrorResponse));

            // Act
            var response = await _systemUnderTest.GetSigningKeys();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            response.ErrorResponse.Should().BeEquivalentTo(expectedErrorResponse);
            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }

        [TestMethod]
        public async Task GetSigningKeys_ReceivedOkWithPoorlyFormedJson_ReturnsErrorMessage()
        {
            // Arrange
            _mockHttpHandler
                .When(HttpMethod.Get, new Uri(_citizenIdApiBaseUrl, ".well-known/jwks.json").ToString())
                .Respond(HttpStatusCode.OK, "application/json",
                    "{ \"something\" : \"Here\"}}}");

            // Act
            var response = await _systemUnderTest.GetSigningKeys();

            // Assert
            response.Body.Should().BeNull();
            response.ErrorResponse.Should().BeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }

        [TestMethod]
        public async Task GetSigningKeys_ReceivedOkWithNoContent_ReturnsErrorMessage()
        {
            // Arrange
            _mockHttpHandler
                .When(HttpMethod.Get, new Uri(_citizenIdApiBaseUrl, ".well-known/jwks.json").ToString())
                .Respond(HttpStatusCode.OK, "application/json",
                    "");

            // Act
            var response = await _systemUnderTest.GetSigningKeys();

            // Assert
            response.Body.Should().BeNull();
            response.ErrorResponse.Should().BeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }

        [TestMethod]
        public async Task GetSigningKeys_HappyPath()
        {
            // Arrange
            var expectedResponse = _fixture.Create<JsonWebKeySet>();

            _mockHttpHandler
                .When(HttpMethod.Get, new Uri(_citizenIdApiBaseUrl, ".well-known/jwks.json").ToString())
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await _systemUnderTest.GetSigningKeys();

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }

        [TestMethod]
        public async Task GetUserInfo_HappyPath()
        {
            // Arrange
            var bearerToken = _fixture.Create<string>();
            var expectedTokenResponse = _fixture.Create<UserInfo>();

            _mockHttpHandler
                .When(HttpMethod.Get, new Uri(_citizenIdApiBaseUrl, "userinfo").ToString())
                .WithHeaders("Authorization", $"Bearer {bearerToken}")
                .Respond("application/json", JsonConvert.SerializeObject(expectedTokenResponse));

            // Act
            var response = await _systemUnderTest.GetUserInfo(bearerToken);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedTokenResponse);
            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }

        [TestMethod]
        public async Task GetUserInfo_ErrorResponseReceived_ReturnsErrorMessage()
        {
            // Arrange
            var bearerToken = _fixture.Create<string>();
            var expectedErrorResponse = _fixture.Create<ErrorResponse>();

            _mockHttpHandler
                .When(HttpMethod.Get, new Uri(_citizenIdApiBaseUrl, "userinfo").ToString())
                .WithHeaders("Authorization", $"Bearer {bearerToken}")
                .Respond(HttpStatusCode.InternalServerError, "application/json",
                    JsonConvert.SerializeObject(expectedErrorResponse));

            // Act
            var response = await _systemUnderTest.GetUserInfo(bearerToken);

            // Assert
            response.Body.Should().BeEquivalentTo(new UserInfo());
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            response.ErrorResponse.Should().BeEquivalentTo(expectedErrorResponse);
            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }

        [TestMethod]
        public async Task GetUserInfo_EndpointCalled_ReturnsErrorResponseCodeWithNullBody_ResponseHasEmptyErrorProperties()
        {
            // Arrange
            var bearerToken = _fixture.Create<string>();

            _mockHttpHandler
                .When(HttpMethod.Get, new Uri(_citizenIdApiBaseUrl, "userinfo").ToString())
                .WithHeaders("Authorization", $"Bearer {bearerToken}")
                .Respond(HttpStatusCode.Forbidden);

            // Act
            var response = await _systemUnderTest.GetUserInfo(bearerToken);

            // Assert
            response.Body.Should().BeNull();
            response.ErrorResponse.Should().BeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public async Task GetUserInfo_EndpointCalled_ReturnsErrorResponseCodeWithEmptyBody_ResponseHasEmptyErrorProperties()
        {
            // Arrange
            var bearerToken = _fixture.Create<string>();

            _mockHttpHandler
                .When(HttpMethod.Get, new Uri(_citizenIdApiBaseUrl, "userinfo").ToString())
                .WithHeaders("Authorization", $"Bearer {bearerToken}")
                .Respond(HttpStatusCode.Forbidden, "application/json", string.Empty);

            // Act
            var response = await _systemUnderTest.GetUserInfo(bearerToken);

            // Assert
            response.Body.Should().BeNull();
            response.ErrorResponse.Should().BeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        public void Dispose()
        {
            _mockHttpHandler.Dispose();
        }

        private Dictionary<string, string> CreateTokenBody(string authCode, string redirectUrl, string codeVerifier)
        {
            var dict = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", authCode },
                { "redirect_uri", redirectUrl },
                { "code_verifier", codeVerifier },
                { "client_id", _clientId },
                { "code_challenge_method", "S256" }
            };
            return dict;
        }
    }
}