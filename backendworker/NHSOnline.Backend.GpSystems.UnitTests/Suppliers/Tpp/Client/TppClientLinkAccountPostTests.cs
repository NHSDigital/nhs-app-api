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
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support;
using RichardSzalay.MockHttp;
using UnitTestHelper;
using Application = NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Application;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp
{
    [TestClass]
    public sealed class TppClientLinkAccountPostTests : IDisposable
    {
        private TppClientTestsContext Context { get; set; }
        private ITppClientRequest<LinkAccount, LinkAccountReply> SystemUnderTest { get; set; }

        private MockHttpMessageHandler MockHttpHandler => Context.MockHttpHandler;
        private Mock<ILogger<TppClientRequestExecutor>> MockLogger => Context.MockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            Context = new TppClientTestsContext();
            Context.Initialise();
            SystemUnderTest = Context.ServiceProvider.GetRequiredService<ITppClientRequest<LinkAccount, LinkAccountReply>>();
        }

        [TestMethod]
        public async Task LinkAccountPostRequest_ReturnsLinkAccountReply_WhenValidlyRequested()
        {
            // Arrange
            var linkRequestModel = new LinkAccount
            {
                OrganisationCode = TppClientTestsContext.UnitId,
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion,
                Application = TppClientTestsContext.CreateApplication()
            };

            var expectedLinkAccountResponse = new LinkAccountReply
            {
                ProviderId = TppClientTestsContext.ApplicationProviderId
            };

            var requestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.RequestTypeHeader, linkRequestModel.RequestType }
            };

            var responseHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.ResponseSuidHeader, TppClientTestsContext.Suid }
            };
            
            var responseContent = new StringContent(expectedLinkAccountResponse.SerializeXml());
            
            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(requestHeaders)
                .WithContent(linkRequestModel.SerializeXml())
                .Respond(HttpStatusCode.OK, responseHeaders, responseContent);    

            // Act
            var response = await SystemUnderTest.Post(linkRequestModel);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedLinkAccountResponse);
            response.Headers.Should().BeEquivalentTo(responseHeaders);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();

            Context.VerifyLogging(linkRequestModel);
        }
        
        [TestMethod]
        public async Task LinkAccountPostRequest_ReturnsErrorWithFalseSuccessCode_WhenResponseHasErrorInBody()
        {   
            // Arrange
            var linkAccountRequestModel = new LinkAccount
            {
                OrganisationCode = TppClientTestsContext.UnitId,
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion,
                Application = TppClientTestsContext.CreateApplication()
            };

            var errorResponseBuilder = new TppErrorResponseBuilder();

            var tppRequestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.RequestTypeHeader, linkAccountRequestModel.RequestType }
            };
            
            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .WithContent(linkAccountRequestModel.SerializeXml())
                .Respond(TppClientTestsContext.MediaType, errorResponseBuilder.BuildXml());

            // Act
            var response = await SystemUnderTest.Post(linkAccountRequestModel);

            // Assert
            response.ErrorResponse.Should().BeEquivalentTo(errorResponseBuilder.BuildExpected());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Should().BeNull();
            response.Headers.Should().BeNull();

            Context.VerifyLogging(linkAccountRequestModel);
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
                OrganisationCode = TppClientTestsContext.UnitId,
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion,
                Application = TppClientTestsContext.CreateApplication()
            };

            var tppRequestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.RequestTypeHeader, linkAccountRequestModel.RequestType }
            };

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .Respond(value);

            // Act
            var response = await SystemUnderTest.Post(linkAccountRequestModel);

            // Assert
            response.StatusCode.Should().Be(value);
            response.HasSuccessResponse.Should().BeFalse();

            Context.VerifyLogging(linkAccountRequestModel);
        }

        [TestMethod]
        public async Task LinkAccountPostRequest_ThrowsUnauthorisedHttpResponseException_WhenResponseIndicatesNotAuthorised()
        {
            // Arrange
            var linkAccountRequestModel = new LinkAccount
            {
                OrganisationCode = TppClientTestsContext.UnitId,
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion,
                Application = TppClientTestsContext.CreateApplication()
            };

            var errorResponseBuilder = new TppErrorResponseBuilder().ErrorCode("3");

            var tppRequestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.RequestTypeHeader, linkAccountRequestModel.RequestType }
            };

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .WithContent(linkAccountRequestModel.SerializeXml())
                .Respond(TppClientTestsContext.MediaType, errorResponseBuilder.BuildXml());

            // Act
            Func<Task> act = async () => { await SystemUnderTest.Post(linkAccountRequestModel); };

            // Assert
            await act.Should().ThrowAsync<UnauthorisedGpSystemHttpRequestException>();

            Context.VerifyLogging(linkAccountRequestModel);
        }

        [TestCleanup]
        public void Dispose()
        {
            Context.Dispose();
        }
    }
}