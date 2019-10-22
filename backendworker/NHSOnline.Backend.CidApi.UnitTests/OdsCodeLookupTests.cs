using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.ServiceJourneyRules.Common;
using NHSOnline.Backend.ServiceJourneyRules.Common.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.CidApi.UnitTests
{
    [TestClass]
    public class OdsCodeLookupTests
    {
        private IFixture _fixture;
        private ILogger<OdsCodeLookup> _logger;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<IServiceJourneyRulesClient> _serviceJourneyRulesClient;
        private string _odsCode;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _odsCode = _fixture.Create<string>();
            _logger = Mock.Of<ILogger<OdsCodeLookup>>();
            _serviceJourneyRulesClient = _fixture.Freeze<Mock<IServiceJourneyRulesClient>>();
            
            _mockGpSystem = _fixture.Freeze<Mock<IGpSystem>>();

            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _mockGpSystemFactory
                .Setup(x => x.CreateGpSystem(Supplier.Emis))
                .Returns(_mockGpSystem.Object);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public async Task LookupSupplier_NullOrEmptyOdsCode_ReturnsOptionNone(string odsCode)
        {
            // Arrange
            var systemUnderTest = new OdsCodeLookup(_mockGpSystemFactory.Object, _logger, _serviceJourneyRulesClient.Object);
            
            // Act
            var result = await systemUnderTest.LookupSupplier(odsCode);

            // Assert
            result.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public async Task LookupSupplier_ServiceJourneyRulesReturnsNotFound_ReturnsOptionNone()
        {
            // Arrange
            _serviceJourneyRulesClient.Setup(x => x.GetServiceJourneyRules(_odsCode))
                .ReturnsAsync(new ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>(HttpStatusCode.NotFound));

            var systemUnderTest = new OdsCodeLookup(_mockGpSystemFactory.Object, _logger, _serviceJourneyRulesClient.Object);

            // Act
            var result = await systemUnderTest.LookupSupplier(_odsCode);
            
            // Assert
            result.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public async Task LookupSupplier_ServiceJourneyRulesReturnsUnknownSupplier_Throws()
        {
            // Arrange
            _serviceJourneyRulesClient.Setup(x => x.GetServiceJourneyRules(_odsCode))
                .ReturnsAsync(new ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>(HttpStatusCode.OK)
                {
                    Body = new ServiceJourneyRulesResponse { Journeys = new Journeys { Supplier = Supplier.Unknown } }
                });

            var systemUnderTest = new OdsCodeLookup(_mockGpSystemFactory.Object, _logger, _serviceJourneyRulesClient.Object);

            // Act
            var result = await systemUnderTest.LookupSupplier(_odsCode);
            
            // Assert
            result.HasValue.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(Supplier.Emis)]
        [DataRow(Supplier.Tpp)]
        public async Task LookupSupplier_ServiceJourneyRulesReturnsValidSupplier_ReturnsValue(Supplier supplier)
        {
            // Arrange
            _serviceJourneyRulesClient.Setup(x => x.GetServiceJourneyRules(_odsCode))
                .ReturnsAsync(new ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>(HttpStatusCode.OK)
                {
                    Body = new ServiceJourneyRulesResponse { Journeys = new Journeys { Supplier = supplier } }
                });

            var systemUnderTest = new OdsCodeLookup(_mockGpSystemFactory.Object, _logger, _serviceJourneyRulesClient.Object);

            var actual = await systemUnderTest.LookupSupplier(_odsCode);
            // Assert
            actual.HasValue.Should().BeTrue();
            actual.ValueOrFailure().Should().Be(supplier);
        }
    }
}