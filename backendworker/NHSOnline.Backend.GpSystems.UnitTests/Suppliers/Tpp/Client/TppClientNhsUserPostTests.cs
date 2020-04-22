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
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Client
{
    [TestClass]
    public sealed class TppClientNhsUserPostTests : IDisposable
    {
        private TppClientTestsContext Context { get; set; }
        private ITppClientRequest<AddNhsUserRequest, AddNhsUserResponse> SystemUnderTest { get; set; }

        private MockHttpMessageHandler MockHttpHandler => Context.MockHttpHandler;
        private Mock<ILogger<TppClientRequestExecutor>> MockLogger => Context.MockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            Context = new TppClientTestsContext();
            Context.Initialise();
            SystemUnderTest = Context.ServiceProvider.GetRequiredService<ITppClientRequest<AddNhsUserRequest, AddNhsUserResponse>>();
        }

        [TestMethod]
        public async Task NhsUserPostRequest_ReturnsAddNhsUserReply_WhenValidlyRequested()
        {
            var addNhsUserRequestModel = new AddNhsUserRequest
            {
                OrganisationCode = TppClientTestsContext.UnitId,
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion,
                Application = TppClientTestsContext.CreateApplication()
            };

            var expectedLinkAccountResponse = new AddNhsUserResponse
            {
                ProviderId = TppClientTestsContext.ApplicationProviderId
            };

            var requestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.RequestTypeHeader, addNhsUserRequestModel.RequestType }
            };

            var responseHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.ResponseSuidHeader, TppClientTestsContext.Suid }
            };

            var responseContent = new StringContent(expectedLinkAccountResponse.SerializeXml());

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(requestHeaders)
                .WithContent(addNhsUserRequestModel.SerializeXml())
                .Respond(HttpStatusCode.OK, responseHeaders, responseContent);

            var response = await SystemUnderTest.Post(addNhsUserRequestModel);

            response.Body.Should().BeEquivalentTo(expectedLinkAccountResponse);
            response.Headers.Should().BeEquivalentTo(responseHeaders);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();

            Context.VerifyLogging(addNhsUserRequestModel);
        }

        [TestMethod]
        public async Task NhsUserPostRequest_ReturnsErrorWithFalseSuccessCode_WhenResponseHasErrorInBody()
        {
            // Arrange
            var nhsAddUserRequestModel = new AddNhsUserRequest
            {
                OrganisationCode = TppClientTestsContext.UnitId,
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion,
                Application = TppClientTestsContext.CreateApplication()
            };

            var errorResponseBuilder = new TppErrorResponseBuilder();

            var tppRequestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.RequestTypeHeader, nhsAddUserRequestModel.RequestType }
            };

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .WithContent(nhsAddUserRequestModel.SerializeXml())
                .Respond(TppClientTestsContext.MediaType, errorResponseBuilder.BuildXml());

            // Act
            var response = await SystemUnderTest.Post(nhsAddUserRequestModel);

            // Assert
            response.ErrorResponse.Should().BeEquivalentTo(errorResponseBuilder.BuildExpected());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Should().BeNull();
            response.Headers.Should().BeNull();

            Context.VerifyLogging(nhsAddUserRequestModel);
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
                OrganisationCode = TppClientTestsContext.UnitId,
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion,
                Application = TppClientTestsContext.CreateApplication()
            };

            var tppRequestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.RequestTypeHeader, addNhsUserRequestModel.RequestType }
            };

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .Respond(value);

            var response = await SystemUnderTest.Post(addNhsUserRequestModel);

            response.StatusCode.Should().Be(value);
            response.HasSuccessResponse.Should().BeFalse();

            Context.VerifyLogging(addNhsUserRequestModel);
        }

        [TestCleanup]
        public void Dispose()
        {
            Context.Dispose();
        }
    }
}