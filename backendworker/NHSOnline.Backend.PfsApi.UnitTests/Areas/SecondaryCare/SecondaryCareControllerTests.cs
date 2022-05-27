using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.SecondaryCare
{
    [TestClass]
    public sealed class SecondaryCareControllerTests
    {
        private SecondaryCareControllerTestContext Context { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Context = new SecondaryCareControllerTestContext();
        }

        [TestMethod]
        public async Task GetSummary_ApimOAuthTokenResponseIsUnsuccessful_Returns502BadGateway()
        {
            // Arrange
            Context.MockNhsApimHttpClientGetTokenReturnsUnsuccessfulResponse();

            // Act
            var result = await Context.CreateSystemUnderTest().Summary(Context.Data.P9UserSession);

            // Assert
            var actionResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            var pfsErrorResponse = actionResult.Value.Should().BeAssignableTo<PfsErrorResponse>().Subject;

            actionResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            pfsErrorResponse.ServiceDeskReference.Should().StartWith("4u");

            Context.Mocks.Auditor.Verify(a => a.PreOperationAudit(AuditingOperations.SecondaryCareGetSummaryRequest,"Attempting to get Secondary Care Summary"));
            Context.Mocks.Auditor.Verify(a => a.PostOperationAudit(AuditingOperations.SecondaryCareGetSummaryRequest, "Failed to get Auth token - response code: BadRequest"));
            Context.Mocks.Auditor.Verify(a => a.PostOperationAudit(AuditingOperations.SecondaryCareGetSummaryResponse, "Error retrieving Secondary Care Summary: BadGateway"));

            VerifyNoOtherLoggerCalls();
        }

        [TestMethod]
        public async Task GetSummary_GetSummaryResponseFromClientIsSuccessful_Returns200WithCompleteOrderedSummaryResponse()
        {
            // Arrange
            Context.MockNhsApimHttpClientGetTokenReturnsSuccessfulResponseWithAuthToken();
            Context.MockSecondaryCareHttpClientGetSummaryReturnsSuccessfulResponseWithData(LoadAggregatorResponse("complete-valid-secondary-care-summary-response"));

            // Act
            var result = await Context.CreateSystemUnderTest().Summary(Context.Data.P9UserSession);

            // Assert
            result.Should().BeAssignableTo<OkObjectResult>().Subject.Value.Should().BeEquivalentTo(Context.Data.SummaryResponse);

            Context.Mocks.Auditor.Verify(a => a.PreOperationAudit(AuditingOperations.SecondaryCareGetSummaryRequest,"Attempting to get Secondary Care Summary"));
            Context.Mocks.Auditor.Verify(a => a.PostOperationAudit(AuditingOperations.SecondaryCareGetSummaryResponse,"Secondary Care Summary successfully retrieved. Total Referrals: 6, Total Upcoming Appointments: 8"));

            VerifyNoOtherLoggerCalls();
        }

        [TestMethod]
        public async Task GetSummary_GetSummaryResponseFromClientIsUnsuccessful_Returns502BadGateway()
        {
            // Arrange
            Context.MockNhsApimHttpClientGetTokenReturnsSuccessfulResponseWithAuthToken();
            Context.MockSecondaryCareHttpClientGetSummaryReturnsUnsuccessfulResponse();

            // Act
            var result = await Context.CreateSystemUnderTest().Summary(Context.Data.P9UserSession);

            // Assert
            var actionResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            var pfsErrorResponse = actionResult.Value.Should().BeAssignableTo<PfsErrorResponse>().Subject;

            actionResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            pfsErrorResponse.ServiceDeskReference.Should().StartWith("4u");

            Context.Mocks.Auditor.Verify(a => a.PreOperationAudit(AuditingOperations.SecondaryCareGetSummaryRequest,"Attempting to get Secondary Care Summary"));
            Context.Mocks.Auditor.Verify(a => a.PostOperationAudit(AuditingOperations.SecondaryCareGetSummaryResult, "Failed - response code: BadRequest"));
            Context.Mocks.Auditor.Verify(a => a.PostOperationAudit(AuditingOperations.SecondaryCareGetSummaryResponse, "Error retrieving Secondary Care Summary: BadGateway"));

            VerifyNoOtherLoggerCalls();
        }

        [TestMethod]
        public async Task GetSummary_GetSummaryResponseFromClientTimesOut_Returns504GatewayTimeout()
        {
            // Arrange
            Context.MockNhsApimHttpClientGetTokenReturnsSuccessfulResponseWithAuthToken();
            Context.MockSecondaryCareHttpClientGetSummaryTimesOut();

            // Act
            var result = await Context.CreateSystemUnderTest().Summary(Context.Data.P9UserSession);

            // Assert
            var actionResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            var pfsErrorResponse = actionResult.Value.Should().BeAssignableTo<PfsErrorResponse>().Subject;

            actionResult.StatusCode.Should().Be(StatusCodes.Status504GatewayTimeout);
            pfsErrorResponse.ServiceDeskReference.Should().StartWith("zu");

            Context.Mocks.ServiceLogger.VerifyLogger(LogLevel.Error, "Aggregator Secondary Care Summary API timed out", Times.Once());
            Context.Mocks.Auditor.Verify(a => a.PreOperationAudit(AuditingOperations.SecondaryCareGetSummaryRequest,"Attempting to get Secondary Care Summary"));
            Context.Mocks.Auditor.Verify(a => a.PostOperationAudit(AuditingOperations.SecondaryCareGetSummaryResult, "Failed - request timed out"));
            Context.Mocks.Auditor.Verify(a => a.PostOperationAudit(AuditingOperations.SecondaryCareGetSummaryResponse, "Error retrieving Secondary Care Summary: Timeout"));

            VerifyNoOtherLoggerCalls();
        }

        [TestMethod]
        public async Task GetSummary_WhenPatientIsUnderMinimumAge_Returns470()
        {
            // Arrange
            Context.MockNhsApimHttpClientGetTokenReturnsSuccessfulResponseWithAuthToken();
            Context.MockSecondaryCareHttpClientGetSummaryReturnsResponseWithData(
                HttpStatusCode.Forbidden,
                LoadAggregatorResponse("under-minimum-age-response"));

            // Act
            var result = await Context.CreateSystemUnderTest().Summary(Context.Data.P9UserSession);

            // Assert
            var actionResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            actionResult.Value.Should().BeAssignableTo<PfsErrorResponse>();
            actionResult.StatusCode.Should().Be(Constants.CustomHttpStatusCodes.Status470FailedSecondaryCareMinimumAgeRequirement);

            Context.Mocks.ServiceLogger.VerifyLogger(LogLevel.Information, "Aggregator Secondary Care Summary API failed minimum age requirement", Times.Once());
            Context.Mocks.Auditor.Verify(a => a.PreOperationAudit(AuditingOperations.SecondaryCareGetSummaryRequest,"Attempting to get Secondary Care Summary"));
            Context.Mocks.Auditor.Verify(a => a.PostOperationAudit(AuditingOperations.SecondaryCareGetSummaryResponse, "Error retrieving Secondary Care Summary: FailedSecondaryCareMinimumAgeRequirement"));

            VerifyNoOtherLoggerCalls();
        }

        [TestMethod]
        [DynamicData(nameof(InvalidResponseData))]
        public async Task GetSummary_GetSummaryResponseHasMissingOrInvalidData_LogsErrorsAndReturns502(
            string response,
            List<string> mapperErrorMessages,
            List<string> serviceErrorMessages,
            List<string> auditMessages)
        {
            // Arrange
            Context.MockNhsApimHttpClientGetTokenReturnsSuccessfulResponseWithAuthToken();
            Context.MockSecondaryCareHttpClientGetSummaryReturnsSuccessfulResponseWithData(response);

            // Act
            var result = await Context.CreateSystemUnderTest().Summary(Context.Data.P9UserSession);

            // Assert
            var actionResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            var pfsErrorResponse = actionResult.Value.Should().BeAssignableTo<PfsErrorResponse>().Subject;

            actionResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            pfsErrorResponse.ServiceDeskReference.Should().StartWith("4u");

            foreach (var errorMessage in mapperErrorMessages)
            {
                Context.Mocks.SummaryMapperLogger.VerifyLogger(LogLevel.Error, errorMessage, Times.Once());
            }

            foreach (var errorMessage in serviceErrorMessages)
            {
                Context.Mocks.ServiceLogger.VerifyLogger(LogLevel.Error, errorMessage, Times.Once());
            }

            foreach (var auditMessage in auditMessages)
            {
                Context.Mocks.Auditor.Verify(a => a.PostOperationAudit(AuditingOperations.SecondaryCareGetSummaryResult,auditMessage));
            }

            Context.Mocks.Auditor.Verify(a => a.PreOperationAudit(AuditingOperations.SecondaryCareGetSummaryRequest,"Attempting to get Secondary Care Summary"));
            Context.Mocks.Auditor.Verify(a => a.PostOperationAudit(AuditingOperations.SecondaryCareGetSummaryResponse, "Error retrieving Secondary Care Summary: BadGateway"));

            VerifyNoOtherLoggerCalls();
        }

        private void VerifyNoOtherLoggerCalls()
        {
            Context.Mocks.SummaryMapperLogger.VerifyNoOtherCalls();
            Context.Mocks.ServiceLogger.VerifyNoOtherCalls();
            Context.Mocks.Auditor.VerifyNoOtherCalls();
        }

        private static IEnumerable<object[]> InvalidResponseData
        {
            get
            {
                yield return new object[]
                {
                    LoadAggregatorResponse("invalid-referral"),
                    new List<string>
                    {
                        "Failed to get Referral Booking Reference from Reference Identifier",
                        "Failed to get Referral Referred Date from Scheduled Period",
                        "Could not find Referral Organisation in list of Performers",
                        "Could not find Extension of type client-id",
                        "Expected Extension value to be of type Hl7.Fhir.Model.Code but was (null)",
                        "Failed to get Provider from extensions",
                        "Failed to get Deep Link from extensions",
                    },
                    new List<string>
                    {
                        "Aggregator Secondary Care Summary API unsuccessfully mapped Bundle to SummaryResponse. See previous log entries for more detail"
                    },
                    new List<string>
                    {
                        "Failed - mapping failed for at least one entry",
                    }
                };

                yield return new object[]
                {
                    LoadAggregatorResponse("invalid-upcoming-appointment"),
                    new List<string>
                    {
                        "Expected Extension value to be of type Hl7.Fhir.Model.Coding but was Hl7.Fhir.Model.Date",
                        "Failed to get Upcoming Appointment Status from extensions",
                        "Failed to get Upcoming Appointment Location from Description",
                    },
                    new List<string>
                    {
                        "Aggregator Secondary Care Summary API unsuccessfully mapped Bundle to SummaryResponse. See previous log entries for more detail"
                    },
                    new List<string>
                    {
                        "Failed - mapping failed for at least one entry",
                    }
                };

                yield return new object[]
                {
                    LoadAggregatorResponse("partial-success-secondary-care-summary-response"),
                    new List<string>(),
                    new List<string>
                    {
                        "Aggregator Secondary Care Summary API errors found in response: Reason: Response failed validation, Provider: company-1|Reason: HTTP-404 returned, Provider: company-2|Reason: Timeout occured, Provider: company-3"
                    },
                    new List<string>
                    {
                        "Failed - errors in response: Reason: Response failed validation, Provider: company-1|Reason: HTTP-404 returned, Provider: company-2|Reason: Timeout occured, Provider: company-3"
                    },
                };
            }
        }

        private static string LoadAggregatorResponse(string fileName)
            => File.ReadAllText($"MockData/SecondaryCare/{fileName}.json");
    }
}