using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Nhs.App.Api.Integration.Tests.Extensions;
using Task = System.Threading.Tasks.Task;

namespace Nhs.App.Api.Integration.Tests
{
    [TestClass]
    public class EventReportsHttpFunctionsTests : CommunicationHttpFunctionBase
    {
        private static TestConfiguration _testConfiguration;

        [ClassInitialize]
        public static void ClassInitialise(TestContext context)
        {
            _testConfiguration = new TestConfiguration(context);
            TestClassSetup(_testConfiguration);
        }

        [TestMethod]
        public async Task EventReportGet_NoFileAvailableForRequestedDate_Returns404NotFound()
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            // Act
            var response =
                await httpClient.GetAsync($"communication/report/events/?day=2001-01-01&page=1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var operationOutcome = await ParseOperationOutcome(response);

            var issue = operationOutcome.Issue.Single();
            issue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            issue.Code.Should().Be(OperationOutcome.IssueType.NotFound);
            issue.Diagnostics.Should().Be("Report not found");
        }

        [TestMethod]
        public async Task EventReportGet_MultipleDaysRequested_Returns400BadRequest()
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            // Act
            var response =
                await httpClient.GetAsync("communication/report/events/?day=2021-03-01&day=2021-03-02&page=1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var operationOutcome = await ParseOperationOutcome(response);

            var issue = operationOutcome.Issue.Single();
            issue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            issue.Code.Should().Be(OperationOutcome.IssueType.Invalid);
            issue.Expression.Single().Should().Be("day");
            issue.Diagnostics.Should().Be("A single value is expected");
        }

        [TestMethod]
        public async Task EventReportGet_MultiplePagesRequested_Returns400BadRequest()
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            // Act
            var response = await httpClient.GetAsync("communication/report/events/?day=2021-03-01&page=1&page=2");

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
        [DataRow("2021-02-30", "1")]
        [DataRow("2021-13-01", "1")]
        [DataRow("09-03-2021", "1")]
        [DataRow("2021-3-9", "1")]
        [DataRow("2021-03-08T01:00:00", "1")]
        [DataRow("yesterday", "1")]
        public async Task EventReportGet_InvalidDayParameter_Returns400BadRequest(string dayParameter,
            string pageParameter)
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            // Act
            var response =
                await httpClient.GetAsync($"communication/report/events/?day={dayParameter}&page={pageParameter}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var operationOutcome = await ParseOperationOutcome(response);

            var issue = operationOutcome.Issue.Single();
            issue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            issue.Code.Should().Be(OperationOutcome.IssueType.Invalid);
            issue.Expression.Single().Should().Be("day");
            issue.Diagnostics.Should().Be("Invalid format");
        }

        [DataTestMethod]
        [DataRow("2021-01-01", "1", @"{""Name"": ""Integration test file 1""}")]
        [DataRow("2021-01-02", "2", @"{""Name"": ""Integration test file 2""}")]
        public async Task EventReportGet_ValidRequestForDayWhereFileExists_ContentsOfFileAreReturned(
            string dayParameter, string pageParameter, string expectedFileContents)
        {
            // Arrange
            using var httpClient = CreateHttpClient();
            var path = $"communication/report/events?day={dayParameter}&page={pageParameter}";

            // Act
            var response = await httpClient.GetAsync(path);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be(expectedFileContents);
            response.Headers.GetValues("Link").Single().Should().BeEquivalentTo($"<{httpClient.BaseAddress}{path}>; rel=\"last\"");
        }

        [TestMethod]
        public async Task EventReportGet_NoCorrelationIdPassed_NoCorrelationIdHeaderInTheResponse()
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            // Act
            var response = await httpClient.GetAsync("communication/report/events/?day=2021-03-08");

            // Assert
            response.Headers.ShouldNotContainHeader("X-Correlation-ID");
        }

        [TestMethod]
        public async Task EventReportGet_ValidRequestForDayWhereSinglePageExists_ContentsAndLinkHeaderReturned()
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            // Act
            var response = await httpClient.GetAsync("communication/report/events?day=2021-01-01&page=1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be(@"{""Name"": ""Integration test file 1""}");
            response.Headers.GetValues("Link").Single().Should().BeEquivalentTo($"<{httpClient.BaseAddress}{$"communication/report/events?day=2021-01-01&page=1"}>; rel=\"last\"");
        }

        [TestMethod]
        public async Task EventReportGet_ValidRequestForDayWhereMultiplePagesExist_ContentsAndLinkHeaderReturned()
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            // Act
            var response = await httpClient.GetAsync("communication/report/events?day=2021-01-02&page=1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be(@"{""Name"": ""Integration test file 2""}");
            var expectedLinkHeader =
                $"<{httpClient.BaseAddress}{"communication/report/events?day=2021-01-02&page=2"}>; rel=\"next\", " +
                $"<{httpClient.BaseAddress}{"communication/report/events?day=2021-01-02&page=2"}>; rel=\"last\"";
            response.Headers.GetValues("Link").Single().Should().BeEquivalentTo(expectedLinkHeader);
        }

        [DataTestMethod]
        [DataRow("A")]
        [DataRow("-1")]
        [DataRow("0")]
        [DataRow("2147483648")]
        [DataRow("")]
        public async Task EventReportGet_InvalidRequestInvalidPageValue_ReturnsBadRequest(string pageParameter)
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            // Act
            var response =
                await httpClient.GetAsync($"communication/report/events?day=2021-01-01&page={pageParameter}");

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
        public async Task EventReportGet_InvalidRequestNoPageValue_ReturnsBadRequest()
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            // Act
            var response =
                await httpClient.GetAsync($"communication/report/events?day=2021-01-01&page=");

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
        public async Task EventReportGet_ValidRequestNoPageParameter_ReturnsOk()
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            // Act
            var response = await httpClient.GetAsync("communication/report/events?day=2021-01-01");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task EventReportGet_InvalidPageAndDateParameters_ReturnsBadRequest()
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            // Act
            var response = await httpClient.GetAsync("communication/report/events?day=abc&page=abc");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var operationOutcome = await ParseOperationOutcome(response);
            operationOutcome.Issue.Count.Should().Be(2);

            var dayError = operationOutcome.Issue.Where(i => i.ExpressionElement.Where(e => e.Value == "day").Any());
            var dayIssue = dayError.Single();
            dayIssue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            dayIssue.Code.Should().Be(OperationOutcome.IssueType.Invalid);
            dayIssue.Expression.Single().Should().Be("day");
            dayIssue.Diagnostics.Should().Be("Invalid format");

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
            var response = await httpClient.GetAsync("communication/report/events/?day=2021-03-08",
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
    }
}
