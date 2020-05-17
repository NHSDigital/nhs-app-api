using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.GpSearch;
using NHSOnline.Backend.PfsApi.GpSearch.Models;

namespace NHSOnline.Backend.PfsApi.UnitTests.GpSearch
{
    [TestClass]
    public class GpSearchServiceTests
    {
        private GpSearchService _gpSearchService;
        private ILogger<GpSearchService> _logger;
        private IFixture _fixture;
        private Mock<IGpLookupClient> _gpLookupClient;
        private IGpLookupConfig _gpLookupConfig;
        private INhsSearchResultChecker _nhsSearchResultChecker;
        private ILogger<NhsSearchResultChecker> _nhsResultCheckerLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = _fixture.Freeze<ILogger<GpSearchService>>();
            _gpLookupClient = new Mock<IGpLookupClient>();
            _gpLookupConfig = _fixture.Freeze<IGpLookupConfig>();
            _nhsResultCheckerLogger = _fixture.Freeze<ILogger<NhsSearchResultChecker>>();
            _nhsSearchResultChecker = new NhsSearchResultChecker(_nhsResultCheckerLogger);

            _gpSearchService = new GpSearchService(_logger, _gpLookupClient.Object, _gpLookupConfig, _nhsSearchResultChecker);
        }

        [TestMethod]
        public async Task IsGpPracticeEpsEnabled_WhenOdsCodeIsBlank_ReturnsBadRequestEnabledFalse()
        {
            // Act
            var result = await _gpSearchService.IsGpPracticeEPSEnabled(string.Empty);

            // Assert
            _gpLookupClient.Verify(x => x.OrganisationSearch(It.IsAny<OrganisationSearchData>()), Times.Never);
            result.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.IsGpEpsEnabled.Should().BeFalse();
        }

        [TestMethod]
        public async Task IsGpPracticeEpsEnabled_WhenNoGpPracticeReturned_ReturnsNotFoundAndEnabledFalse()
        {
            // Arrange
            const string odsCode = "AB234";

            var organisationReturnResult =
                new GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>(HttpStatusCode.OK)
                {
                    Body = new NhsOrganisationSearchResponse
                    {
                        Organisations = new List<Organisation>(),
                        OrganisationCount = 0,
                    }
                };

            _gpLookupClient.Setup(x => x.OrganisationSearch(It.IsAny<OrganisationSearchData>()))
                .Returns(Task.FromResult(organisationReturnResult))
                .Verifiable();

            // Act
            var result = await _gpSearchService.IsGpPracticeEPSEnabled(odsCode);

            // Assert
            _gpLookupClient.Verify();
            using (new AssertionScope())
            {
                result.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
                result.IsGpEpsEnabled.Should().BeFalse();
            }
        }

        [TestMethod]
        public async Task IsGpPracticeEpsEnabled_WhenGpPracticeDoesntHaveAnyMetrics_ReturnsFalse()
        {
            // Arrange
            const string odsCode = "AB234";

            var organisationReturnResults = new List<Organisation>
            {
                new Organisation { OrganisationName = "Test GP Practice" }
            };

            var organisationReturnResult =
                new GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>(HttpStatusCode.OK)
                {
                    Body = new NhsOrganisationSearchResponse
                    {
                        Organisations = organisationReturnResults,
                        OrganisationCount = 1,
                    }
                };

            _gpLookupClient.Setup(x => x.OrganisationSearch(It.IsAny<OrganisationSearchData>()))
                .Returns(Task.FromResult(organisationReturnResult))
                .Verifiable();

            // Act
            var result = await _gpSearchService.IsGpPracticeEPSEnabled(odsCode);

            // Assert
            _gpLookupClient.Verify();
            using (new AssertionScope())
            {
                result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
                result.IsGpEpsEnabled.Should().BeFalse();
            }
        }

        [TestMethod]
        public async Task IsGpPracticeEpsEnabled_WhenGpPracticeIsNotEpsEnabled_ReturnsFalse()
        {
            // Arrange
            const string odsCode = "AB234";
            const int metricIdOtherThanToEpsEnabled = 999;

            var organisationReturnResults = new List<Organisation>
            {
                new Organisation { OrganisationName = "Test GP Practice", Metrics = $"[{{\"MetricID\": {metricIdOtherThanToEpsEnabled} }}]" }
            };

            var organisationReturnResult =
                new GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>(HttpStatusCode.OK)
                {
                    Body = new NhsOrganisationSearchResponse
                    {
                        Organisations = organisationReturnResults,
                        OrganisationCount = 1,
                    }
                };

            _gpLookupClient.Setup(x => x.OrganisationSearch(It.IsAny<OrganisationSearchData>()))
                .Returns(Task.FromResult(organisationReturnResult))
                .Verifiable();

            // Act
            var result = await _gpSearchService.IsGpPracticeEPSEnabled(odsCode);

            // Assert
            _gpLookupClient.Verify();
            using (new AssertionScope())
            {
                result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
                result.IsGpEpsEnabled.Should().BeFalse();
            }
        }

        [TestMethod]
        public async Task IsGpPracticeEpsEnabled_WhenSuccessfulResponseWithEpsEnabled_ReturnsTrue()
        {
            // Arrange
            const string odsCode = "AB234";

            var organisationReturnResults = new List<Organisation>
            {
                new Organisation { OrganisationName = "Test GP Practice", Metrics = $"[{{\"MetricID\": {Constants.MetricIdForEPSEnabled} }}]" }
            };

            var organisationReturnResult =
                new GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>(HttpStatusCode.OK)
                {
                    Body = new NhsOrganisationSearchResponse
                    {
                        Organisations = organisationReturnResults,
                        OrganisationCount = 1,
                    }
                };

            _gpLookupClient.Setup(x => x.OrganisationSearch(It.Is<OrganisationSearchData>(
                o => o.Top == 1 &&
                string.Equals(o.Select, "OrganisationID,OrganisationName,NACSCode,Metrics", System.StringComparison.OrdinalIgnoreCase) &&
                string.Equals(o.Filter, $"OrganisationTypeID eq 'GPB' and NACSCode eq '{odsCode}'", System.StringComparison.OrdinalIgnoreCase))
                ))
                .Returns(Task.FromResult(organisationReturnResult))
                .Verifiable();

            // Act
            var result = await _gpSearchService.IsGpPracticeEPSEnabled(odsCode);

            // Assert
            _gpLookupClient.Verify();
            using (new AssertionScope())
            {
                result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
                result.IsGpEpsEnabled.Should().BeTrue();
            }
        }

        [TestMethod]
        public async Task GetGpPracticeByOdsCode_WhenFound_ReturnsGpPractice()
        {
            // Arrange
            const string odsCode = "AB234";
            const string orgName = "Test GP Practice";

            var organisationReturnResults = new List<Organisation>
            {
                new Organisation { OrganisationName = orgName },
            };

            var organisationReturnResult =
                new GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>(HttpStatusCode.OK)
                {
                    Body = new NhsOrganisationSearchResponse
                    {
                        Organisations = organisationReturnResults,
                        OrganisationCount = 1,
                    }
                };

            _gpLookupClient.Setup(x => x.OrganisationSearch(It.Is<OrganisationSearchData>(
                    o => o.Top == 1 &&
                         string.Equals(o.Select, "OrganisationID,OrganisationName,NACSCode,Metrics", System.StringComparison.OrdinalIgnoreCase) &&
                         string.Equals(o.Filter, $"OrganisationTypeID eq 'GPB' and NACSCode eq '{odsCode}'", System.StringComparison.OrdinalIgnoreCase))
                ))
                .Returns(Task.FromResult(organisationReturnResult))
                .Verifiable();

            // Act
            var result = await _gpSearchService.GetGpPracticeByOdsCode(odsCode);

            // Assert
            _gpLookupClient.Verify();
            var successResult = result.Should().BeOfType<GpSearchResult.Success>().Subject;
            successResult.Response.OrganisationQueryCount.Should().Be(1);
            successResult.Response.Organisations.Count.Should().Be(1);
            successResult.Response.Organisations[0].OrganisationName.Should().Be(orgName);
        }
    }
}