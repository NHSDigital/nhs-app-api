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
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Client
{
    [TestClass]
    public sealed class TppClientRequestSystmOnlineMessagesTests :IDisposable
    {
        private TppClientTestsContext Context { get; set; }
        private ITppClientRequest<TppRequestParameters, RequestSystmOnlineMessagesReply> SystemUnderTest { get; set; }

        private MockHttpMessageHandler MockHttpHandler => Context.MockHttpHandler;
        private Mock<ILogger<TppClientRequestExecutor>> MockLogger => Context.MockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            Context = new TppClientTestsContext();
            Context.Initialise();
            SystemUnderTest = Context.ServiceProvider.GetRequiredService<ITppClientRequest<TppRequestParameters, RequestSystmOnlineMessagesReply>>();
        }

        [TestMethod]
        public async Task RequestSystmOnlineMessagesPost_ReturnsRequestSystmOnlineMessagesReply_WhenValidlyRequested()
        {
            // Arrange
            var requestModel = new RequestSystmOnlineMessages(new TppRequestParameters() { OdsCode = TppClientTestsContext.UnitId })
            {
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion,
            };

            var tppRequestParameters = new TppRequestParameters
            {
                OdsCode = TppClientTestsContext.UnitId,
                Suid = TppClientTestsContext.Suid
            };

            var requestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.RequestTypeHeader, requestModel.RequestType },
                { TppClientTestsContext.RequestSuidHeader, TppClientTestsContext.Suid }
            };

            var responseHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.ResponseSuidHeader, TppClientTestsContext.Suid }
            };

            var expectedResponse = new RequestSystmOnlineMessagesReply();
            var responseContent = new StringContent(expectedResponse.SerializeXml());

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(requestHeaders)
                .WithContent(requestModel.SerializeXml())
                .Respond(HttpStatusCode.OK, responseHeaders, responseContent);

            // Act
            var response = await SystemUnderTest.Post(tppRequestParameters);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.Headers.Should().BeEquivalentTo(responseHeaders);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();

            Context.VerifyLogging(requestModel);
        }

        [TestMethod]
        public async Task RequestSystmOnlineMessagesPost_ReturnsErrorWithFalseSuccessCode_WhenResponseHasErrorInBody()
        {
            // Arrange
            var tppRequestParameters = new TppRequestParameters
            {
                OdsCode = TppClientTestsContext.UnitId,
                Suid = TppClientTestsContext.Suid
            };

            var requestModel = new RequestSystmOnlineMessages(new TppRequestParameters { OdsCode = TppClientTestsContext.UnitId })
            {
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion,
            };

            var requestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.RequestTypeHeader, requestModel.RequestType },
                { TppClientTestsContext.RequestSuidHeader, TppClientTestsContext.Suid }
            };

            var errorResponseBuilder = new TppErrorResponseBuilder();

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(requestHeaders)
                .WithContent(requestModel.SerializeXml())
                .Respond(TppClientTestsContext.MediaType, errorResponseBuilder.BuildXml());

            // Act
            var response = await SystemUnderTest.Post(tppRequestParameters);

            // Assert
            response.ErrorResponse.Should().BeEquivalentTo(errorResponseBuilder.BuildExpected());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Should().BeNull();
            response.Headers.Should().BeNull();

            Context.VerifyLogging(requestModel);
        }

        [TestMethod]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.Unauthorized)]
        [DataRow(HttpStatusCode.NotFound)]
        public async Task RequestSystmOnlineMessagesPost_ReturnsErrorWithSameStatusCode_WhenResponseIsHttpError(
            HttpStatusCode value)
        {
            var tppRequestParameters = new TppRequestParameters
            {
                OdsCode = TppClientTestsContext.UnitId,
                Suid = TppClientTestsContext.Suid
            };

            // Act
            var requestModel = new RequestSystmOnlineMessages(new TppRequestParameters { OdsCode = TppClientTestsContext.UnitId })
            {
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion,
            };

            var tppRequestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.RequestTypeHeader, requestModel.RequestType },
                { TppClientTestsContext.RequestSuidHeader, TppClientTestsContext.Suid }
            };

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .Respond(value);

            // Act
            var response = await SystemUnderTest.Post(tppRequestParameters);

            // Assert
            response.StatusCode.Should().Be(value);
            response.HasSuccessResponse.Should().BeFalse();

            Context.VerifyLogging(requestModel);
        }

        [TestCleanup]
        public void Dispose()
        {
            Context.Dispose();
        }
    }
}