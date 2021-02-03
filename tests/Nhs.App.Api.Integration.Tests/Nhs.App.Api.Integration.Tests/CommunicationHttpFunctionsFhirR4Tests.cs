using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

        public class EndpointInfo
        {
            public string Path { get; set; }
            public string DisplayName { get; set; }
        }

        private static IEnumerable<object[]> Endpoints
        {
            get
            {
                yield return new object[]{ new EndpointInfo
                {
                    Path = "communication/in-app/FHIR/R4/CommunicationRequest",
                    DisplayName = "In-App"
                }};

                yield return new object[]{ new EndpointInfo
                {
                    Path = "communication/notification/FHIR/R4/CommunicationRequest",
                    DisplayName = "Notification"
                }};

                yield return new object[]{ new EndpointInfo
                {
                    Path = "communication/in-app-with-notification/FHIR/R4/CommunicationRequest",
                    DisplayName = "In-App With Notification"
                }};
            }
        }

        public static string EndpointInfoDisplayName(MethodInfo methodInfo, object[] data) =>
            $"{methodInfo.Name}({((EndpointInfo)data[0]).DisplayName})";

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
        [DynamicData(nameof(Endpoints), DynamicDataDisplayName = nameof(EndpointInfoDisplayName))]
        public async Task CommunicationFhirR4Post_ValidCommunicationRequestByNhsNumbersWithContentString_ReturnsCreatedStatusCode(EndpointInfo endpoint)
        {
            // Arrange
            var validPayload = BuildValidRequestBody(PayloadContentKind.ContentString, RecipientKind.NhsNumbers);

            await CommunicationFhirR4Post_ValidTest(validPayload, endpoint.Path);
        }

        [TestMethod]
        [DynamicData(nameof(Endpoints), DynamicDataDisplayName = nameof(EndpointInfoDisplayName))]
        public async Task
            CommunicationFhirR4Post_ValidCommunicationRequestByOdsCodeWithContentString_ReturnsCreatedStatusCode(EndpointInfo endpoint)
        {
            // Arrange
            var validPayload = BuildValidRequestBody(PayloadContentKind.ContentString, RecipientKind.OdsCode);

            await CommunicationFhirR4Post_ValidTest(validPayload, endpoint.Path);
        }

        [TestMethod]
        [DynamicData(nameof(Endpoints), DynamicDataDisplayName = nameof(EndpointInfoDisplayName))]
        public async Task CommunicationFhirR4Post_ValidCommunicationRequestByNhsNumbersWithContentReference_ReturnsCreatedStatusCode(EndpointInfo endpoint)
        {
            // Arrange
            var validPayload = BuildValidRequestBody(PayloadContentKind.ContentReference, RecipientKind.NhsNumbers);

            await CommunicationFhirR4Post_ValidTest(validPayload, endpoint.Path);
        }

        [TestMethod]
        [DynamicData(nameof(Endpoints), DynamicDataDisplayName = nameof(EndpointInfoDisplayName))]
        public async Task CommunicationFhirR4Post_ValidCommunicationRequestByOdsCodeWithContentReference_ReturnsCreatedStatusCode(EndpointInfo endpoint)
        {
            // Arrange
            var validPayload = BuildValidRequestBody(PayloadContentKind.ContentReference, RecipientKind.OdsCode);

            await CommunicationFhirR4Post_ValidTest(validPayload, endpoint.Path);
        }

        [TestMethod]
        [DynamicData(nameof(Endpoints), DynamicDataDisplayName = nameof(EndpointInfoDisplayName))]
        public async Task CommunicationFhirR4Post_InvalidApiKey_Returns401Unauthorized(EndpointInfo endpoint)
        {
            // Arrange
            using var httpClient = CreateHttpClient();
            httpClient.DefaultRequestHeaders.Remove("x-api-key");
            httpClient.DefaultRequestHeaders.Add("x-api-key", "invalid-key");

            var stringPayload = BuildValidRequestBody();
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            // Act
            var response = await httpClient.PostAsync(endpoint.Path, httpContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            // TODO - NHSO-11327 - enhance this test to check that the output is an OperationOutcome with appropriate properties.
        }

        [TestMethod]
        [DynamicData(nameof(Endpoints), DynamicDataDisplayName = nameof(EndpointInfoDisplayName))]
        public async Task CommunicationPost_UnparseablePayload_Returns400BadRequest(EndpointInfo endpoint)
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            var httpContent = new StringContent("{ bad Json", Encoding.UTF8, "application/json");

            // Act
            var response = await httpClient.PostAsync(endpoint.Path, httpContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var operationOutcome = await ParseOperationOutcome(response);

            var issue = operationOutcome.Issue.Single();
            issue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            issue.Code.Should().Be(OperationOutcome.IssueType.Invalid);

            issue.Diagnostics.Should().Contain("Invalid Json encountered.");
        }

        [TestMethod]
        [DynamicData(nameof(Endpoints), DynamicDataDisplayName = nameof(EndpointInfoDisplayName))]
        public async Task CommunicationPost_ValidJsonInvalidFhirResourceUnknownResourceType_Returns400BadRequest(EndpointInfo endpoint)
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            var stringPayload = BuildValidRequestBody();
            stringPayload = stringPayload.Replace("\"CommunicationRequest\"", "\"UnknownType\"", StringComparison.OrdinalIgnoreCase);

            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            // Act
            var response = await httpClient.PostAsync(endpoint.Path, httpContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var operationOutcome = await ParseOperationOutcome(response);

            var issue = operationOutcome.Issue.Single();
            issue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            issue.Code.Should().Be(OperationOutcome.IssueType.Invalid);

            issue.Diagnostics.Should().Contain("Cannot locate type information for type 'UnknownType'");
        }

        [TestMethod]
        [DynamicData(nameof(Endpoints), DynamicDataDisplayName = nameof(EndpointInfoDisplayName))]
        public async Task CommunicationPost_ValidJsonInvalidFhirResourceMissingResourceType_Returns400BadRequest(EndpointInfo endpoint)
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            var stringPayload = BuildValidRequestBody();
            stringPayload = stringPayload.Replace("\"ResourceType\":", "", StringComparison.OrdinalIgnoreCase);
            stringPayload = stringPayload.Replace( "\"CommunicationRequest\",", "", StringComparison.OrdinalIgnoreCase);

            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            // Act
            var response = await httpClient.PostAsync(endpoint.Path, httpContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var operationOutcome = await ParseOperationOutcome(response);

            var issue = operationOutcome.Issue.Single();
            issue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            issue.Code.Should().Be(OperationOutcome.IssueType.Invalid);

            issue.Diagnostics.Should().Contain("Root object has no type indication (resourceType)");
        }

        private static async Task CommunicationFhirR4Post_ValidTest(string validPayload, string endpointPath)
        {
            // Arrange, continued.
            using var httpClient = CreateHttpClient();

            var httpContent = new StringContent(validPayload, Encoding.UTF8, "application/json");

            // Act
            var response = await httpClient.PostAsync(endpointPath, httpContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var responseObject = await DeserializeResponseAsync<CommunicationPostResponse>(response);
            Guid.TryParse(responseObject.Id, out var communicationId).Should().BeTrue();

            response.Headers.Location.Should().Be($"{httpClient.BaseAddress}{endpointPath}/{communicationId}");
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
                Sender = new ResourceReference
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
            var recipients = new List<ResourceReference>();

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

        private static async Task<OperationOutcome> ParseOperationOutcome(HttpResponseMessage response)
        {
            var responseString = await response.Content.ReadAsStringAsync();

            var operationOutcome = new FhirJsonParser().Parse<OperationOutcome>(responseString);

            return operationOutcome;
        }
    }
}
