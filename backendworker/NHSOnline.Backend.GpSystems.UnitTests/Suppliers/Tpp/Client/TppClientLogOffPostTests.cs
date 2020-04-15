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
    public sealed class TppClientLogOffPostTests : IDisposable
    {
        private TppClientTestsContext Context { get; set; }
        private ITppClientRequest<TppRequestParameters, LogoffReply> SystemUnderTest { get; set; }

        private MockHttpMessageHandler MockHttpHandler => Context.MockHttpHandler;
        private Mock<ILogger<TppClientRequestExecutor>> MockLogger => Context.MockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            Context = new TppClientTestsContext();
            Context.Initialise();
            SystemUnderTest = Context.ServiceProvider.GetRequiredService<ITppClientRequest<TppRequestParameters, LogoffReply>>();
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
                { TppClientTestsContext.RequestTypeHeader, logoffRequestModel.RequestType },
                { TppClientTestsContext.RequestSuidHeader, TppClientTestsContext.Suid }
            };

            var tppRequestParameters = new TppRequestParameters
            {
                Suid = TppClientTestsContext.Suid
            };

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .WithContent(logoffRequestModel.SerializeXml())
                .Respond(TppClientTestsContext.MediaType, errorResponseBuilder.BuildXml());

            var response = await SystemUnderTest.Post(tppRequestParameters);

            response.ErrorResponse.Should().BeEquivalentTo(errorResponseBuilder.BuildExpected());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Should().BeNull();
            response.Headers.Should().BeNull();

            Context.VerifyLogging(logoffRequestModel);
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
            var tppRequestParameters = new TppRequestParameters
            {
                Suid = TppClientTestsContext.Suid
            };

            var tppRequestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.RequestTypeHeader, logoffRequestModel.RequestType },
                { TppClientTestsContext.RequestSuidHeader, TppClientTestsContext.Suid }
            };

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .WithContent(logoffRequestModel.SerializeXml())
                .Respond(HttpStatusCode.OK, responseContent);

            var response = await SystemUnderTest.Post(tppRequestParameters);

            response.Body.Should().BeEquivalentTo(expectedLogoffResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();
            response.Headers.Should().BeNull();

            Context.VerifyLogging(logoffRequestModel);
        }


        [TestCleanup]
        public void Dispose()
        {
            Context.Dispose();
        }
    }
}