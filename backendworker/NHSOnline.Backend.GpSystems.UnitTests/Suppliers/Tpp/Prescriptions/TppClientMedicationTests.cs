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
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Prescriptions;
using NHSOnline.Backend.Support;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Prescriptions
{
    [TestClass]
    public sealed class TppClientMedicationPostTests : IDisposable
    {
        private TppClientTestsContext Context { get; set; }
        private ITppClientRequest<(TppUserSession, RequestMedication), RequestMedicationReply> SystemUnderTest { get; set; }

        private MockHttpMessageHandler MockHttpHandler => Context.MockHttpHandler;
        private Mock<ILogger<TppClientRequestExecutor>> MockLogger => Context.MockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            Context = new TppClientTestsContext();
            Context.Initialise();
            SystemUnderTest = Context.ServiceProvider.GetRequiredService<ITppClientRequest<(TppUserSession, RequestMedication), RequestMedicationReply>>();
        }

        [TestMethod]
        public async Task
            OrderPrescriptionPostRequest_MakesHttpRequestToCorrectUrlWithCorrectHeaders_AndRespondsWithDeserializedXml()
        {
            // Arrange
            var requestMedicationRequestModel = new RequestMedication
            {
                UnitId = TppClientTestsContext.UnitId,
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion
            };

            var expectedMedicationResponse = new RequestMedicationReply();

            var tppUserSession = new TppUserSession();

            var requestHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.RequestTypeHeader, requestMedicationRequestModel.RequestType }
            };

            var responseHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { TppClientTestsContext.ResponseSuidHeader, TppClientTestsContext.Suid }
            };

            var responseContent = new StringContent(expectedMedicationResponse.SerializeXml());

            MockHttpHandler
                .When(HttpMethod.Post, TppClientTestsContext.ApiUrl.ToString())
                .WithHeaders(requestHeaders)
                .WithContent(requestMedicationRequestModel.SerializeXml())
                .Respond(HttpStatusCode.OK, responseHeaders, responseContent);

            // Act
            var response = await SystemUnderTest.Post((tppUserSession, requestMedicationRequestModel));

            // Assert
            response.Body.Should().BeEquivalentTo(expectedMedicationResponse);
            response.Headers.Should().BeEquivalentTo(responseHeaders);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();

            Context.VerifyLogging(requestMedicationRequestModel);
        }

        [TestCleanup]
        public void Dispose()
        {
            Context.Dispose();
        }
    }
}