using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public async Task GetSummary_GetSummaryResponseFromClientIsSuccessful_Returns200WithSummaryResponse()
        {
            // Arrange
            Context.MockSecondaryCareHttpClientGetSummaryReturnsSuccessfulResponseWithData();

            // Act
            var result = await Context.CreateSystemUnderTest().Summary(Context.Data.P9UserSession);

            // Assert
            result.Should().BeAssignableTo<OkObjectResult>().Subject.Value.Should().BeEquivalentTo(Context.Data.SummaryResponse);

            Context.Mocks.Auditor.Verify(a => a.PreOperationAudit("SecondaryCare_GetSummary_Request","Attempting to get Secondary Care Summary"));
            Context.Mocks.Auditor.Verify(a => a.PostOperationAudit("SecondaryCare_GetSummary_Response","Secondary Care Summary successfully retrieved. Total Referrals: 3, Total Upcoming Appointments: 3"));
        }

        [TestMethod]
        public async Task GetSummary_GetSummaryResponseFromClientIsUnsuccessful_Returns502BadGateway()
        {
            // Arrange
            Context.MockSecondaryCareHttpClientGetSummaryReturnsUnsuccessfulResponse();

            // Act
            var result = await Context.CreateSystemUnderTest().Summary(Context.Data.P9UserSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>().Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);

            Context.Mocks.Auditor.Verify(a => a.PreOperationAudit("SecondaryCare_GetSummary_Request","Attempting to get Secondary Care Summary"));
            Context.Mocks.Auditor.Verify(a => a.PostOperationAudit("SecondaryCare_GetSummary_Response", "Error retrieving Secondary Care Summary: bad gateway"));
        }
    }
}