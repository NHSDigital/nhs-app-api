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
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Prescriptions;
using NHSOnline.Backend.Support.ResponseParsers;
using NHSOnline.Backend.Support.Http;
using RichardSzalay.MockHttp;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp
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
        private static readonly Guid Uuid = new Guid("8a1c6b80-7bcb-49fd-9c6f-4801e12207d6"); 
        private const string UnitId = "unitId";

        private const string CertificatePath = "CertificatePath";
        private const string CertificatePassphrase = "CerticiatePassphrase";
        private const string MediaType = "application/xml";
        private const string Suid = "session_id";

        private int? PrescriptionsMaxCoursesSoftLimit = 100;
        private int? CoursesMaxCoursesLimit = 100;

        private ITppClient _systemUnderTest;
        private MockHttpMessageHandler _mockHttpHandler;
        private TppHttpClient _httpClient;
        private TppConfigurationSettings _tppConfig;

        private Mock<IGuidCreator> _guidCreator;
        private IFixture _fixture;
        private const string Environment = "environment";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Register<IXmlResponseParser>(() => new XmlResponseParser());

            _guidCreator = new Mock<IGuidCreator>();
            _guidCreator.Setup(x => x.CreateGuid()).Returns(Uuid);

            _tppConfig = new TppConfigurationSettings(ApiUrl, ApiVersion, ApplicationName, ApplicationVersion, 
                ApplicationProviderId, ApplicationDeviceType, CertificatePassphrase, CertificatePath, 
                PrescriptionsMaxCoursesSoftLimit, CoursesMaxCoursesLimit, Environment);
            _mockHttpHandler = new MockHttpMessageHandler();
            _httpClient = new TppHttpClient(new HttpClient(_mockHttpHandler), _tppConfig);

            _fixture.Inject(_httpClient);
            _fixture.Inject(_tppConfig);
            _fixture.Inject(_guidCreator);

            _systemUnderTest = _fixture.Create<TppClient>();
        }

        [TestMethod]
        public async Task AuthenticatePostRequest_ReturnsAuthenticateReply_WhenValidlyRequested()
        {
            var authenticateRequestModel = _fixture.Create<Authenticate>();
            authenticateRequestModel.UnitId = UnitId;
            authenticateRequestModel.Uuid = Uuid;
            authenticateRequestModel.ApiVersion = ApiVersion;
            authenticateRequestModel.Application = new Application
            {
                Name = ApplicationName,
                Version = ApplicationVersion,
                ProviderId = authenticateRequestModel.ProviderId,
                DeviceType = ApplicationDeviceType
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
 
            var response = await _systemUnderTest.AuthenticatePost(authenticateRequestModel);

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
            authenticateRequestModel.Uuid = Uuid;
            authenticateRequestModel.ApiVersion = ApiVersion;
            authenticateRequestModel.Application = new Application
            {
                Name = ApplicationName,
                Version = ApplicationVersion,
                ProviderId = authenticateRequestModel.ProviderId,
                DeviceType = ApplicationDeviceType
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

            var response = await _systemUnderTest.AuthenticatePost(authenticateRequestModel);

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
            authenticateRequestModel.Uuid = Uuid;
            authenticateRequestModel.ApiVersion = ApiVersion;
            authenticateRequestModel.Application = new Application
            {
                Name = ApplicationName,
                Version = ApplicationVersion,
                ProviderId = authenticateRequestModel.ProviderId,
                DeviceType = ApplicationDeviceType
            };

            var tppRequestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(TppClient.RequestTypeHeader, authenticateRequestModel.RequestType)
            };

            _mockHttpHandler
                .WhenTpp(HttpMethod.Post, ApiUrl)
                .WithTppHeaders(tppRequestHeaders)
                .Respond(value);

            var response = await _systemUnderTest.AuthenticatePost(authenticateRequestModel);

            response.StatusCode.Should().Be(value);
            response.HasSuccessResponse.Should().BeFalse();
        }


        [TestMethod]
        public async Task
            OrderPrescriptionPostRequest_MakesHttpRequestToCorrectUrlWithCorrectHeaders_AndRespondsWithDeserializedXml()
        {
            var requestMedicationRequestModel = _fixture.Create<RequestMedication>();
            requestMedicationRequestModel.UnitId = UnitId;
            requestMedicationRequestModel.Uuid = Uuid;
            requestMedicationRequestModel.ApiVersion = ApiVersion;
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

            var response = await _systemUnderTest.OrderPrescriptionsPost(tppUserSession, requestMedicationRequestModel);

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
            linkRequestModel.Uuid = Uuid;
            linkRequestModel.ApiVersion = ApiVersion;
            linkRequestModel.Application = CreateApplication();

            var expectedLinkAccountResponse = _fixture.Create<LinkAccountReply>();
            expectedLinkAccountResponse.ProviderId = _tppConfig.ApplicationProviderId;

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

            var response = await _systemUnderTest.LinkAccountPost(linkRequestModel);

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
            linkAccountRequestModel.Uuid = Uuid;
            linkAccountRequestModel.ApiVersion = ApiVersion;
            linkAccountRequestModel.Application = CreateApplication();

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

            var response = await _systemUnderTest.LinkAccountPost(linkAccountRequestModel);

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
            linkAccountRequestModel.Uuid = Uuid;
            linkAccountRequestModel.ApiVersion = ApiVersion;
            linkAccountRequestModel.Application = CreateApplication();

            var tppRequestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(TppClient.RequestTypeHeader, linkAccountRequestModel.RequestType)
            };

            _mockHttpHandler
                .WhenTpp(HttpMethod.Post, ApiUrl)
                .WithTppHeaders(tppRequestHeaders)
                .Respond(value);

            var response = await _systemUnderTest.LinkAccountPost(linkAccountRequestModel);

            response.StatusCode.Should().Be(value);
            response.HasSuccessResponse.Should().BeFalse();
        }

        [TestMethod]
        public async Task NhsUserPostRequest_ReturnsAddNhsUserReply_WhenValidlyRequested()
        {
            var addNhsUserRequestModel = _fixture.Create<AddNhsUserRequest>();
            addNhsUserRequestModel.OrganisationCode = UnitId;
            addNhsUserRequestModel.Uuid = Uuid;
            addNhsUserRequestModel.ApiVersion = ApiVersion;
            addNhsUserRequestModel.Application = CreateApplication();

            var expectedLinkAccountResponse = _fixture.Create<AddNhsUserResponse>();
            expectedLinkAccountResponse.ProviderId = _tppConfig.ApplicationProviderId;

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(TppClient.RequestTypeHeader, addNhsUserRequestModel.RequestType)
            };

            var responseHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(TppClient.ResponseSuidHeader, Suid)
            };
            
            var responseContent = new StringContent(expectedLinkAccountResponse.SerializeXml());
            
            _mockHttpHandler
                .WhenTpp(HttpMethod.Post, ApiUrl)
                .WithTppHeaders(requestHeaders)
                .WithContent(addNhsUserRequestModel.SerializeXml())
                .Respond(HttpStatusCode.OK, responseHeaders, responseContent);    

            var response = await _systemUnderTest.NhsUserPost(addNhsUserRequestModel);

            response.Body.Should().BeEquivalentTo(expectedLinkAccountResponse);
            response.Headers.Should().BeEquivalentTo(responseHeaders);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();
        }
        
        [TestMethod]
        public async Task NhsUserPostRequest_ReturnsErrorWithFalseSuccessCode_WhenResponseHasErrorInBody()
        {            
            var nhsAddUserRequestModel = _fixture.Create<AddNhsUserRequest>();
            nhsAddUserRequestModel.OrganisationCode = UnitId;
            nhsAddUserRequestModel.Uuid = Uuid;
            nhsAddUserRequestModel.ApiVersion = ApiVersion;
            nhsAddUserRequestModel.Application = CreateApplication();

            var expectedErrorResponse = _fixture.Create<Error>();

            var tppRequestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(TppClient.RequestTypeHeader, nhsAddUserRequestModel.RequestType)
            };
            
            _mockHttpHandler
                .WhenTpp(HttpMethod.Post, ApiUrl)
                .WithTppHeaders(tppRequestHeaders)
                .WithContent(nhsAddUserRequestModel.SerializeXml())
                .Respond(MediaType, expectedErrorResponse.SerializeXml());

            var response = await _systemUnderTest.NhsUserPost(nhsAddUserRequestModel);

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
        public async Task NhsUserPostRequest_ReturnsErrorWithSameStatusCode_WhenResponseIsHttpError(
            HttpStatusCode value)
        {
            var addNhsUserRequestModel = _fixture.Create<AddNhsUserRequest>();
            addNhsUserRequestModel.OrganisationCode = UnitId;
            addNhsUserRequestModel.Uuid = Uuid;
            addNhsUserRequestModel.ApiVersion = ApiVersion;
            addNhsUserRequestModel.Application = CreateApplication();

            var tppRequestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(TppClient.RequestTypeHeader, addNhsUserRequestModel.RequestType)
            };

            _mockHttpHandler
                .WhenTpp(HttpMethod.Post, ApiUrl)
                .WithTppHeaders(tppRequestHeaders)
                .Respond(value);

            var response = await _systemUnderTest.NhsUserPost(addNhsUserRequestModel);

            response.StatusCode.Should().Be(value);
            response.HasSuccessResponse.Should().BeFalse();
        }

        [TestMethod]
        public async Task LogoffPostRequest_ReturnsErrorWithFalseSuccessCode_WhenResponseHasErrorInBody()
        {            
            var logoffRequestModel = _fixture.Create<Logoff>();
            logoffRequestModel.UnitId = null;
            logoffRequestModel.Uuid = Uuid;
            logoffRequestModel.ApiVersion = ApiVersion;

            var expectedErrorResponse = _fixture.Create<Error>();

            var tppRequestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(TppClient.RequestTypeHeader, logoffRequestModel.RequestType)
            };

            var tppUserSession = _fixture.Create<TppUserSession>();
            
            _mockHttpHandler
                .WhenTpp(HttpMethod.Post, ApiUrl)
                .WithTppHeaders(tppRequestHeaders)
                .WithContent(logoffRequestModel.SerializeXml())
                .Respond(MediaType, expectedErrorResponse.SerializeXml());

            var response = await _systemUnderTest.LogoffPost(tppUserSession);

            response.ErrorResponse.Should().BeEquivalentTo(expectedErrorResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Should().BeNull();
            response.Headers.Should().BeNull();
        }

        [TestMethod]
        public async Task LogoffPostRequest_ReturnsLogoffReply_WhenGivenValidRequest()
        {            
            var logoffRequestModel = _fixture.Create<Logoff>();
            logoffRequestModel.UnitId = null;
            logoffRequestModel.Uuid = Uuid;
            logoffRequestModel.ApiVersion = ApiVersion;

            var expectedLogoffResponse = _fixture.Create<LogoffReply>();
            var responseContent = new StringContent(expectedLogoffResponse.SerializeXml());
            var tppUserSession = _fixture.Create<TppUserSession>();

            var tppRequestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(TppClient.RequestTypeHeader, logoffRequestModel.RequestType)
            };

            _mockHttpHandler
                .WhenTpp(HttpMethod.Post, ApiUrl)
                .WithTppHeaders(tppRequestHeaders)
                .WithContent(logoffRequestModel.SerializeXml())
                .Respond(HttpStatusCode.OK, responseContent);

            var response = await _systemUnderTest.LogoffPost(tppUserSession);

            response.Body.Should().BeEquivalentTo(expectedLogoffResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();
            response.Headers.Should().BeNull();
        }

        [TestMethod]
        public async Task AnyRequest_ThrowsUnauthorisedHttpResponseException_WhenResponseIndicatesNotAuthorised()
        {
            // Arrange
            var linkAccountRequestModel = _fixture.Create<LinkAccount>();
            linkAccountRequestModel.OrganisationCode = UnitId;
            linkAccountRequestModel.Uuid = Uuid;
            linkAccountRequestModel.ApiVersion = ApiVersion;
            linkAccountRequestModel.Application = CreateApplication();

            var expectedErrorResponse = new Error
            {
                ErrorCode = TppApiErrorCodes.NotAuthenticated,
            };

            var tppRequestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(TppClient.RequestTypeHeader, linkAccountRequestModel.RequestType)
            };

            _mockHttpHandler
                .WhenTpp(HttpMethod.Post, ApiUrl)
                .WithTppHeaders(tppRequestHeaders)
                .WithContent(linkAccountRequestModel.SerializeXml())
                .Respond(MediaType, expectedErrorResponse.SerializeXml());

            UnauthorisedGpSystemHttpRequestException caughtException = null;

            // Act
            try
            {
                await _systemUnderTest.LinkAccountPost(linkAccountRequestModel);
            }
            catch (UnauthorisedGpSystemHttpRequestException e)
            {
                caughtException = e;
            }

            // Assert
            Assert.IsNotNull(caughtException);
        }

        [TestMethod]
        public async Task RequestSystmOnlineMessagesPost_ReturnsRequestSystmOnlineMessagesReply_WhenValidlyRequested()
        {
            // Arrange
            var requestModel = _fixture.Create<RequestSystmOnlineMessages>();
            requestModel.UnitId = UnitId;
            requestModel.Uuid = Uuid;
            requestModel.ApiVersion = ApiVersion;

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(TppClient.RequestTypeHeader, requestModel.RequestType),
                new KeyValuePair<string, string>(TppClient.RequestSuidHeader, Suid)
            };

            var responseHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(TppClient.ResponseSuidHeader, Suid)
            };
            
            var expectedResponse = _fixture.Create<RequestSystmOnlineMessagesReply>();
            var responseContent = new StringContent(expectedResponse.SerializeXml());
            
            _mockHttpHandler
                .WhenTpp(HttpMethod.Post, ApiUrl)
                .WithTppHeaders(requestHeaders)
                .WithContent(requestModel.SerializeXml())
                .Respond(HttpStatusCode.OK, responseHeaders, responseContent);    

            // Act
            var response = await _systemUnderTest.RequestSystmOnlineMessages(requestModel, Suid);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.Headers.Should().BeEquivalentTo(responseHeaders);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();
        }

        [TestMethod]
        public async Task RequestSystmOnlineMessagesPost_ReturnsErrorWithFalseSuccessCode_WhenResponseHasErrorInBody()
        {
            // Arrange
            var requestModel = _fixture.Create<RequestSystmOnlineMessages>();
            requestModel.UnitId = UnitId;
            requestModel.Uuid = Uuid;
            requestModel.ApiVersion = ApiVersion;
            
            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(TppClient.RequestTypeHeader, requestModel.RequestType),
                new KeyValuePair<string, string>(TppClient.RequestSuidHeader, Suid)
            };

            var expectedErrorResponse = _fixture.Create<Error>();
            
            _mockHttpHandler
                .WhenTpp(HttpMethod.Post, ApiUrl)
                .WithTppHeaders(requestHeaders)
                .WithContent(requestModel.SerializeXml())
                .Respond(MediaType, expectedErrorResponse.SerializeXml());
            
            // Act
            var response = await _systemUnderTest.RequestSystmOnlineMessages(requestModel, Suid);
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
        public async Task RequestSystmOnlineMessagesPost_ReturnsErrorWithSameStatusCode_WhenResponseIsHttpError(
            HttpStatusCode value)
        {
            var requestModel = _fixture.Create<RequestSystmOnlineMessages>();
            requestModel.UnitId = UnitId;
            requestModel.Uuid = Uuid;
            requestModel.ApiVersion = ApiVersion;

            var tppRequestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(TppClient.RequestTypeHeader, requestModel.RequestType),
                new KeyValuePair<string, string>(TppClient.RequestSuidHeader, Suid)
            };

            _mockHttpHandler
                .WhenTpp(HttpMethod.Post, ApiUrl)
                .WithTppHeaders(tppRequestHeaders)
                .Respond(value);

            var response = await _systemUnderTest.RequestSystmOnlineMessages(requestModel, Suid);

            response.StatusCode.Should().Be(value);
            response.HasSuccessResponse.Should().BeFalse();
        }
        
        [TestCleanup]
        public void Dispose()
        {
            _mockHttpHandler.Dispose();
        }

        private Application CreateApplication()
        {
            return new Application 
            {
                Name = _tppConfig.ApplicationName,
                Version = _tppConfig.ApplicationVersion,
                ProviderId = _tppConfig.ApplicationProviderId,
                DeviceType = _tppConfig.ApplicationDeviceType 
            };
        }
    }
}
