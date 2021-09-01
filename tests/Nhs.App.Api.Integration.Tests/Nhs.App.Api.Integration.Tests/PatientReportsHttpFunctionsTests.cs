using System;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    public class PatientReportsHttpFunctionsTests : CommunicationHttpFunctionBase
    {
        private static TestConfiguration _testConfiguration;

        [ClassInitialize]
        public static void ClassInitialise(TestContext context)
        {
            _testConfiguration = new TestConfiguration(context);
            TestClassSetup(_testConfiguration);
        }

        [TestMethod]
        public async Task CommunicationPost_InvalidBearerToken_Returns401Unauthorized()
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            var correlationId = Guid.NewGuid().ToString();

            // Act
            var response = await httpClient.PostAsync("communication/report/patients?ods-organisation-code=A12345&page=1", null, correlationId, "invalidAccessToken");

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
        public async Task PatientReportGet_BlankOdsCodeParameter_Returns400BadRequest()
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            // Act
            var response = await httpClient.GetAsync("communication/report/patients/?ods-organisation-code=&page=1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var operationOutcome = await ParseOperationOutcome(response);

            var issue = operationOutcome.Issue.Single();
            issue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            issue.Code.Should().Be(OperationOutcome.IssueType.Invalid);
            issue.Diagnostics.Should().Contain("Invalid format");
            issue.Expression.Single().Should().Be("ods-organisation-code");
        }

        [DataTestMethod]
        [DataRow("C12345", "1")]
        [DataRow("D12345", "1")]
        [DataRow("E12345", "15")]
        public async Task PatientReportGet_NoFileAvailableForRequestedOdsCode_Returns404NotFound(string odsCode,
            string pageParameter)
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            // Act
            var response =
                await httpClient.GetAsync($"communication/report/patients/?ods-organisation-code={odsCode}&page={pageParameter}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var operationOutcome = await ParseOperationOutcome(response);

            var issue = operationOutcome.Issue.Single();
            issue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            issue.Code.Should().Be(OperationOutcome.IssueType.NotFound);
            issue.Diagnostics.Should().Be("Report not found");
        }

        [TestMethod]
        public async Task PatientReportGet_MultipleOdsCodesRequested_Returns400BadRequest()
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            // Act
            var response =
                await httpClient.GetAsync("communication/report/patients/?ods-organisation-code=A12345&ods-organisation-code=A12355&page=1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var operationOutcome = await ParseOperationOutcome(response);

            var issue = operationOutcome.Issue.Single();
            issue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            issue.Code.Should().Be(OperationOutcome.IssueType.Invalid);
            issue.Expression.Single().Should().Be("ods-organisation-code");
            issue.Diagnostics.Should().Be("A single value is expected");
        }

        [TestMethod]
        public async Task PatientReportGet_MultiplePagesRequested_Returns400BadRequest()
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            // Act
            var response = await httpClient.GetAsync("communication/report/patients/?ods-organisation-code=A12345&page=1&page=2");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var operationOutcome = await ParseOperationOutcome(response);

            var issue = operationOutcome.Issue.Single();
            issue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            issue.Code.Should().Be(OperationOutcome.IssueType.Invalid);
            issue.Expression.Single().Should().Be("page");
            issue.Diagnostics.Should().Be("A single value is expected");
        }

        [DataTestMethod]
        [DataRow("C12345C", "1")]
        [DataRow("D123455", "1")]
        public async Task PatientReportGet_InvalidOdsCodeParameter_Returns400BadRequest(string odsCode,
            string pageParameter)
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            // Act
            var response =
                await httpClient.GetAsync($"communication/report/patients/?ods-organisation-code={odsCode}&page={pageParameter}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var operationOutcome = await ParseOperationOutcome(response);

            var issue = operationOutcome.Issue.Single();
            issue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            issue.Code.Should().Be(OperationOutcome.IssueType.Invalid);
            issue.Expression.Single().Should().Be("ods-organisation-code");
            issue.Diagnostics.Should().Be("Invalid format");
        }

        [TestMethod]
        public async Task PatientReportGet_ValidRequestForOdsCodeWhereSingleExists_ContentsAndLinkHeaderReturned()
        {
            // Arrange
            using var httpClient = CreateHttpClient();
            var expectedLinkHeader =
                $"<{httpClient.BaseAddress}{$"communication/report/patients?ods-organisation-code=A12345&page=1"}>; rel=\"last\"";
            const string expectedFileContents = @"[{""NhsNumber"": ""987654321"", ""NotificationsEnabled"": ""true""}]";

            // Act
            var response = await httpClient.GetAsync("communication/report/patients?ods-organisation-code=A12345&page=1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be(expectedFileContents);
            response.Headers.GetValues("Link").Single().Should().BeEquivalentTo(expectedLinkHeader);
        }

        [DataTestMethod]
        public async Task PatientReportGet_ValidRequestForOdsCodeWhereMultiplePagesExist_ContentsAndLinkHeaderReturned()
        {
            // Arrange
            using var httpClient = CreateHttpClient();
            var expectedLinkHeader =
                $"<{httpClient.BaseAddress}{"communication/report/patients?ods-organisation-code=B12345&page=2"}>; rel=\"next\"," +
                $"<{httpClient.BaseAddress}{"communication/report/patients?ods-organisation-code=B12345&page=2"}>; rel=\"last\"";
            const string expectedFileContents = @"[{""NhsNumber"": ""987654321"", ""NotificationsEnabled"": ""true""}]";

            // Act
            var response = await httpClient.GetAsync("communication/report/patients?ods-organisation-code=B12345&page=1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be(expectedFileContents);
            response.Headers.GetValues("Link").Single().Should().BeEquivalentTo(expectedLinkHeader);
        }

        [DataTestMethod]
        [DataRow("D12345", "A")]
        [DataRow("D12345", "-1")]
        [DataRow("D12345", "0")]
        [DataRow("D12345", "2147483648")]
        public async Task PatientReportGet_InvalidRequestInvalidPageValue_ReturnsBadRequest(string odsCode,
            string pageParameter)
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            // Act
            var response =
                await httpClient.GetAsync($"communication/report/patients?ods-organisation-code={odsCode}&page={pageParameter}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var operationOutcome = await ParseOperationOutcome(response);

            var issue = operationOutcome.Issue.Single();
            issue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            issue.Code.Should().Be(OperationOutcome.IssueType.Invalid);
            issue.Expression.Single().Should().Be("page");
            issue.Diagnostics.Should().Be("Invalid format");
        }

        [DataTestMethod]
        [DataRow("A12345", null)]
        [DataRow("A12345", "")]
        public async Task PatientReportGet_InvalidRequestNoPageValue_ReturnsBadRequest(string odsCode,
            string pageParameter)
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            // Act
            var response =
                await httpClient.GetAsync($"communication/report/patients?ods-organisation-code={odsCode}&page={pageParameter}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var operationOutcome = await ParseOperationOutcome(response);

            var issue = operationOutcome.Issue.Single();
            issue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            issue.Code.Should().Be(OperationOutcome.IssueType.Invalid);
            issue.Expression.Single().Should().Be("page");
            issue.Diagnostics.Should().Be("Invalid format");
        }

        [TestMethod]
        public async Task PatientReportGet_ValidRequestNoPageParameter_ReturnsOk()
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            // Act
            var response = await httpClient.GetAsync("communication/report/patients?ods-organisation-code=A12345");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task PatientReportGet_InvalidPageAndOdsCodeParameters_ReturnsBadRequest()
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            // Act
            var response = await httpClient.GetAsync("communication/report/patients?ods-organisation-code=abc&page=abc");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var operationOutcome = await ParseOperationOutcome(response);
            operationOutcome.Issue.Count.Should().Be(2);

            var odsCodeError = operationOutcome.Issue.Where(i => i.ExpressionElement.Where(e => e.Value == "ods-organisation-code").Any());
            var odsCodeIssue = odsCodeError.Single();
            odsCodeIssue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            odsCodeIssue.Code.Should().Be(OperationOutcome.IssueType.Invalid);
            odsCodeIssue.Expression.Single().Should().Be("ods-organisation-code");
            odsCodeIssue.Diagnostics.Should().Be("Invalid format");

            var pageError = operationOutcome.Issue.Where(i => i.ExpressionElement.Where(e => e.Value == "page").Any());
            var pageIssue = pageError.Single();
            pageIssue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            pageIssue.Code.Should().Be(OperationOutcome.IssueType.Invalid);
            pageIssue.Expression.Single().Should().Be("page");
            pageIssue.Diagnostics.Should().Be("Invalid format");
        }

        [TestMethod]
        public async Task EventReportGet_InvalidBearerToken_Returns401Unauthorized()
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            var correlationId = Guid.NewGuid().ToString();

            // Act
            var response = await httpClient.GetAsync("communication/report/patients?ods-organisation-code=A12345",
                correlationId, "invalidAccessToken");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var operationOutcome = await ParseOperationOutcome(response);

            var issue = operationOutcome.Issue.Single();
            issue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            issue.Code.Should().Be(OperationOutcome.IssueType.Forbidden);
            issue.Diagnostics.Should().Be("Invalid Access Token");

            response.Headers.ShouldContainHeader("X-Correlation-ID", correlationId);
        }

        private static async Task<OperationOutcome> ParseOperationOutcome(HttpResponseMessage response)
        {
            var responseString = await response.Content.ReadAsStringAsync();

            var operationOutcome = new FhirJsonParser().Parse<OperationOutcome>(responseString);

            return operationOutcome;
        }
    }
}
