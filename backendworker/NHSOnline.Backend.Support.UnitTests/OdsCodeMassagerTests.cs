using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitTestHelper;

namespace NHSOnline.Backend.Support.UnitTests
{
    [TestClass]
    public class OdsCodeMassagerTests
    {
        private IFixture _fixture;
        private Mock<ILogger<OdsCodeMassager>> _mockLogger;
        private Mock<IConfiguration> _mockConfiguration;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockLogger = _fixture.Freeze<Mock<ILogger<OdsCodeMassager>>>();
            _mockConfiguration = _fixture.Freeze<Mock<IConfiguration>>();
        }

        [DataTestMethod]
        [DataRow("G85075", "G85075")]    // default vision mapping
        [DataRow("G85672", "G85672")]    // default vision mapping
        [DataRow("C83615", "C83615")]    // non-default mapping
        [DataRow("Y00025", "Y00025")]    // non-default mapping
        public void CheckOdsCode_IsNotEnabled_AlwaysReturnsSameOdsCode(string inputOdsCode, string expectedOutputOdsCode)
        {
            // Arrange
            _mockConfiguration.Setup(x => x["ODS_REMAP_ENABLED"]).Returns("false");
            var _systemUnderTest = _fixture.Create<OdsCodeMassager>();

            // Act
            var outputOdsCode = _systemUnderTest.CheckOdsCode(inputOdsCode);

            // Assert
            outputOdsCode.Should().Be(expectedOutputOdsCode);
        }

        [DataTestMethod]
        [DataRow("G85075", "X00100")]    // default vision mapping
        [DataRow("G85672", "X00100")]    // default vision mapping
        [DataRow("C83615", "C83615")]    // non-default mapping, should be unchanged
        [DataRow("Y00025", "Y00025")]    // non-default mapping, should be unchanged
        public void CheckOdsCode_IsEnabledButNoMapConfigurationSpecified_ReturnsDefaultMappings(
            string inputOdsCode, string expectedOutputOdsCode)
        {
            // Arrange
            _mockConfiguration.Setup(x => x["ODS_REMAP_ENABLED"]).Returns("true");
            _mockConfiguration.Setup(x => x["ODS_REMAP_MAP"]).Returns((string)null);
            var _systemUnderTest = _fixture.Create<OdsCodeMassager>();

            // Act
            var outputOdsCode = _systemUnderTest.CheckOdsCode(inputOdsCode);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Warning,
                "", Times.Once());
            outputOdsCode.Should().Be(expectedOutputOdsCode);
        }

        [DataTestMethod]
        [DataRow("G85075", "X00100")]    // default vision mapping - uppercase
        [DataRow("g85075", "X00100")]    // default vision mapping - lowercase
        [DataRow("G85672", "X00100")]    // default vision mapping - uppercase
        [DataRow("g85672", "X00100")]    // default vision mapping - lowercase
        public void CheckOdsCode_IsEnabledButNoMapConfigurationSpecified_DefaultMappingsAreCaseInsensitive(
            string inputOdsCode, string expectedOutputOdsCode)
        {
            // Arrange
            _mockConfiguration.Setup(x => x["ODS_REMAP_ENABLED"]).Returns("true");
            _mockConfiguration.Setup(x => x["ODS_REMAP_MAP"]).Returns((string)null);
            var _systemUnderTest = _fixture.Create<OdsCodeMassager>();

            // Act
            var outputOdsCode = _systemUnderTest.CheckOdsCode(inputOdsCode);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Warning,
                "", Times.Once());
            outputOdsCode.Should().Be(expectedOutputOdsCode);
        }

        [DataTestMethod]
        [DataRow("9064d2e6-e1c6-4d93-bbac-8c1dad4df096", "G85075", "X00100")]    // default vision mapping
        [DataRow("9064d2e6-e1c6-4d93-bbac-8c1dad4df096", "G85672", "X00100")]    // default vision mapping
        [DataRow("9064d2e6-e1c6-4d93-bbac-8c1dad4df096", "C83615", "C83615")]    // non-default mapping, should be unchanged
        [DataRow("9064d2e6-e1c6-4d93-bbac-8c1dad4df096", "Y00025", "Y00025")]    // non-default mapping, should be unchanged
        [DataRow(" A12345:B34567;D98765:E76543", "A12345", "A12345")]    // unexpected whitespace in map
        [DataRow("A12345:B34567 ;D98765:E76543", "A12345", "A12345")]    // unexpected whitespace in map
        [DataRow("A12345:B34567; D98765:E76543", "D98765", "D98765")]    // unexpected whitespace in map
        [DataRow("A12345:B34567; D98765:E76543", "D98765", "D98765")]    // unexpected whitespace in map
        [DataRow("A12345:B34567;D9765:E76543", "A12345", "A12345")]    // ODS Code too short
        [DataRow("A12345:B34567;99765:E76543", "A12345", "A12345")]    // ODS Code invalid
        [DataRow("A12345:B34567;99765:E76543;", "A12345", "A12345")]    // No trailing semicolon
        public void CheckOdsCode_IsEnabledButMapInvalid_ReturnsDefaultMappings(
            string odsCodeMap, string inputOdsCode, string expectedOutputOdsCode)
        {
            // Arrange
            _mockConfiguration.Setup(x => x["ODS_REMAP_ENABLED"]).Returns("true");
            _mockConfiguration.Setup(x => x["ODS_REMAP_MAP"]).Returns(odsCodeMap);
            var _systemUnderTest = _fixture.Create<OdsCodeMassager>();

            // Act
            var outputOdsCode = _systemUnderTest.CheckOdsCode(inputOdsCode);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Warning,
                "Unable to parse environment variable ODS_REMAP_MAP - using default ODS Code map", Times.Once());
            outputOdsCode.Should().Be(expectedOutputOdsCode);
        }

        [DataTestMethod]
        [DataRow("A12345:B34567", "A12345", "B34567")]    // Mapped code is mapped
        [DataRow("A12345:B34567", "G85075", "G85075")]    // Non-mapped code is not mapped
        [DataRow("A12345:B34567;D98765:E76543", "A12345", "B34567")]    // mapped code is mapped
        [DataRow("A12345:B34567;D98765:E76543", "D98765", "E76543")]    // mapped code is mapped
        [DataRow("A12345:B34567;D98765:E76543", "G85075", "G85075")]    // Non-mapped code is not mapped
        [DataRow("A12345:B34567;D98765:E76543", "A12345", "B34567")]    // mapped code is mapped
        public void CheckOdsCode_IsEnabledAndValidMapSpecified_UsesSpecifiedMappings(
            string odsCodeMap, string inputOdsCode, string expectedOutputOdsCode)
        {
            // Arrange
            _mockConfiguration.Setup(x => x["ODS_REMAP_ENABLED"]).Returns("true");
            _mockConfiguration.Setup(x => x["ODS_REMAP_MAP"]).Returns(odsCodeMap);
            var _systemUnderTest = _fixture.Create<OdsCodeMassager>();

            // Act
            var outputOdsCode = _systemUnderTest.CheckOdsCode(inputOdsCode);

            // Assert
            outputOdsCode.Should().Be(expectedOutputOdsCode);
        }

        [DataTestMethod]
        [DataRow("A12345:B34567;D98765:E76543", "A12345", "B34567")]
        [DataRow("A12345:B34567;D98765:E76543", "a12345", "B34567")]
        [DataRow("A12345:B34567;D98765:E76543", "D98765", "E76543")]
        [DataRow("A12345:B34567;D98765:E76543", "d98765", "E76543")]
        public void CheckOdsCode_IsEnabledAndValidMapSpecified_MappingIsCaseInsensitive(
            string odsCodeMap, string inputOdsCode, string expectedOutputOdsCode)
        {
            // Arrange
            _mockConfiguration.Setup(x => x["ODS_REMAP_ENABLED"]).Returns("true");
            _mockConfiguration.Setup(x => x["ODS_REMAP_MAP"]).Returns(odsCodeMap);
            var _systemUnderTest = _fixture.Create<OdsCodeMassager>();

            // Act
            var outputOdsCode = _systemUnderTest.CheckOdsCode(inputOdsCode);

            // Assert
            outputOdsCode.Should().Be(expectedOutputOdsCode);
        }

        [DataTestMethod]
        [DataRow("A12345:B34567;D98765:E76543", "A12345", "B34567")]
        [DataRow("A12345:B34567;D98765:E76543", "D98765", "E76543")]
        public void CheckOdsCode_CodeIsMapped_LogsInformationalMessage(
            string odsCodeMap, string inputOdsCode, string expectedOutputOdsCode)
        {
            // Arrange
            _mockConfiguration.Setup(x => x["ODS_REMAP_ENABLED"]).Returns("true");
            _mockConfiguration.Setup(x => x["ODS_REMAP_MAP"]).Returns(odsCodeMap);
            var _systemUnderTest = _fixture.Create<OdsCodeMassager>();

            // Act
            var outputOdsCode = _systemUnderTest.CheckOdsCode(inputOdsCode);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Information, $"Massaging ODS Code {inputOdsCode} to {outputOdsCode}", Times.Once());
            outputOdsCode.Should().Be(expectedOutputOdsCode);
        }

        [DataTestMethod]
        [DataRow("A12345:B34567;D98765:E76543", "Q12345", "Q12345")]
        [DataRow("A12345:B34567;D98765:E76543", "G85672", "G85672")]
        public void CheckOdsCode_CodeIsUnchanged_NoInformationalMessageLogged(
            string odsCodeMap, string inputOdsCode, string expectedOutputOdsCode)
        {
            // Arrange
            _mockConfiguration.Setup(x => x["ODS_REMAP_ENABLED"]).Returns("true");
            _mockConfiguration.Setup(x => x["ODS_REMAP_MAP"]).Returns(odsCodeMap);
            var _systemUnderTest = _fixture.Create<OdsCodeMassager>();

            // Act
            var outputOdsCode = _systemUnderTest.CheckOdsCode(inputOdsCode);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Information, $"Massaging ODS Code {inputOdsCode} to {outputOdsCode}", Times.Never());
            outputOdsCode.Should().Be(expectedOutputOdsCode);
        }
    }
}