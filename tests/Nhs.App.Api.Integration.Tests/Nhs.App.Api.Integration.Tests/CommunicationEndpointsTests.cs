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
using Nhs.App.Api.Integration.Tests.Extensions;
using Task = System.Threading.Tasks.Task;

namespace Nhs.App.Api.Integration.Tests
{
    [TestClass]
    public class CommunicationEndpointsTests : HttpEndpointTestsBase
    {
        private static TestConfiguration _testConfiguration;

        private static readonly EndpointInfo InApp = new EndpointInfo
        {
            Path = "communication/in-app/FHIR/R4/CommunicationRequest",
            DisplayName = "In-App"
        };

        private static readonly EndpointInfo Notification = new EndpointInfo
        {
            Path = "communication/notification/FHIR/R4/CommunicationRequest",
            DisplayName = "Notification"
        };

        private enum PayloadContentKind
        {
            ContentString,
            ContentReference
        }

        public enum MessageContentKind
        {
            Normal,
            Questionnaire
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
                yield return new object[]{ InApp };

                yield return new object[]{ Notification };
            }
        }

        public static string EndpointInfoDisplayName(MethodInfo methodInfo, object[] data) =>
            $"{methodInfo.Name}({((EndpointInfo)data[0]).DisplayName})";

        [ClassInitialize]
        public static void ClassInitialise(TestContext context)
        {
            _testConfiguration = new TestConfiguration(context);
            TestClassSetup(_testConfiguration);
        }

        [TestMethod]
        [DynamicData(nameof(Endpoints), DynamicDataDisplayName = nameof(EndpointInfoDisplayName))]
        public async Task CommunicationPost_ValidCommunicationRequestWithContentString_ReturnsCreatedStatusCode(EndpointInfo endpoint)
        {
            // Arrange
            var validPayload = BuildValidRequestBody(endpoint.DisplayName, PayloadContentKind.ContentString);

            await CommunicationPost_ValidTest(validPayload, endpoint.Path);
        }

        [TestMethod]
        [DynamicData(nameof(Endpoints), DynamicDataDisplayName = nameof(EndpointInfoDisplayName))]
        public async Task CommunicationPost_ValidCommunicationRequestWithContentReference_ReturnsCreatedStatusCode(EndpointInfo endpoint)
        {
            // Arrange
            var validPayload = BuildValidRequestBody(endpoint.DisplayName, PayloadContentKind.ContentReference);

            await CommunicationPost_ValidTest(validPayload, endpoint.Path);
        }

        [TestMethod]
        [DynamicData(nameof(Endpoints), DynamicDataDisplayName = nameof(EndpointInfoDisplayName))]
        public async Task CommunicationPost_InvalidBearerToken_Returns401Unauthorized(EndpointInfo endpoint)
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            var stringPayload = BuildValidRequestBody(endpoint.DisplayName);
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            var correlationId = Guid.NewGuid().ToString();

            // Act
            var response = await httpClient.PostAsync(endpoint.Path, httpContent, correlationId, "invalidAccessToken");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var operationOutcome = await ParseOperationOutcome(response);

            var issue = operationOutcome.Issue.Single();
            issue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            issue.Code.Should().Be(OperationOutcome.IssueType.Forbidden);
            issue.Diagnostics.Should().Be("Invalid Access Token");

            response.Headers.ShouldContainHeader("X-Correlation-ID", correlationId);
        }

        [TestMethod]
        [DynamicData(nameof(Endpoints), DynamicDataDisplayName = nameof(EndpointInfoDisplayName))]
        public async Task CommunicationPost_UnparseablePayload_Returns400BadRequest(EndpointInfo endpoint)
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            var httpContent = new StringContent("{ bad Json", Encoding.UTF8, "application/json");
            var correlationId = Guid.NewGuid().ToString();

            // Act
            var response = await httpClient.PostAsync(endpoint.Path, httpContent, correlationId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var operationOutcome = await ParseOperationOutcome(response);

            var issue = operationOutcome.Issue.Single();
            issue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            issue.Code.Should().Be(OperationOutcome.IssueType.Invalid);

            issue.Diagnostics.Should().Contain("Invalid Json encountered.");

            response.Headers.ShouldContainHeader("X-Correlation-ID", correlationId);
        }

        [TestMethod]
        [DynamicData(nameof(Endpoints), DynamicDataDisplayName = nameof(EndpointInfoDisplayName))]
        public async Task CommunicationPost_ValidJsonInvalidFhirResourceUnknownResourceType_Returns400BadRequest(EndpointInfo endpoint)
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            var stringPayload = BuildValidRequestBody(endpoint.DisplayName);
            stringPayload = stringPayload.Replace("\"CommunicationRequest\"", "\"UnknownType\"", StringComparison.OrdinalIgnoreCase);

            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            var correlationId = Guid.NewGuid().ToString();

            // Act
            var response = await httpClient.PostAsync(endpoint.Path, httpContent, correlationId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var operationOutcome = await ParseOperationOutcome(response);

            var issue = operationOutcome.Issue.Single();
            issue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            issue.Code.Should().Be(OperationOutcome.IssueType.Invalid);

            issue.Diagnostics.Should().Contain("Cannot locate type information for type 'UnknownType'");

            response.Headers.ShouldContainHeader("X-Correlation-ID", correlationId);
        }

        [TestMethod]
        [DynamicData(nameof(Endpoints), DynamicDataDisplayName = nameof(EndpointInfoDisplayName))]
        public async Task CommunicationPost_ValidJsonInvalidFhirResourceMissingResourceType_Returns400BadRequest(EndpointInfo endpoint)
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            var stringPayload = BuildValidRequestBody(endpoint.DisplayName);
            stringPayload = stringPayload.Replace("\"ResourceType\":", "", StringComparison.OrdinalIgnoreCase);
            stringPayload = stringPayload.Replace( "\"CommunicationRequest\",", "", StringComparison.OrdinalIgnoreCase);

            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            var correlationId = Guid.NewGuid().ToString();

            // Act
            var response = await httpClient.PostAsync(endpoint.Path, httpContent, correlationId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var operationOutcome = await ParseOperationOutcome(response);

            var issue = operationOutcome.Issue.Single();
            issue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            issue.Code.Should().Be(OperationOutcome.IssueType.Invalid);

            issue.Diagnostics.Should().Contain("Root object has no type indication (resourceType)");

            response.Headers.ShouldContainHeader("X-Correlation-ID", correlationId);
        }
        [TestMethod]
        public async Task CommunicationPost_ValidJsonInvalidFhirRequesterIdentifier_Returns400BadRequest()
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            var stringPayload = BuildValidRequestBody(InApp.DisplayName);
            stringPayload = stringPayload.Replace("https://fhir.nhs.uk/Id/ods-organization-code", "https://fhir.nhs.uk/Id/nhs-number", StringComparison.OrdinalIgnoreCase);

            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            var correlationId = Guid.NewGuid().ToString();

            // Act
            var response = await httpClient.PostAsync(InApp.Path, httpContent, correlationId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var operationOutcome = await ParseOperationOutcome(response);

            var issue = operationOutcome.Issue.Single();
            issue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            issue.Code.Should().Be(OperationOutcome.IssueType.Invalid);

            issue.Diagnostics.Should().Contain("Identifier system is invalid");

            response.Headers.ShouldContainHeader("X-Correlation-ID", correlationId);
        }
        [TestMethod]
        [DynamicData(nameof(Endpoints), DynamicDataDisplayName = nameof(EndpointInfoDisplayName))]
        public async Task CommunicationPost_NoCorrelationIdPassed_NoCorrelationIdHeaderInTheResponse(EndpointInfo endpoint)
        {
            // Arrange
            using var httpClient = CreateHttpClient();
            var stringPayload = BuildValidRequestBody(endpoint.DisplayName);
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            // Act
            var response = await httpClient.PostAsync(endpoint.Path, httpContent);

            // Assert
            response.Headers.ShouldNotContainHeader("X-Correlation-ID");
        }

        [TestMethod]
        public async Task CommunicationPost_ValidCommunicationRequestWithQuestionnaire_ReturnsCreatedStatusCode()
        {
            // Arrange
            var validPayload = BuildValidRequestBody(InApp.DisplayName, PayloadContentKind.ContentString, MessageContentKind.Questionnaire);

            //Act & Assert
            await CommunicationPost_ValidTest(validPayload, InApp.Path);
        }

        [TestMethod]
        public async Task CommunicationPost_ValidCommunicationRequestWithQuestionnaire_ForFreeTextReply_ReturnsCreatedStatusCode()
        {
            // Arrange
            var validPayload = BuildValidRequestBody(InApp.DisplayName, PayloadContentKind.ContentString,
                MessageContentKind.Questionnaire, Questionnaire.QuestionnaireItemType.Text);

            //Act & Assert
            await CommunicationPost_ValidTest(validPayload, InApp.Path);
        }

        [TestMethod]
        public async Task CommunicationPost_CommunicationRequestWithInvalidQuestionnaire_Returns400BadRequest()
        {
            // Arrange
            using var httpClient = CreateHttpClient();
            var payload = BuildInvalidQuestionnaireRequestBody(InApp.DisplayName);
            var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
            var correlationId = Guid.NewGuid().ToString();

            // Act
            var response = await httpClient.PostAsync(InApp.Path, httpContent, correlationId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var operationOutcome = await ParseOperationOutcome(response);
            var issue = operationOutcome.Issue.Single();
            issue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            issue.Code.Should().Be(OperationOutcome.IssueType.Invalid);
            issue.Diagnostics.Should().Contain("url is invalid");
        }

        [TestMethod]
        public async Task CommunicationPost_CommunicationRequestWithInvalidQuestionnaire_ForFreeTextReply_Returns400BadRequest()
        {
            // Arrange
            using var httpClient = CreateHttpClient();
            var payload = BuildInvalidQuestionnaireRequestBody(InApp.DisplayName,
                default,MessageContentKind.Questionnaire, Questionnaire.QuestionnaireItemType.Text);
            var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
            var correlationId = Guid.NewGuid().ToString();

            // Act
            var response = await httpClient.PostAsync(InApp.Path, httpContent, correlationId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var operationOutcome = await ParseOperationOutcome(response);
            var issue = operationOutcome.Issue.Single();
            issue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            issue.Code.Should().Be(OperationOutcome.IssueType.Invalid);
            issue.Diagnostics.Should().Contain("url is invalid");
        }
        private static async Task CommunicationPost_ValidTest(string validPayload, string endpointPath)
        {
            // Arrange, continued.
            using var httpClient = CreateHttpClient();

            var httpContent = new StringContent(validPayload, Encoding.UTF8, "application/json");
            var correlationId = Guid.NewGuid().ToString();

            // Act
            var response = await httpClient.PostAsync(endpointPath, httpContent, correlationId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseObject = await DeserializeFhirResponseAsync(response);
            var identifier = responseObject.Identifier
                .Should().ContainSingle(x => x.System == FhirR4IdentifierSystem.CommunicationId)
                .Subject;

            if (!Guid.TryParse(identifier.Value, out _))
            {
                Assert.Fail(
                    $"Expected the value of the identifier with system {FhirR4IdentifierSystem.CommunicationId} to be parseable as a GUID, but '{identifier.Value}' is not.");
            }

            response.Headers.Location.Should().Be($"{httpClient.BaseAddress}{endpointPath}/{identifier.Value}");
            response.Headers.ShouldContainHeader("X-Correlation-ID", correlationId);
        }


        private static string BuildValidRequestBody(string endpoint,
            PayloadContentKind payloadContentKind = PayloadContentKind.ContentString,
            MessageContentKind messageContentKind = MessageContentKind.Normal,
            Questionnaire.QuestionnaireItemType questionnaireItemType = Questionnaire.QuestionnaireItemType.Choice
        )
        {
            var communicationRequest = BuildValidCommunicationRequest(endpoint, payloadContentKind, messageContentKind, questionnaireItemType);

            return BuildRequestBody(communicationRequest);
        }


        private static string BuildInvalidQuestionnaireRequestBody(string endpoint,
            PayloadContentKind payloadContentKind = PayloadContentKind.ContentString,
            MessageContentKind messageContentKind = MessageContentKind.Questionnaire,
            Questionnaire.QuestionnaireItemType questionnaireItemType = Questionnaire.QuestionnaireItemType.Choice
        )
        {
            var communicationRequest = BuildValidCommunicationRequest(endpoint, payloadContentKind, messageContentKind,questionnaireItemType);
            communicationRequest.Extension[0].Url = "";
            return BuildRequestBody(communicationRequest);
        }


        private static string BuildRequestBody(CommunicationRequest communicationRequest)
        {
            var serializer = new FhirJsonSerializer();
            var json = serializer.SerializeToString(communicationRequest);

            return json;
        }

       private static CommunicationRequest BuildValidCommunicationRequest(string endpoint,
            PayloadContentKind payloadContentKind = PayloadContentKind.ContentString,
            MessageContentKind messageContentKind = MessageContentKind.Normal,
            Questionnaire.QuestionnaireItemType questionnaireItemType = Questionnaire.QuestionnaireItemType.Choice)
        {
            var communicationRequest = new CommunicationRequest
            {
                Status = RequestStatus.Active,
                Identifier = new List<Identifier>
                {
                    new(FhirR4IdentifierSystem.CampaignId, "Campaign ID 1"),
                    new(FhirR4IdentifierSystem.RequestReference, "Request Reference 1")
                },
                Recipient = BuildValidRecipient(),
                Payload = BuildValidPayload(payloadContentKind),
                Requester = endpoint.Equals(InApp.DisplayName, StringComparison.OrdinalIgnoreCase) ? BuildValidRequester() : null
            };

            if (messageContentKind == MessageContentKind.Questionnaire)
            {
                switch (questionnaireItemType)
                {
                    case Questionnaire.QuestionnaireItemType.Choice:
                        communicationRequest.Contained = new List<Resource>
                        {
                            new Questionnaire {
                                Status = PublicationStatus.Active,
                                Id="question",
                                Item = new List<Questionnaire.ItemComponent>
                                {
                                    new()
                                    {
                                        LinkId = "1",
                                        Type = Questionnaire.QuestionnaireItemType.Choice,
                                        AnswerOption = new List<Questionnaire.AnswerOptionComponent>
                                        {
                                            new()
                                            {
                                                Value = new Coding {Code = "YES"}
                                            },
                                            new()
                                            {
                                                Value = new Coding {Code = "NO"}
                                            }
                                        }
                                    }
                                }

                            }
                        };
                        break;
                    case Questionnaire.QuestionnaireItemType.Text:
                        communicationRequest.Contained = new List<Resource>
                        {
                            new Questionnaire {
                                Status = PublicationStatus.Active,
                                Id="question",
                                Item = new List<Questionnaire.ItemComponent>
                                {
                                    new()
                                    {
                                        LinkId = "1",
                                        Type = Questionnaire.QuestionnaireItemType.Text,
                                        AnswerOption = null
                                    }
                                }

                            }
                        };
                        break;
                }

                communicationRequest.Extension = new List<Extension>
                {
                    new()
                    {
                         Url= FhirR4IdentifierSystem.ExtensionQuestionnaireUrl,
                         Value = new ResourceReference("#question")
                    }

                };
            }

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
                new() { Content = content }
            };
        }

        private static List<ResourceReference> BuildValidRecipient()
        {
            var recipient = new List<ResourceReference>
            {
                new ResourceReference
                {
                    Identifier = new Identifier(FhirR4IdentifierSystem.NhsNumber, _testConfiguration.SendToNhsNumber),
                    Type = ResourceType.Patient.ToString()
                }
            };
            return recipient;
        }
        private static ResourceReference BuildValidRequester()
        {
            var requester = new ResourceReference
            {
                Identifier = new Identifier(FhirR4IdentifierSystem.Requester, _testConfiguration.OdsCode),
                Type = ResourceType.Organization.ToString(),
                Display = _testConfiguration.RequesterDisplay
            };
            return requester;
        }
        private static async Task<CommunicationRequest> DeserializeFhirResponseAsync(HttpResponseMessage response)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = new FhirJsonParser().Parse(responseString) as CommunicationRequest;

            return responseObject;
        }
    }
}
