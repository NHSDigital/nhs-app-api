using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using FluentAssertions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Task = System.Threading.Tasks.Task;

namespace Nhs.App.Api.Integration.Tests
{
    [TestClass]
    public class CommunicationHttpFunctionsFhirR4Tests : CommunicationHttpFunctionBase
    {
        private static string[] _sendToNhsNumbers;
        private static string _sendToOdsCode;
        private const string InAppMessagePath = "communication/in-app/FHIR/R4/CommunicationRequest";

        private enum PayloadContentKind
        {
            ContentString,
            ContentReference
        }

        private enum RecipientKind
        {
            NhsNumbers,
            OdsCode
        }

        [ClassInitialize]
        public static void ClassInitialise(TestContext context)
        {
            TestClassSetup(context);

            _sendToNhsNumbers = context!.Properties["SendToNhsNumbers"]?.ToString()
                .Split(',')
                .Select(x => x.Trim())
                .ToArray();

            _sendToOdsCode = context!.Properties["SendToOdsCode"]?.ToString();
        }

        [TestMethod]
        public async Task CommunicationFhirR4InAppPost_ValidCommunicationRequestByNhsNumbersWithContentString_ReturnsCreatedStatusCode()
        {
            // Arrange
            var validPayload = BuildValidRequestBody(PayloadContentKind.ContentString, RecipientKind.NhsNumbers);

            await CommunicationFhirR4InAppPost_ValidTest(validPayload);
        }

        [TestMethod]
        public async Task CommunicationFhirR4InAppPost_ValidCommunicationRequestByOdsCodeWithContentString_ReturnsCreatedStatusCode()
        {
            // Arrange
            var validPayload = BuildValidRequestBody(PayloadContentKind.ContentString, RecipientKind.OdsCode);

            await CommunicationFhirR4InAppPost_ValidTest(validPayload);
        }

        [TestMethod]
        public async Task CommunicationFhirR4InAppPost_ValidCommunicationRequestByNhsNumbersWithContentReference_ReturnsCreatedStatusCode()
        {
            // Arrange
            var validPayload = BuildValidRequestBody(PayloadContentKind.ContentReference, RecipientKind.NhsNumbers);

            await CommunicationFhirR4InAppPost_ValidTest(validPayload);
        }

        [TestMethod]
        public async Task CommunicationFhirR4InAppPost_ValidCommunicationRequestByOdsCodeWithContentReference_ReturnsCreatedStatusCode()
        {
            // Arrange
            var validPayload = BuildValidRequestBody(PayloadContentKind.ContentReference, RecipientKind.OdsCode);

            await CommunicationFhirR4InAppPost_ValidTest(validPayload);
        }

        [TestMethod]
        public async Task CommunicationFhirR4InAppPost_InvalidApiKey_Returns401Unauthorized()
        {
            // Arrange
            using var httpClient = CreateHttpClient();
            httpClient.DefaultRequestHeaders.Remove("x-api-key");
            httpClient.DefaultRequestHeaders.Add("x-api-key", "invalid-key");

            var stringPayload = BuildValidRequestBody();
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            // Act
            var response = await httpClient.PostAsync(InAppMessagePath, httpContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        private static async Task CommunicationFhirR4InAppPost_ValidTest(string validPayload)
        {
            // Arrange, continued.
            using var httpClient = CreateHttpClient();

            var httpContent = new StringContent(validPayload, Encoding.UTF8, "application/json");

            // Act
            var response = await httpClient.PostAsync(InAppMessagePath, httpContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var responseObject = await DeserializeResponseAsync<CommunicationPostResponse>(response);
            Guid.TryParse(responseObject.Id, out var communicationId).Should().BeTrue();

            response.Headers.Location.Should().Be($"{httpClient.BaseAddress}{InAppMessagePath}/{communicationId}");
        }

        private static string BuildValidRequestBody(
            PayloadContentKind payloadContentKind = PayloadContentKind.ContentString,
            RecipientKind recipientKind = RecipientKind.NhsNumbers)
        {
            var communicationRequest = BuildValidCommunicationRequest(payloadContentKind, recipientKind);

            return BuildRequestBody(communicationRequest);
        }

        private static string BuildRequestBody(CommunicationRequest communicationRequest)
        {
            var serializer = new FhirJsonSerializer();
            var json = serializer.SerializeToString(communicationRequest);

            return json;
        }

        private static CommunicationRequest BuildValidCommunicationRequest(
            PayloadContentKind payloadContentKind = PayloadContentKind.ContentString,
            RecipientKind recipientKind = RecipientKind.NhsNumbers)
        {
            var communicationRequest = new CommunicationRequest
            {
                Status = RequestStatus.Active,
                Requester = new ResourceReference
                {
                    Display = "NHS App"
                },
                Identifier = new List<Identifier>
                {
                    new Identifier(FhirR4IdentifierSystem.CampaignId, "Campaign ID 1"),
                    new Identifier(FhirR4IdentifierSystem.RequestReference, "Request Reference 1")
                },
                Recipient = BuildValidRecipient(recipientKind),
                Payload = BuildValidPayload(payloadContentKind)
            };

            return communicationRequest;
        }

        private static List<CommunicationRequest.PayloadComponent> BuildValidPayload(PayloadContentKind payloadContentKind)
        {
            DataType content;

            if (payloadContentKind == PayloadContentKind.ContentReference)
            {
                content = new ResourceReference(
                    "https://www.nhsapp.service.nhs.uk/appointments",
                    "Communication body text 1");
            }
            else
            {
                content = new FhirString("Communication body text 2");
            }

            return new List<CommunicationRequest.PayloadComponent>
            {
                new CommunicationRequest.PayloadComponent { Content = content }
            };
        }

        private static List<ResourceReference> BuildValidRecipient(RecipientKind recipientKind)
        {
            List<ResourceReference> recipients = new List<ResourceReference>();

            if (recipientKind == RecipientKind.OdsCode)
            {
                recipients.Add(new ResourceReference
                {
                    Identifier = new Identifier(FhirR4IdentifierSystem.PatientsAtOdsCode, _sendToOdsCode),
                    Type = "group"
                });
            }
            else
            {
                recipients.AddRange(_sendToNhsNumbers.Select(nhsNumber => new ResourceReference
                {
                    Identifier = new Identifier(FhirR4IdentifierSystem.NhsNumber, nhsNumber),
                    Type = "patient"
                }));
            }

            return recipients;
        }
    }
}
