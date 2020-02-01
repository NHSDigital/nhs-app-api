using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Prescriptions;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet.Filters;
using RichardSzalay.MockHttp;
using UnitTestHelper;
using Application = NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Application;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp
{
    [TestClass]
    public sealed class TppClientTests : IDisposable
    {
        private const string ResponseSuidHeader = "suid";
        private const string RequestTypeHeader = "type";
        private const string RequestSuidHeader = "suid";

        private const string UnitId = "unitId";

        private const string MediaType = "application/xml";
        private const string Suid = "session_id";
        private const string LoggingMessageTemplate = "Sending TPP REQUEST TYPE: {0} UUID: {1}";

        private TppClientTestsContext Context { get; set; }
        private ITppClient SystemUnderTest { get; set; }

        private MockHttpMessageHandler MockHttpHandler => Context.MockHttpHandler;
        private Mock<ILogger<TppClientRequestExecutor>> MockLogger => Context.MockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            Context = new TppClientTestsContext();
            Context.Initialise();
            SystemUnderTest = Context.ServiceProvider.GetRequiredService<ITppClient>();
        }

        [TestMethod]
        public async Task AuthenticatePostRequest_ReturnsAuthenticateReply_WhenValidlyRequested()
        {
            // Arrange
            var authenticateRequestModel = new Authenticate
            {
                UnitId = UnitId,
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion
            };
            authenticateRequestModel.Application = new Application
            {
                Name = TppClientTestsContext.ApplicationName,
                Version = TppClientTestsContext.ApplicationVersion,
                ProviderId = authenticateRequestModel.ProviderId,
                DeviceType = TppClientTestsContext.ApplicationDeviceType
            };

            var expectedAuthenticateResponse = new AuthenticateReply();

            var requestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { RequestTypeHeader, authenticateRequestModel.RequestType }
            };

            var responseHeaders = new Dictionary<string, string>(StringComparer.Ordinal) { { ResponseSuidHeader, Suid } };

            var responseContent = new StringContent(expectedAuthenticateResponse.SerializeXml());

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(requestHeaders)
                .WithContent(authenticateRequestModel.SerializeXml())
                .Respond(HttpStatusCode.OK, responseHeaders, responseContent);

            // Act
            var response = await SystemUnderTest.AuthenticatePost(authenticateRequestModel);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedAuthenticateResponse);
            response.Headers.Should().BeEquivalentTo(responseHeaders);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();
            VerifyLogging(authenticateRequestModel);
        }

        [TestMethod]
        public async Task AuthenticatePostRequest_ReturnsNhsUnparsableException_WhenResponseIsIncomplete()
        {
            // Arrange
            var authenticateRequestModel = new Authenticate
            {
                UnitId = UnitId,
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion
            };
            authenticateRequestModel.Application = new Application
            {
                Name = TppClientTestsContext.ApplicationName,
                Version = TppClientTestsContext.ApplicationVersion,
                ProviderId = authenticateRequestModel.ProviderId,
                DeviceType = TppClientTestsContext.ApplicationDeviceType
            };

            var expectedAuthenticateResponse = new AuthenticateReply();

            var requestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { RequestTypeHeader, authenticateRequestModel.RequestType }
            };

            var responseHeaders = new Dictionary<string, string>(StringComparer.Ordinal) { { ResponseSuidHeader, Suid } };

            var responseBody = expectedAuthenticateResponse.SerializeXml();
            // Deliberately screw up the XML, change uuid to be not a GUID so it won't deserialize
            responseBody = responseBody.Replace(expectedAuthenticateResponse.Uuid.ToString(), "Not A Guid",
                StringComparison.InvariantCulture);
            var responseContent = new StringContent(responseBody);

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(requestHeaders)
                .WithContent(authenticateRequestModel.SerializeXml())
                .Respond(HttpStatusCode.OK, responseHeaders, responseContent);

            // Act
            Func<Task> act = async () => { await SystemUnderTest.AuthenticatePost(authenticateRequestModel); };

            // Assert
            await act.Should().ThrowAsync<NhsUnparsableException>();
            VerifyLogging(authenticateRequestModel);
        }

        [TestMethod]
        public async Task AuthenticatePostRequest_ReturnsErrorWithFalseSuccessCode_WhenResponseHasErrorInBody()
        {
            // Arrange
            var authenticateRequestModel = new Authenticate
            {
                UnitId = UnitId,
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion
            };
            authenticateRequestModel.Application = new Application
            {
                Name = TppClientTestsContext.ApplicationName,
                Version = TppClientTestsContext.ApplicationVersion,
                ProviderId = authenticateRequestModel.ProviderId,
                DeviceType = TppClientTestsContext.ApplicationDeviceType
            };

            var errorResponseBuilder = new TppErrorResponseBuilder();

            var tppRequestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { RequestTypeHeader, authenticateRequestModel.RequestType }
            };

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .WithContent(authenticateRequestModel.SerializeXml())
                .Respond(MediaType, errorResponseBuilder.BuildXml());

            // Act
            var response = await SystemUnderTest.AuthenticatePost(authenticateRequestModel);
            
            // Assert
            response.ErrorResponse.Should().BeEquivalentTo(errorResponseBuilder.BuildExpected());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Should().BeNull();
            response.Headers.Should().BeNull();

            VerifyLogging(authenticateRequestModel);
        }

        [TestMethod]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.Unauthorized)]
        [DataRow(HttpStatusCode.NotFound)]
        public async Task AuthenticatePostRequest_ReturnsErrorWithSameStatusCode_WhenResponseIsHttpError(
            HttpStatusCode value)
        {
            // Arrange
            var authenticateRequestModel = new Authenticate
            {
                UnitId = UnitId,
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion
            };
            authenticateRequestModel.Application = new Application
            {
                Name = TppClientTestsContext.ApplicationName,
                Version = TppClientTestsContext.ApplicationVersion,
                ProviderId = authenticateRequestModel.ProviderId,
                DeviceType = TppClientTestsContext.ApplicationDeviceType
            };

            var tppRequestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { RequestTypeHeader, authenticateRequestModel.RequestType }
            };

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .Respond(value);

            // Act
            var response = await SystemUnderTest.AuthenticatePost(authenticateRequestModel);

            // Assert
            response.StatusCode.Should().Be(value);
            response.HasSuccessResponse.Should().BeFalse();

            VerifyLogging(authenticateRequestModel);
        }

        [TestMethod]
        public async Task
            OrderPrescriptionPostRequest_MakesHttpRequestToCorrectUrlWithCorrectHeaders_AndRespondsWithDeserializedXml()
        {
            // Arrange
            var requestMedicationRequestModel = new RequestMedication
            {
                UnitId = UnitId,
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion
            };

            var expectedMedicationResponse = new RequestMedicationReply();

            var tppUserSession = new TppUserSession();

            var requestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { RequestTypeHeader, requestMedicationRequestModel.RequestType }
            };

            var responseHeaders = new Dictionary<string, string>(StringComparer.Ordinal) { { ResponseSuidHeader, Suid } };

            var responseContent = new StringContent(expectedMedicationResponse.SerializeXml());

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(requestHeaders)
                .WithContent(requestMedicationRequestModel.SerializeXml())
                .Respond(HttpStatusCode.OK, responseHeaders, responseContent);

            // Act
            var response = await SystemUnderTest.OrderPrescriptionsPost(tppUserSession, requestMedicationRequestModel);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedMedicationResponse);
            response.Headers.Should().BeEquivalentTo(responseHeaders);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();

            VerifyLogging(requestMedicationRequestModel);
        }
        
        [TestMethod]
        public async Task LinkAccountPostRequest_ReturnsLinkAccountReply_WhenValidlyRequested()
        {
            // Arrange
            var linkRequestModel = new LinkAccount
            {
                OrganisationCode = UnitId,
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion,
                Application = CreateApplication()
            };

            var expectedLinkAccountResponse = new LinkAccountReply
            {
                ProviderId = TppClientTestsContext.ApplicationProviderId
            };

            var requestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { RequestTypeHeader, linkRequestModel.RequestType }
            };

            var responseHeaders = new Dictionary<string, string>(StringComparer.Ordinal) { { ResponseSuidHeader, Suid } };
            
            var responseContent = new StringContent(expectedLinkAccountResponse.SerializeXml());
            
            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(requestHeaders)
                .WithContent(linkRequestModel.SerializeXml())
                .Respond(HttpStatusCode.OK, responseHeaders, responseContent);    

            // Act
            var response = await SystemUnderTest.LinkAccountPost(linkRequestModel);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedLinkAccountResponse);
            response.Headers.Should().BeEquivalentTo(responseHeaders);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();

            VerifyLogging(linkRequestModel);
        }
        
        [TestMethod]
        public async Task LinkAccountPostRequest_ReturnsErrorWithFalseSuccessCode_WhenResponseHasErrorInBody()
        {   
            // Arrange
            var linkAccountRequestModel = new LinkAccount
            {
                OrganisationCode = UnitId,
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion,
                Application = CreateApplication()
            };

            var errorResponseBuilder = new TppErrorResponseBuilder();

            var tppRequestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { RequestTypeHeader, linkAccountRequestModel.RequestType }
            };
            
            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .WithContent(linkAccountRequestModel.SerializeXml())
                .Respond(MediaType, errorResponseBuilder.BuildXml());

            // Act
            var response = await SystemUnderTest.LinkAccountPost(linkAccountRequestModel);

            // Assert
            response.ErrorResponse.Should().BeEquivalentTo(errorResponseBuilder.BuildExpected());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Should().BeNull();
            response.Headers.Should().BeNull();

            VerifyLogging(linkAccountRequestModel);
        }

        [TestMethod]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.Unauthorized)]
        [DataRow(HttpStatusCode.NotFound)]
        public async Task LinkAccountPostRequest_ReturnsErrorWithSameStatusCode_WhenResponseIsHttpError(
            HttpStatusCode value)
        {
            // Arrange
            var linkAccountRequestModel = new LinkAccount
            {
                OrganisationCode = UnitId,
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion,
                Application = CreateApplication()
            };

            var tppRequestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { RequestTypeHeader, linkAccountRequestModel.RequestType }
            };

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .Respond(value);

            // Act
            var response = await SystemUnderTest.LinkAccountPost(linkAccountRequestModel);

            // Assert
            response.StatusCode.Should().Be(value);
            response.HasSuccessResponse.Should().BeFalse();

            VerifyLogging(linkAccountRequestModel);
        }

        [TestMethod]
        public async Task NhsUserPostRequest_ReturnsAddNhsUserReply_WhenValidlyRequested()
        {
            var addNhsUserRequestModel = new AddNhsUserRequest
            {
                OrganisationCode = UnitId,
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion,
                Application = CreateApplication()
            };

            var expectedLinkAccountResponse = new AddNhsUserResponse
            {
                ProviderId = TppClientTestsContext.ApplicationProviderId
            };

            var requestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { RequestTypeHeader, addNhsUserRequestModel.RequestType }
            };

            var responseHeaders = new Dictionary<string, string>(StringComparer.Ordinal) { { ResponseSuidHeader, Suid } };
            
            var responseContent = new StringContent(expectedLinkAccountResponse.SerializeXml());
            
            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(requestHeaders)
                .WithContent(addNhsUserRequestModel.SerializeXml())
                .Respond(HttpStatusCode.OK, responseHeaders, responseContent);    

            var response = await SystemUnderTest.NhsUserPost(addNhsUserRequestModel);

            response.Body.Should().BeEquivalentTo(expectedLinkAccountResponse);
            response.Headers.Should().BeEquivalentTo(responseHeaders);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();

            VerifyLogging(addNhsUserRequestModel);
        }
        
        [TestMethod]
        public async Task NhsUserPostRequest_ReturnsErrorWithFalseSuccessCode_WhenResponseHasErrorInBody()
        {   
            // Arrange
            var nhsAddUserRequestModel = new AddNhsUserRequest
            {
                OrganisationCode = UnitId,
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion,
                Application = CreateApplication()
            };

            var errorResponseBuilder = new TppErrorResponseBuilder();

            var tppRequestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { RequestTypeHeader, nhsAddUserRequestModel.RequestType }
            };
            
            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .WithContent(nhsAddUserRequestModel.SerializeXml())
                .Respond(MediaType, errorResponseBuilder.BuildXml());

            // Act
            var response = await SystemUnderTest.NhsUserPost(nhsAddUserRequestModel);

            // Assert
            response.ErrorResponse.Should().BeEquivalentTo(errorResponseBuilder.BuildExpected());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Should().BeNull();
            response.Headers.Should().BeNull();

            VerifyLogging(nhsAddUserRequestModel);
        }

        [TestMethod]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.Unauthorized)]
        [DataRow(HttpStatusCode.NotFound)]
        public async Task NhsUserPostRequest_ReturnsErrorWithSameStatusCode_WhenResponseIsHttpError(
            HttpStatusCode value)
        {
            var addNhsUserRequestModel = new AddNhsUserRequest
            {
                OrganisationCode = UnitId,
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion,
                Application = CreateApplication()
            };

            var tppRequestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { RequestTypeHeader, addNhsUserRequestModel.RequestType }
            };

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .Respond(value);

            var response = await SystemUnderTest.NhsUserPost(addNhsUserRequestModel);

            response.StatusCode.Should().Be(value);
            response.HasSuccessResponse.Should().BeFalse();

            VerifyLogging(addNhsUserRequestModel);
        }

        [TestMethod]
        public async Task LogoffPostRequest_ReturnsErrorWithFalseSuccessCode_WhenResponseHasErrorInBody()
        {
            var logoffRequestModel = new Logoff
            {
                UnitId = null,
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion
            };

            var errorResponseBuilder = new TppErrorResponseBuilder();

            var tppRequestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { RequestTypeHeader, logoffRequestModel.RequestType }
            };

            var tppUserSession = new TppUserSession();
            
            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .WithContent(logoffRequestModel.SerializeXml())
                .Respond(MediaType, errorResponseBuilder.BuildXml());

            var response = await SystemUnderTest.LogoffPost(tppUserSession);

            response.ErrorResponse.Should().BeEquivalentTo(errorResponseBuilder.BuildExpected());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Should().BeNull();
            response.Headers.Should().BeNull();

            VerifyLogging(logoffRequestModel);
        }

        [TestMethod]
        public async Task LogoffPostRequest_ReturnsLogoffReply_WhenGivenValidRequest()
        {
            var logoffRequestModel = new Logoff
            {
                UnitId = null,
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion
            };

            var expectedLogoffResponse = new LogoffReply();
            var responseContent = new StringContent(expectedLogoffResponse.SerializeXml());
            var tppUserSession = new TppUserSession();

            var tppRequestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { RequestTypeHeader, logoffRequestModel.RequestType }
            };

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .WithContent(logoffRequestModel.SerializeXml())
                .Respond(HttpStatusCode.OK, responseContent);

            var response = await SystemUnderTest.LogoffPost(tppUserSession);

            response.Body.Should().BeEquivalentTo(expectedLogoffResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();
            response.Headers.Should().BeNull();

            VerifyLogging(logoffRequestModel);
        }

        [TestMethod]
        public async Task AnyRequest_ThrowsUnauthorisedHttpResponseException_WhenResponseIndicatesNotAuthorised()
        {
            // Arrange
            var linkAccountRequestModel = new LinkAccount
            {
                OrganisationCode = UnitId,
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion,
                Application = CreateApplication()
            };

            var errorResponseBuilder = new TppErrorResponseBuilder().ErrorCode("3");

            var tppRequestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { RequestTypeHeader, linkAccountRequestModel.RequestType }
            };

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .WithContent(linkAccountRequestModel.SerializeXml())
                .Respond(MediaType, errorResponseBuilder.BuildXml());

            // Act
            Func<Task> act = async () => { await SystemUnderTest.LinkAccountPost(linkAccountRequestModel); };

            // Assert
            await act.Should().ThrowAsync<UnauthorisedGpSystemHttpRequestException>();

            VerifyLogging(linkAccountRequestModel);
        }

        [TestMethod]
        public async Task RequestSystmOnlineMessagesPost_ReturnsRequestSystmOnlineMessagesReply_WhenValidlyRequested()
        {
            // Arrange
            var requestModel = new RequestSystmOnlineMessages(new TppUserSession { OdsCode = UnitId })
            {
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion
            };

            var requestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { RequestTypeHeader, requestModel.RequestType },
                { RequestSuidHeader, Suid }
            };

            var responseHeaders = new Dictionary<string, string>(StringComparer.Ordinal) { { ResponseSuidHeader, Suid } };
            
            var expectedResponse = new RequestSystmOnlineMessagesReply();
            var responseContent = new StringContent(expectedResponse.SerializeXml());
            
            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(requestHeaders)
                .WithContent(requestModel.SerializeXml())
                .Respond(HttpStatusCode.OK, responseHeaders, responseContent);    

            // Act
            var response = await SystemUnderTest.RequestSystmOnlineMessages(requestModel, Suid);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.Headers.Should().BeEquivalentTo(responseHeaders);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();

            VerifyLogging(requestModel);
        }

        [TestMethod]
        public async Task RequestSystmOnlineMessagesPost_ReturnsErrorWithFalseSuccessCode_WhenResponseHasErrorInBody()
        {
            // Arrange
            var requestModel = new RequestSystmOnlineMessages(new TppUserSession { OdsCode = UnitId })
            {
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion
            };

            var requestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { RequestTypeHeader, requestModel.RequestType },
                { RequestSuidHeader, Suid }
            };

            var errorResponseBuilder = new TppErrorResponseBuilder();

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(requestHeaders)
                .WithContent(requestModel.SerializeXml())
                .Respond(MediaType, errorResponseBuilder.BuildXml());
            
            // Act
            var response = await SystemUnderTest.RequestSystmOnlineMessages(requestModel, Suid);
            
            // Assert
            response.ErrorResponse.Should().BeEquivalentTo(errorResponseBuilder.BuildExpected());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Should().BeNull();
            response.Headers.Should().BeNull();

            VerifyLogging(requestModel);
        }
        
        [TestMethod]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.Unauthorized)]
        [DataRow(HttpStatusCode.NotFound)]
        public async Task RequestSystmOnlineMessagesPost_ReturnsErrorWithSameStatusCode_WhenResponseIsHttpError(
            HttpStatusCode value)
        {
            // Act
            var requestModel = new RequestSystmOnlineMessages(new TppUserSession { OdsCode = UnitId })
            {
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion
            };

            var tppRequestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { RequestTypeHeader, requestModel.RequestType },
                { RequestSuidHeader, Suid }
            };

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .Respond(value);

            // Act
            var response = await SystemUnderTest.RequestSystmOnlineMessages(requestModel, Suid);

            // Assert
            response.StatusCode.Should().Be(value);
            response.HasSuccessResponse.Should().BeFalse();

            VerifyLogging(requestModel);
        }

        [TestCleanup]
        public void Dispose()
        {
            Context.Dispose();
        }

        private static Application CreateApplication()
        {
            return new Application 
            {
                Name = TppClientTestsContext.ApplicationName,
                Version = TppClientTestsContext.ApplicationVersion,
                ProviderId = TppClientTestsContext.ApplicationProviderId,
                DeviceType = TppClientTestsContext.ApplicationDeviceType 
            };
        }

        private void VerifyLogging(ITppRequest requestModel)
        {
            MockLogger.VerifyLogger(LogLevel.Information, string.Format(CultureInfo.InvariantCulture,
                LoggingMessageTemplate,
                requestModel.RequestType,
                requestModel.Uuid), Times.Once());
        }
    }
}
