using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.BinaryData;
using NHSOnline.Backend.Support;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp
{
    [TestClass]
    public sealed class TppClientRequestBinaryDataPostTests : IDisposable
    {
        private IFixture _fixture;

        private TppClientTestsContext Context { get; set; }

        private ITppClientRequest<(TppRequestParameters tppRequestParameters, string documentIdentifier),
            RequestBinaryDataReply> SystemUnderTest { get; set; }

        private MockHttpMessageHandler MockHttpHandler => Context.MockHttpHandler;

        private TppRequestParameters _tppRequestParameters;


        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            Context = new TppClientTestsContext();
            Context.Initialise();

            _tppRequestParameters = _fixture.Create<TppRequestParameters>();

            SystemUnderTest = Context.ServiceProvider.GetRequiredService<
                ITppClientRequest<(TppRequestParameters tppUserSession, string documentIdentifier),
                RequestBinaryDataReply>>();
        }

        [TestMethod]
        public async Task RequestBinaryDataPostRequest_ReturnsBinaryDataReply_WhenValidlyRequested()
        {
            var binaryDataRequest = new RequestBinaryData
            {
                PatientId = _tppRequestParameters.PatientId,
                OnlineUserId = _tppRequestParameters.OnlineUserId,
                UnitId = _tppRequestParameters.OdsCode,
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

            var response = await SystemUnderTest.Post((_tppRequestParameters, "test"));

            response.Body.Should().BeEquivalentTo(expectedBinaryRequestResponse);
            response.Headers.Should().BeEquivalentTo(responseHeaders);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();
        }

        [TestMethod]
        public async Task RequestBinaryDataPostRequest_ReturnsErrorWithFalseSuccessCode_WhenResponseHasErrorInBody()
        {
            // Arrange
            var binaryDataRequest = new RequestBinaryData
            {
                PatientId = _tppRequestParameters.PatientId,
                OnlineUserId = _tppRequestParameters.OnlineUserId,
                UnitId = _tppRequestParameters.OdsCode,
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
            var response = await SystemUnderTest.Post((_tppRequestParameters, "test"));

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
            var binaryDataRequest = new RequestBinaryData
            {
                PatientId = _tppRequestParameters.PatientId,
                OnlineUserId = _tppRequestParameters.OnlineUserId,
                UnitId = _tppRequestParameters.OdsCode,
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

            var response = await SystemUnderTest.Post((_tppRequestParameters, "test"));

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