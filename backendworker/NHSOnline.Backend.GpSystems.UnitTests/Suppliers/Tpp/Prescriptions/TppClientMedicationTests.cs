using System;
using System.Collections.Generic;
using System.Linq;
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
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
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
        private ITppClientRequest<(TppRequestParameters, RepeatPrescriptionRequest), RequestMedicationReply> SystemUnderTest { get; set; }

        private MockHttpMessageHandler MockHttpHandler => Context.MockHttpHandler;
        private Mock<ILogger<TppClientRequestExecutor>> MockLogger => Context.MockLogger;
        
        [TestInitialize]
        public void TestInitialize()
        {
            Context = new TppClientTestsContext();
            Context.Initialise();
            SystemUnderTest = Context.ServiceProvider.GetRequiredService<ITppClientRequest<(TppRequestParameters, RepeatPrescriptionRequest), RequestMedicationReply>>();
        }

        [TestMethod]
        public async Task
            OrderPrescriptionPostRequest_MakesHttpRequestToCorrectUrlWithCorrectHeaders_AndRespondsWithDeserializedXml()
        {
            // Arrange
            var tppRequestParameters = new TppRequestParameters
            {
                OdsCode = TppClientTestsContext.UnitId,
                OnlineUserId = "onlineID",
                PatientId = "patientId",
                Suid = TppClientTestsContext.Suid
            };

            var repeatPrescriptionRequest = new RepeatPrescriptionRequest
            {
                CourseIds = new List<string> { "1234", "1234" },
                SpecialRequest = "test"
            };
            
            var requestMedicationRequestModel = new RequestMedication
            {
                UnitId = tppRequestParameters.OdsCode,
                Uuid = TppClientTestsContext.Uuid,
                ApiVersion = TppClientTestsContext.ApiVersion,
                PatientId = tppRequestParameters.PatientId,
                OnlineUserId = tppRequestParameters.OnlineUserId,
                Medications = repeatPrescriptionRequest.CourseIds.Select(x => new MedicationRequest
                {
                    DrugId = x,
                    Type = TppApiConstants.MedicationType.Repeat,
                }).ToList(),
                Notes = repeatPrescriptionRequest.SpecialRequest
            };

            var expectedMedicationResponse = new RequestMedicationReply();

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
            var response = await SystemUnderTest.Post((tppRequestParameters, repeatPrescriptionRequest));

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