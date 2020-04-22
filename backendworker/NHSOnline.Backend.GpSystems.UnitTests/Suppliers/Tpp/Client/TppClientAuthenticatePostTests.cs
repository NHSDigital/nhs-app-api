using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet.Filters;
using RichardSzalay.MockHttp;
using Application = NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Application;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Client
{
    [TestClass]
    public sealed class TppClientAuthenticatePostTests : IDisposable
    {
        private TppClientTestsContext Context { get; set; }
        private ITppClientRequest<Authenticate, AuthenticateReply> SystemUnderTest { get; set; }

        private MockHttpMessageHandler MockHttpHandler => Context.MockHttpHandler;
        private Mock<ILogger<TppClientRequestExecutor>> MockLogger => Context.MockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            Context = new TppClientTestsContext();
            Context.Initialise();
            SystemUnderTest = Context.ServiceProvider.GetRequiredService<ITppClientRequest<Authenticate, AuthenticateReply>>();
        }

        [TestMethod]
        public async Task AuthenticatePostRequest_ReturnsAuthenticateReply_WhenValidlyRequested()
        {
            // Arrange
            var authenticateRequestModel = new Authenticate
            {
                UnitId = TppClientTestsContext.UnitId,
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
                { TppClientTestsContext.RequestTypeHeader, authenticateRequestModel.RequestType }
            };

            var responseHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.ResponseSuidHeader, TppClientTestsContext.Suid }
            };

            var responseContent = new StringContent(expectedAuthenticateResponse.SerializeXml());

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(requestHeaders)
                .WithContent(authenticateRequestModel.SerializeXml())
                .Respond(HttpStatusCode.OK, responseHeaders, responseContent);

            // Act
            var response = await SystemUnderTest.Post(authenticateRequestModel);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedAuthenticateResponse);
            response.Headers.Should().BeEquivalentTo(responseHeaders);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();
            Context.VerifyLogging(authenticateRequestModel);
        }

        [TestMethod]
        public async Task AuthenticatePostRequest_ReturnsNhsUnparsableException_WhenResponseIsIncomplete()
        {
            // Arrange
            var authenticateRequestModel = new Authenticate
            {
                UnitId = TppClientTestsContext.UnitId,
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
                { TppClientTestsContext.RequestTypeHeader, authenticateRequestModel.RequestType }
            };

            var responseHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.ResponseSuidHeader, TppClientTestsContext.Suid }
            };

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
            Func<Task> act = async () => { await SystemUnderTest.Post(authenticateRequestModel); };

            // Assert
            await act.Should().ThrowAsync<NhsUnparsableException>();
            Context.VerifyLogging(authenticateRequestModel);
        }

        [TestMethod]
        public async Task AuthenticatePostRequest_ReturnsErrorWithFalseSuccessCode_WhenResponseHasErrorInBody()
        {
            // Arrange
            var authenticateRequestModel = new Authenticate
            {
                UnitId = TppClientTestsContext.UnitId,
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
                { TppClientTestsContext.RequestTypeHeader, authenticateRequestModel.RequestType }
            };

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .WithContent(authenticateRequestModel.SerializeXml())
                .Respond(TppClientTestsContext.MediaType, errorResponseBuilder.BuildXml());

            // Act
            var response = await SystemUnderTest.Post(authenticateRequestModel);

            // Assert
            response.ErrorResponse.Should().BeEquivalentTo(errorResponseBuilder.BuildExpected());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Should().BeNull();
            response.Headers.Should().BeNull();

            Context.VerifyLogging(authenticateRequestModel);
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
                UnitId = TppClientTestsContext.UnitId,
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
                { TppClientTestsContext.RequestTypeHeader, authenticateRequestModel.RequestType }
            };

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .Respond(value);

            // Act
            var response = await SystemUnderTest.Post(authenticateRequestModel);

            // Assert
            response.StatusCode.Should().Be(value);
            response.HasSuccessResponse.Should().BeFalse();

            Context.VerifyLogging(authenticateRequestModel);
        }

        [TestCleanup]
        public void Dispose()
        {
            Context.Dispose();
        }
    }
}