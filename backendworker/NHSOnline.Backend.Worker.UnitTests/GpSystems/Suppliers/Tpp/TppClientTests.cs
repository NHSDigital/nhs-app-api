using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Worker.ResponseParsers;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp
{
    [TestClass]
    public sealed class TppClientTests : IDisposable
    {
        public static readonly string DefaultTypeHeaderValue = "DefaultTypeHeaderValue";

        private const string ApplicationName = "appName";
        private const string ApplicationVersion = "13";
        private const string ApplicationProviderId = "providerId";
        private const string ApplicationDeviceType = "deviceType";
        private static readonly Uri ApiUrl = new Uri("http://tppapitest:60015/Test/");
        private const string ApiVersion = "12";
        private const string UnitId = "unitId";

        private const string MediaType = "application/xml";
        private const string Suid = "session_id";

        private ITppClient _sut;
        private MockHttpMessageHandler _mockHttpHandler;
        private TppHttpClient _httpClient;
        private Mock<ITppConfig> _configMock;
        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Register<IXmlResponseParser>(() => new XmlResponseParser());

            _configMock = new Mock<ITppConfig>();
            _configMock.SetupGet(x => x.ApplicationName).Returns(ApplicationName);
            _configMock.SetupGet(x => x.ApplicationVersion).Returns(ApplicationVersion);
            _configMock.SetupGet(x => x.ApplicationProviderId).Returns(ApplicationProviderId);
            _configMock.SetupGet(x => x.ApplicationDeviceType).Returns(ApplicationDeviceType);
            _configMock.SetupGet(x => x.ApiUrl).Returns(ApiUrl);
            _configMock.SetupGet(x => x.ApiVersion).Returns(ApiVersion);
            _configMock.Setup(x => x.CreateGuid()).Returns(new Guid("8a1c6b80-7bcb-49fd-9c6f-4801e12207d6"));

            _mockHttpHandler = new MockHttpMessageHandler();
            _httpClient = new TppHttpClient(new HttpClient(_mockHttpHandler), _configMock.Object);

            _fixture.Inject(_configMock);
            _fixture.Inject(_httpClient);

            _sut = _fixture.Create<TppClient>();
        }

        [TestMethod]
        public async Task AuthenticatePostRequest_ReturnsAuthenticateReply_WhenValidlyRequested()
        {
            var authenticateRequestModel = _fixture.Create<Authenticate>();
            authenticateRequestModel.UnitId = UnitId;
            authenticateRequestModel.ApplyConfig(_configMock.Object);
            authenticateRequestModel.Application = new Application
            {
                Name = _configMock.Object.ApplicationName,
                Version = _configMock.Object.ApplicationVersion,
                ProviderId = _configMock.Object.ApplicationProviderId,
                DeviceType = _configMock.Object.ApplicationDeviceType
            };

            var expectedAuthenticateResponse = _fixture.Create<AuthenticateReply>();

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(TppClient.RequestTypeHeader, authenticateRequestModel.RequestType)
            };

            var responseHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(TppClient.ResponseSuidHeader, Suid)
            };

            var responseContent = new StringContent(expectedAuthenticateResponse.SerializeXml());

            _mockHttpHandler
                .WhenTpp(HttpMethod.Post, ApiUrl)
                .WithTppHeaders(requestHeaders)
                .WithContent(authenticateRequestModel.SerializeXml())
                .Respond(HttpStatusCode.OK, responseHeaders, responseContent);

            var response = await _sut.AuthenticatePost(authenticateRequestModel);

            response.Body.Should().BeEquivalentTo(expectedAuthenticateResponse);
            response.Headers.Should().BeEquivalentTo(responseHeaders);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();
        }

        [TestMethod]
        public async Task AuthenticatePostRequest_ReturnsErrorWithFalseSuccessCode_WhenResponseHasErrorInBody()
        {
            var authenticateRequestModel = _fixture.Create<Authenticate>();
            authenticateRequestModel.UnitId = UnitId;
            authenticateRequestModel.ApplyConfig(_configMock.Object);
            authenticateRequestModel.Application = new Application
            {
                Name = _configMock.Object.ApplicationName,
                Version = _configMock.Object.ApplicationVersion,
                ProviderId = _configMock.Object.ApplicationProviderId,
                DeviceType = _configMock.Object.ApplicationDeviceType
            };

            var expectedErrorResponse = _fixture.Create<Error>();

            var tppRequestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(TppClient.RequestTypeHeader, authenticateRequestModel.RequestType)
            };

            _mockHttpHandler
                .WhenTpp(HttpMethod.Post, ApiUrl)
                .WithTppHeaders(tppRequestHeaders)
                .WithContent(authenticateRequestModel.SerializeXml())
                .Respond(MediaType, expectedErrorResponse.SerializeXml());

            var response = await _sut.AuthenticatePost(authenticateRequestModel);

            response.ErrorResponse.Should().BeEquivalentTo(expectedErrorResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Should().BeNull();
            response.Headers.Should().BeNull();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.Unauthorized)]
        [DataRow(HttpStatusCode.NotFound)]
        public async Task AuthenticatePostRequest_ReturnsErrorWithSameStatusCode_WhenResponseIsHttpError(
            HttpStatusCode value)
        {
            var authenticateRequestModel = _fixture.Create<Authenticate>();
            authenticateRequestModel.UnitId = UnitId;
            authenticateRequestModel.ApplyConfig(_configMock.Object);
            authenticateRequestModel.Application = new Application
            {
                Name = _configMock.Object.ApplicationName,
                Version = _configMock.Object.ApplicationVersion,
                ProviderId = _configMock.Object.ApplicationProviderId,
                DeviceType = _configMock.Object.ApplicationDeviceType
            };

            var tppRequestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(TppClient.RequestTypeHeader, authenticateRequestModel.RequestType)
            };

            _mockHttpHandler
                .WhenTpp(HttpMethod.Post, ApiUrl)
                .WithTppHeaders(tppRequestHeaders)
                .Respond(value);

            var response = await _sut.AuthenticatePost(authenticateRequestModel);

            response.StatusCode.Should().Be(value);
            response.HasSuccessResponse.Should().BeFalse();
        }


        [TestMethod]
        public async Task
            OrderPrescriptionPostRequest_MakesHttpRequestToCorrectUrlWithCorrectHeaders_AndRespondsWithDeserializedXml()
        {
            var requestMedicationRequestModel = _fixture.Create<RequestMedication>();
            requestMedicationRequestModel.UnitId = UnitId;
            requestMedicationRequestModel.ApplyConfig(_configMock.Object);
            var expectedMedicationResponse = _fixture.Create<RequestMedicationReply>();

            var tppUserSession = _fixture.Create<TppUserSession>();

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(TppClient.RequestTypeHeader, requestMedicationRequestModel.RequestType)
            };

            var responseHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(TppClient.ResponseSuidHeader, Suid)
            };

            var responseContent = new StringContent(expectedMedicationResponse.SerializeXml());

            _mockHttpHandler
                .WhenTpp(HttpMethod.Post, ApiUrl)
                .WithTppHeaders(requestHeaders)
                .WithContent(requestMedicationRequestModel.SerializeXml())
                .Respond(HttpStatusCode.OK, responseHeaders, responseContent);

            var response = await _sut.OrderPrescriptionsPost(tppUserSession, requestMedicationRequestModel);

            response.Body.Should().BeEquivalentTo(expectedMedicationResponse);
            response.Headers.Should().BeEquivalentTo(responseHeaders);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();
        }
        
        [TestMethod]
        public async Task LinkAccountPostRequest_ReturnsLinkAccountReply_WhenValidlyRequested()
        {
            var linkRequestModel = _fixture.Create<LinkAccount>();
            linkRequestModel.OrganisationCode = UnitId;
            linkRequestModel.ApplyConfig(_configMock.Object);
            
            var expectedLinkAccountResponse = _fixture.Create<LinkAccountReply>();

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(TppClient.RequestTypeHeader, linkRequestModel.RequestType)
            };

            var responseHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(TppClient.ResponseSuidHeader, Suid)
            };
            
            var responseContent = new StringContent(expectedLinkAccountResponse.SerializeXml());
            
            _mockHttpHandler
                .WhenTpp(HttpMethod.Post, ApiUrl)
                .WithTppHeaders(requestHeaders)
                .WithContent(linkRequestModel.SerializeXml())
                .Respond(HttpStatusCode.OK, responseHeaders, responseContent);    

            var response = await _sut.LinkAccountPost(linkRequestModel);

            response.Body.Should().BeEquivalentTo(expectedLinkAccountResponse);
            response.Headers.Should().BeEquivalentTo(responseHeaders);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();
        }
        
        [TestMethod]
        public async Task LinkAccountPostRequest_ReturnsErrorWithFalseSuccessCode_WhenResponseHasErrorInBody()
        {            
            var linkAccountRequestModel = _fixture.Create<LinkAccount>();
            linkAccountRequestModel.OrganisationCode = UnitId;
            linkAccountRequestModel.ApplyConfig(_configMock.Object);
            
            var expectedErrorResponse = _fixture.Create<Error>();

            var tppRequestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(TppClient.RequestTypeHeader, linkAccountRequestModel.RequestType)
            };
            
            _mockHttpHandler
                .WhenTpp(HttpMethod.Post, ApiUrl)
                .WithTppHeaders(tppRequestHeaders)
                .WithContent(linkAccountRequestModel.SerializeXml())
                .Respond(MediaType, expectedErrorResponse.SerializeXml());

            var response = await _sut.LinkAccountPost(linkAccountRequestModel);

            response.ErrorResponse.Should().BeEquivalentTo(expectedErrorResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Should().BeNull();
            response.Headers.Should().BeNull();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.Unauthorized)]
        [DataRow(HttpStatusCode.NotFound)]
        public async Task LinkAccountPostRequest_ReturnsErrorWithSameStatusCode_WhenResponseIsHttpError(
            HttpStatusCode value)
        {
            var linkAccountRequestModel = _fixture.Create<LinkAccount>();
            linkAccountRequestModel.OrganisationCode = UnitId;
            linkAccountRequestModel.ApplyConfig(_configMock.Object);

            var tppRequestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(TppClient.RequestTypeHeader, linkAccountRequestModel.RequestType)
            };

            _mockHttpHandler
                .WhenTpp(HttpMethod.Post, ApiUrl)
                .WithTppHeaders(tppRequestHeaders)
                .Respond(value);

            var response = await _sut.LinkAccountPost(linkAccountRequestModel);

            response.StatusCode.Should().Be(value);
            response.HasSuccessResponse.Should().BeFalse();
        }

        [TestCleanup]
        public void Dispose()
        {
            _mockHttpHandler.Dispose();
        }
    }
}
