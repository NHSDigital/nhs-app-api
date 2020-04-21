using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging;
using NHSOnline.Backend.Support;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Client
{
    [TestClass]
    public sealed class TppClientMessageCreatePostTests : IDisposable
    {
        private IFixture _fixture;

        private TppClientTestsContext Context { get; set; }

        private ITppClientRequest<(TppUserSession tppRequestParameters, string recpientIdentifier, string messageBody),
            MessageCreateReply> SystemUnderTest { get; set; }

        private MockHttpMessageHandler MockHttpHandler => Context.MockHttpHandler;

        private TppUserSession _tppUserSession;


        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            Context = new TppClientTestsContext();
            Context.Initialise();

            _tppUserSession = _fixture.Create<TppUserSession>();

            SystemUnderTest = Context.ServiceProvider.GetRequiredService<
                      ITppClientRequest<(TppUserSession tppUserSession, string recipientIdentifier,
                          string messageText), MessageCreateReply>>();
        }

        [TestMethod]
        public async Task MessageCreatePostRequest_ReturnsMessageCreateReply_WhenValidlyRequested()
        {
            var messageCreateRequest = new MessageCreate
            {
                PatientId = _tppUserSession.PatientId,
                OnlineUserId = _tppUserSession.OnlineUserId,
                UnitId = _tppUserSession.OdsCode,
                Message = "test message",
                ApiVersion = TppClientTestsContext.ApiVersion,
                Uuid = TppClientTestsContext.Uuid,
                RecipientId = "1"
            };

            var expectedMessageCreateResponse = new MessageCreateReply
            {
                PatientId = _tppUserSession.PatientId,
                OnlineUserId = _tppUserSession.OnlineUserId,
                Uuid = TppClientTestsContext.Uuid.ToString()
            };

            var requestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.RequestTypeHeader, messageCreateRequest.RequestType }
            };

            var responseHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.ResponseSuidHeader, TppClientTestsContext.Suid }
            };

            var responseContent = new StringContent(expectedMessageCreateResponse.SerializeXml());

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(requestHeaders)
                .WithContent(messageCreateRequest.SerializeXml())
                .Respond(HttpStatusCode.OK, responseHeaders, responseContent);

            var response = await SystemUnderTest.Post((_tppUserSession, "1:Recipient", "test message"));

            response.Body.Should().BeEquivalentTo(expectedMessageCreateResponse);
            response.Headers.Should().BeEquivalentTo(responseHeaders);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();
        }

        [TestMethod]
        public async Task MessageCreatePostRequest_ReturnsMessageCreateReplyForUnitRecipient_WhenValidlyRequested()
        {
            // Arrange
            var messageCreateRequest = new MessageCreate
            {
                PatientId = _tppUserSession.PatientId,
                OnlineUserId = _tppUserSession.OnlineUserId,
                UnitId = _tppUserSession.OdsCode,
                UnitRecipientId = "1",
                Message = "To Unit recipient",
                ApiVersion = TppClientTestsContext.ApiVersion,
                Uuid = TppClientTestsContext.Uuid
            };

            var expectedMessageCreateResponse = new MessageCreateReply
            {
                PatientId = _tppUserSession.PatientId,
                OnlineUserId = _tppUserSession.OnlineUserId,
                Uuid = TppClientTestsContext.Uuid.ToString()
            };

            var tppRequestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.RequestTypeHeader, messageCreateRequest.RequestType }
            };

            var responseHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.ResponseSuidHeader, TppClientTestsContext.Suid }
            };

            var responseContent = new StringContent(expectedMessageCreateResponse.SerializeXml());

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .WithContent(messageCreateRequest.SerializeXml())
                .Respond(HttpStatusCode.OK, responseHeaders, responseContent);

            // Act
            var response = await SystemUnderTest.Post((_tppUserSession, "1:UnitRecipient", "To Unit recipient"));

            // Assert
            response.Body.Should().BeEquivalentTo(expectedMessageCreateResponse);
            response.Headers.Should().BeEquivalentTo(responseHeaders);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.Unauthorized)]
        [DataRow(HttpStatusCode.NotFound)]
        public async Task MessageCreatePostRequest_ReturnsErrorWithSameStatusCode_WhenResponseIsHttpError(
            HttpStatusCode value)
        {
            // Arrange
            var messageCreateRequest = new MessageCreate
            {
                PatientId = _tppUserSession.PatientId,
                OnlineUserId = _tppUserSession.OnlineUserId,
                UnitId = _tppUserSession.OdsCode,
                Message = "test message",
                RecipientId = "1",
                ApiVersion = TppClientTestsContext.ApiVersion,
                Uuid = TppClientTestsContext.Uuid
            };


            var tppRequestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.RequestTypeHeader, messageCreateRequest.RequestType }
            };

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(tppRequestHeaders)
                .Respond(value);

            var response = await SystemUnderTest.Post((_tppUserSession, "1:Recipient", "test"));

            response.StatusCode.Should().Be(value);
            response.HasSuccessResponse.Should().BeFalse();
        }

        [TestMethod]
        public async Task MessageCreate_ThrowsArgumentException_WhenInvalidRecipientIdentifier()
        {
            await SystemUnderTest.Awaiting(s => s.Post((_tppUserSession, "Invalid", "test")))
                .Should().ThrowAsync<ArgumentException>();
        }

        [TestCleanup]
        public void Dispose()
        {
            Context.Dispose();
        }
    }
}