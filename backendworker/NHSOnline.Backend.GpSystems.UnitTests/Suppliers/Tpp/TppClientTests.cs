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
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.BinaryData;
using NHSOnline.Backend.Support;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp
{
    [TestClass]
    public sealed class TppClientTests : IDisposable
    {
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
        public async Task RequestBinaryDataPostRequest_ReturnsBinaryDataReply_WhenValidlyRequested()
        {

            var tppUserSession = new TppUserSession
            {
                PatientId = "1234",
                OnlineUserId = "12345",
                OdsCode = "1234"
            };

            var binaryDataRequest = new RequestBinaryData
            {
                PatientId = tppUserSession.PatientId,
                OnlineUserId = tppUserSession.OnlineUserId,
                UnitId = tppUserSession.UnitId,
                BinaryDataId = "test",
                ApiVersion = TppClientTestsContext.ApiVersion,
                Uuid = TppClientTestsContext.Uuid
            };

            var expectedBinaryRequestResponse = new RequestBinaryDataReply
            {
                BinaryData = new BinaryDataElement
                {
                    FileType = "jpg",
                    BinaryDataPage = new BinaryDataPage
                    {
                        BinaryData = "test",
                    }
                }
            };

            var requestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.RequestTypeHeader, binaryDataRequest.RequestType }
            };

            var responseHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.ResponseSuidHeader, TppClientTestsContext.Suid }
            };

            var responseContent = new StringContent(expectedBinaryRequestResponse.SerializeXml());

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(requestHeaders)
                .WithContent(binaryDataRequest.SerializeXml())
                .Respond(HttpStatusCode.OK, responseHeaders, responseContent);

            var response = await SystemUnderTest.RequestBinaryData("test", tppUserSession);

            response.Body.Should().BeEquivalentTo(expectedBinaryRequestResponse);
            response.Headers.Should().BeEquivalentTo(responseHeaders);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();
        }

        [TestMethod]
        public async Task RequestBinaryDataPostRequest_ReturnsErrorWithFalseSuccessCode_WhenResponseHasErrorInBody()
        {
            // Arrange
            var tppUserSession = new TppUserSession
            {
                PatientId = "1234",
                OnlineUserId = "12345",
                OdsCode = "1234"
            };

            var binaryDataRequest = new RequestBinaryData
            {
                PatientId = tppUserSession.PatientId,
                OnlineUserId = tppUserSession.OnlineUserId,
                UnitId = tppUserSession.UnitId,
                BinaryDataId = "test",
                ApiVersion = TppClientTestsContext.ApiVersion,
                Uuid = TppClientTestsContext.Uuid
            };


            var errorResponseBuilder = new TppErrorResponseBuilder();

            var tppRequestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.RequestTypeHeader, binaryDataRequest.RequestType }
            };

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .WithContent(binaryDataRequest.SerializeXml())
                .Respond(TppClientTestsContext.MediaType, errorResponseBuilder.BuildXml());

            // Act
            var response = await SystemUnderTest.RequestBinaryData("test", tppUserSession);

            // Assert
            response.ErrorResponse.Should().BeEquivalentTo(errorResponseBuilder.BuildExpected());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Should().BeNull();
            response.Headers.Should().BeNull();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.Unauthorized)]
        [DataRow(HttpStatusCode.NotFound)]
        public async Task RequestBinaryDataPostRequest_ReturnsErrorWithSameStatusCode_WhenResponseIsHttpError(
            HttpStatusCode value)
        {
            // Arrange
            var tppUserSession = new TppUserSession
            {
                PatientId = "1234",
                OnlineUserId = "12345",
                OdsCode = "1234"
            };

            var binaryDataRequest = new RequestBinaryData
            {
                PatientId = tppUserSession.PatientId,
                OnlineUserId = tppUserSession.OnlineUserId,
                UnitId = tppUserSession.UnitId,
                BinaryDataId = "test",
                ApiVersion = TppClientTestsContext.ApiVersion,
                Uuid = TppClientTestsContext.Uuid
            };


            var tppRequestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.RequestTypeHeader, binaryDataRequest.RequestType }
            };

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .Respond(value);

            var response = await SystemUnderTest.RequestBinaryData("test", tppUserSession);

            response.StatusCode.Should().Be(value);
            response.HasSuccessResponse.Should().BeFalse();
        }

        [TestCleanup]
        public void Dispose()
        {
            Context.Dispose();
        }
    }
}