using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support.Mappers;
using UnitTestHelper;

namespace NHSOnline.Backend.Support.UnitTests.Mappers
{
    [TestClass]
    public sealed class ProofLevelMapperTests
    {
        private IFixture _fixture;
        private ProofLevelMapper _systemUnderTest;
        private Mock<ILogger<ProofLevelMapper>> _mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _mockLogger = _fixture.Freeze<Mock<ILogger<ProofLevelMapper>>>();

            _systemUnderTest = _fixture.Create<ProofLevelMapper>();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("    ")]
        public void Map_IdentityProofingLevelIsNullOrEmpty_DefaultsToP9(string identityProofingLevel)
        {
            // Act
            var result = _systemUnderTest.Map(identityProofingLevel);

            // Assert
            _mockLogger.VerifyNoOtherCalls();

            result.Should().Be(ProofLevel.P9);
        }

        [TestMethod]
        [DataRow("P5", ProofLevel.P5)]
        [DataRow("P9", ProofLevel.P9)]
        public void Map_IdentityProofingLevelIsValid_ReturnsMappedEnum(string identityProofingLevel, ProofLevel expected)
        {
            // Act
            var result = _systemUnderTest.Map(identityProofingLevel);

            // Assert
            _mockLogger.VerifyNoOtherCalls();

            result.Should().Be(expected);
        }

        [TestMethod]
        public void Map_UnknownIdentityProofingLevel_ReturnsNull()
        {
            // Act
            var result = _systemUnderTest.Map("P7");

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Error, Times.Once());

            result.Should().BeNull();
        }
    }
}
