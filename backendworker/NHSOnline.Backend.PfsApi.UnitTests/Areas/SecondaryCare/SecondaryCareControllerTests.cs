using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
        public async Task GetSummary_GetSummaryResponseFromClientIsSuccessful_Returns200WithCompleteOrderedSummaryResponse()
        {
            // Arrange
            Context.MockSecondaryCareHttpClientGetSummaryReturnsSuccessfulResponseWithData(LoadAggregatorResponse("complete-valid-secondary-care-summary-response"));

            // Act
            var result = await Context.CreateSystemUnderTest().Summary(Context.Data.P9UserSession);

            // Assert
            result.Should().BeAssignableTo<OkObjectResult>().Subject.Value.Should().BeEquivalentTo(Context.Data.SummaryResponse);

            Context.Mocks.Auditor.Verify(a => a.PreOperationAudit("SecondaryCare_GetSummary_Request","Attempting to get Secondary Care Summary"));
            Context.Mocks.Auditor.Verify(a => a.PostOperationAudit("SecondaryCare_GetSummary_Response", "Secondary Care Summary successfully retrieved. Total Referrals: 6, Total Upcoming Appointments: 4"));

            Context.Mocks.SummaryMapperLogger.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetSummary_GetSummaryResponseFromClientIsUnsuccessful_Returns502BadGateway()
        {
            // Arrange
            Context.MockSecondaryCareHttpClientGetSummaryReturnsUnsuccessfulResponse();

            // Act
            var result = await Context.CreateSystemUnderTest().Summary(Context.Data.P9UserSession);

            // Assert
            var actionResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            var pfsErrorResponse = actionResult.Value.Should().BeAssignableTo<PfsErrorResponse>().Subject;

            actionResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            pfsErrorResponse.ServiceDeskReference.Should().StartWith("4u");

            Context.Mocks.Auditor.Verify(a => a.PreOperationAudit("SecondaryCare_GetSummary_Request","Attempting to get Secondary Care Summary"));
            Context.Mocks.Auditor.Verify(a => a.PostOperationAudit("SecondaryCare_GetSummary_Response", "Error retrieving Secondary Care Summary: BadGateway"));
        }

        [TestMethod]
        public async Task GetSummary_GetSummaryResponseFromClientTimesOut_Returns504GatewayTimeout()
        {
            // Arrange
            Context.MockSecondaryCareHttpClientGetSummaryTimesOut();

            // Act
            var result = await Context.CreateSystemUnderTest().Summary(Context.Data.P9UserSession);

            // Assert
            var actionResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            var pfsErrorResponse = actionResult.Value.Should().BeAssignableTo<PfsErrorResponse>().Subject;

            actionResult.StatusCode.Should().Be(StatusCodes.Status504GatewayTimeout);
            pfsErrorResponse.ServiceDeskReference.Should().StartWith("zu");

            Context.Mocks.Auditor.Verify(a => a.PreOperationAudit("SecondaryCare_GetSummary_Request","Attempting to get Secondary Care Summary"));
            Context.Mocks.Auditor.Verify(a => a.PostOperationAudit("SecondaryCare_GetSummary_Response", "Error retrieving Secondary Care Summary: Timeout"));
        }

        [TestMethod]
        [DynamicData(nameof(InvalidResponseData))]
        public async Task GetSummary_GetSummaryResponseHasMissingOrInvalidData_LogsErrorsAndReturns502(string response, List<string> errorMessages)
        {
            Context.MockSecondaryCareHttpClientGetSummaryReturnsSuccessfulResponseWithData(response);

            var result = await Context.CreateSystemUnderTest().Summary(Context.Data.P9UserSession);

            var actionResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            var pfsErrorResponse = actionResult.Value.Should().BeAssignableTo<PfsErrorResponse>().Subject;

            actionResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            pfsErrorResponse.ServiceDeskReference.Should().StartWith("4u");

            foreach (var errorMessage in errorMessages)
            {
                Context.Mocks.SummaryMapperLogger.VerifyLogger(LogLevel.Error, errorMessage, Times.Once());
            }
            Context.Mocks.SummaryMapperLogger.VerifyNoOtherCalls();
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
                        "Could not find Extension of type code",
                        "Expected Extension value to be of type Hl7.Fhir.Model.Code but was (null)",
                        "Failed to get Provider from extensions",
                        "Failed to get Deep Link from extensions",
                    },
                };

                yield return new object[]
                {
                    LoadAggregatorResponse("invalid-upcoming-appointment"),
                    new List<string>
                    {
                        "Expected Extension value to be of type Hl7.Fhir.Model.Coding but was Hl7.Fhir.Model.Date",
                        "Failed to get Upcoming Appointment Status from extensions",
                        "Failed to get Upcoming Appointment Location from Description",
                    }
                };
            }
        }

        private static string LoadAggregatorResponse(string fileName)
            => File.ReadAllText($"MockData/SecondaryCare/{fileName}.json");
    }
}