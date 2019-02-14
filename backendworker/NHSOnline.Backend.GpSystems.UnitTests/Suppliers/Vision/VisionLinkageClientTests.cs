using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Linkage;
using NHSOnline.Backend.Support.ResponseParsers;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision
{
    [TestClass]
    public sealed class VisionLinkageClientTests : IDisposable
    {
        private IVisionLinkageClient _sut;
        private MockHttpMessageHandler _mockHttpHandler;
        private Mock<IVisionLinkageConfig> _configMock;
        private IFixture _fixture;

        private VisionLinkageHttpClient _httpClient;

        private static readonly Uri ApiUri = new Uri("http://vision_base_url/", UriKind.Absolute);

        private const string LinkageBasePath = "organisations/{0}/onlineservices/linkage";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _fixture.Register<IJsonResponseParser>(() => new JsonResponseParser());

            _mockHttpHandler = new MockHttpMessageHandler();

            _configMock = new Mock<IVisionLinkageConfig>();
            _configMock.SetupGet(x => x.ApiUrl).Returns(ApiUri);

            _httpClient = new VisionLinkageHttpClient(new HttpClient(_mockHttpHandler), _configMock.Object);

            _fixture.Inject(_configMock);
            _fixture.Inject(_httpClient);

            _sut = _fixture.Create<VisionLinkageClient>();
        }

        [TestMethod]
        public async Task GetLinkageKey_ReturnsALinkageKeyResponse_WhenValidlyRequested()
        {
            var expectedResponse = _fixture.Create<LinkageKeyGetResponse>();
            var getLinkageKeyRequest = _fixture.Create<GetLinkageKey>();

            var expectedUri = new Uri(ApiUri, $"organisations/{ getLinkageKeyRequest.OdsCode }/onlineservices/linkage?nhsNumber={ getLinkageKeyRequest.NhsNumber }");

            _mockHttpHandler
                .WhenVision(HttpMethod.Get, expectedUri)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var response = await _sut.GetLinkageKey(getLinkageKeyRequest);

            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.HasSuccessResponse.Should().Be(true);
        }

        [TestMethod]
        public async Task Vision_CreateLinkageKey_HappyPath_Returns_LinkageKeyPostResponse()
        {
            // Arrange
            var createLinkageKey = _fixture.Create<CreateLinkageKey>();

            var linkageResponse = _fixture.Create<LinkageKeyPostResponse>();

            var linkagePostUri = new Uri(ApiUri,
                String.Format(CultureInfo.CurrentCulture, LinkageBasePath, createLinkageKey.OdsCode));

            _mockHttpHandler.WhenVision(HttpMethod.Post, linkagePostUri)
                .WithContent(JsonConvert.SerializeObject(createLinkageKey.LinkageKeyPostRequest,
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }))
                .Respond("application/json", JsonConvert.SerializeObject(linkageResponse));

            // Act
            var response = await _sut.CreateLinkageKey(createLinkageKey);

            // Assert
            response.Body.Should().BeEquivalentTo(linkageResponse);
            response.StatusCode.Should().Be(200);
            response.ErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task Vision_CreateLinkageKey_Returns_ErrorResponse_409Conflict()
        {
            // Arrange
            var createLinkageKey = _fixture.Create<CreateLinkageKey>();

            var errorResponseWrapper = _fixture.Create<VisionLinkageClient.ErrorResponseWrapper>();

            var linkagePostUri = new Uri(ApiUri,
                String.Format(CultureInfo.CurrentCulture, LinkageBasePath, createLinkageKey.OdsCode));

            _mockHttpHandler.WhenVision(HttpMethod.Post, linkagePostUri)
                .WithContent(JsonConvert.SerializeObject(createLinkageKey.LinkageKeyPostRequest,
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }))
                .Respond(HttpStatusCode.Conflict, "application/json",
                    JsonConvert.SerializeObject(errorResponseWrapper));

            // Act
            var response = await _sut.CreateLinkageKey(createLinkageKey);

            // Assert
            response.StatusCode.Should().Be(409);
            response.ErrorResponse.Should().BeEquivalentTo(errorResponseWrapper.Error);
        }

        [TestCleanup]
        public void Dispose()
        {
            _mockHttpHandler.Dispose();
        }
    }
}