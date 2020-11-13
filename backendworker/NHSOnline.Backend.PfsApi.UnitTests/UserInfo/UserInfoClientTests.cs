using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.UserInfo;
using RichardSzalay.MockHttp;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.UserInfo
{
    [TestClass]
    public sealed class UserInfoClientTests : IDisposable
    {
        public static readonly Uri BaseUri = new Uri("http://base_url/v1/api/");

        private IUserInfoClient _systemUnderTest;
        private MockHttpMessageHandler _mockHttpHandler;
        private Mock<IUserInfoApiConfig> _configMock;
        private IFixture _fixture;
        private Mock<HttpContext> _httpContext;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _httpContext = HttpContextGetAccessTokenHelper.CreateMockHttpContext();
            _mockHttpHandler = new MockHttpMessageHandler();

            _configMock = _fixture.Create<Mock<IUserInfoApiConfig>>();
            _configMock.SetupGet(x => x.UserInfoApiBaseUrl).Returns(BaseUri);

            _fixture.Inject(new HttpClient(_mockHttpHandler));
            _fixture.Inject(_configMock);
            _fixture.Freeze<UserInfoHttpClient>();

            _systemUnderTest = _fixture.Create<UserInfoClient>();
        }

        [TestMethod]
        public async Task Post_ReturnsCreated()
        {
            // Arrange
            _mockHttpHandler
                .WhenUserInfo(HttpMethod.Post, string.Empty)
                .Respond(HttpStatusCode.Created);

            // Act
            var response = await _systemUnderTest.Post(_fixture.Create<string>(), _httpContext.Object);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.HasSuccessResponse.Should().Be(true);
        }

        [TestMethod]
        [DataRow(HttpStatusCode.InternalServerError)]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.Forbidden)]
        public async Task Post_ErrorResponseReceived_ReturnsError(HttpStatusCode httpStatusCode)
        {
            // Arrange
            _mockHttpHandler
                .WhenUserInfo(HttpMethod.Post, string.Empty)
                .Respond(httpStatusCode);

            // Act
            var response = await _systemUnderTest.Post(_fixture.Create<string>(), _httpContext.Object);

            // Assert
            response.StatusCode.Should().Be(httpStatusCode);
            response.HasSuccessResponse.Should().Be(false);
        }

        public void Dispose()
        {
            _mockHttpHandler.Dispose();
        }

    }
}