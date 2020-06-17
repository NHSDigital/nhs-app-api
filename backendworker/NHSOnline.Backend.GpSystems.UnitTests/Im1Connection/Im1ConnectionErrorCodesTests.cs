using System;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Linkage;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Im1Connection
{
    [TestClass]
    public class Im1ConnectionErrorCodesTests
    {
        private ILogger<EmisLinkageService> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = fixture.Create<ILoggerFactory>().CreateLogger<EmisLinkageService>();
        }

        [TestMethod]
        public void InternalErrorResponses_Successful()
        {
            var systemUnderTest = new Im1ConnectionErrorCodes();
            var errorResponses = systemUnderTest.InternalErrorResponses;
            
            errorResponses.Should().NotBeNull();
            errorResponses.Count.Should().Be(42);
            const int singleCode = (int)Im1ConnectionErrorCodes.InternalCode.InvalidLinkageDetails;

            var specificResponse = errorResponses[singleCode];
            specificResponse.ErrorCode.Should().Be(singleCode);
            specificResponse.ErrorMessage.Should().Be("Invalid linkage details");
        }

        [TestMethod]
        public void ExternalErrorResponses_Successful()
        {
            var systemUnderTest = new Im1ConnectionErrorCodes();
            var errorResponses = systemUnderTest.ExternalErrorResponses;
            
            errorResponses.Should().NotBeNull();
            errorResponses.Count.Should().Be(20);

            const int singleCode = (int)Im1ConnectionErrorCodes.ExternalCode.InvalidDetails;

            var specificResponse = errorResponses[singleCode];
            specificResponse.ErrorCode.Should().Be(singleCode);
            specificResponse.ErrorMessage.Should().Be("Invalid Details");
        }

        [TestMethod]
        public void GetAndLogErrorResponse_Successful()
        {
            var systemUnderTest = new Im1ConnectionErrorCodes();
            var result = systemUnderTest.GetAndLogErrorResponse(
                Im1ConnectionErrorCodes.InternalCode.InvalidLinkageDetails,
                Supplier.Emis,
                _logger);

            result.Should().NotBeNull();
            result.ErrorCode.Should().Be((int)Im1ConnectionErrorCodes.ExternalCode.InvalidDetails);
            result.ErrorMessage.Should().Be("Invalid Details");
            result.GpSystem.Should().Be("Emis");
        }

        [TestMethod]
        public void GetExternalCode_AllResponses_Successful()
        {
            // Arrange
            var systemUnderTest = new Im1ConnectionErrorCodes();
            var allInternalCodes = EnumHelper.GetValues<Im1ConnectionErrorCodes.InternalCode>().ToList();

            var internalCodesToTest = allInternalCodes.Where(code=> code != Im1ConnectionErrorCodes.InternalCode.InvalidOption);

            foreach (var internalCode in internalCodesToTest)
            {
                var externalCode = systemUnderTest.GetExternalCode(internalCode);
                externalCode.Should().NotBeNull();
            }

            // Act
            Action act = () => systemUnderTest.GetExternalCode(Im1ConnectionErrorCodes.InternalCode.InvalidOption);
            
            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("InvalidOption is not a valid option");
        }
    }
}
