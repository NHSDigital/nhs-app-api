using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NHSOnline.Backend.Repository.UnitTests
{
    [TestClass]
    public sealed class AzureMongoClientTests
    {
        private Mock<INamedMongoClient> _mockPrimaryMongoClient;
        private Mock<INamedMongoClient> _mockSecondaryMongoClient;
        private Mock<ILogger<AzureMongoClient>> _mockLogger;
        private AzureMongoClient _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockPrimaryMongoClient = new Mock<INamedMongoClient>();
            _mockSecondaryMongoClient = new Mock<INamedMongoClient>();
            _mockLogger = new Mock<ILogger<AzureMongoClient>>();

            _systemUnderTest = new AzureMongoClient(_mockLogger.Object);
        }

        [TestMethod]
        public void WhenInstanceCreated_DefaultsAreSet()
        {
            // Assert
            _systemUnderTest.IsHealthy.Should().BeFalse();
            _systemUnderTest.UsingPrimary.Should().BeFalse();
            _systemUnderTest.ActiveClient.Should().BeNull();
        }

        [TestMethod]
        public void ActiveClientIsNullWhenIsHealthyIsFalse()
        {
            // Arrange
            _systemUnderTest.Initialize(
                _mockPrimaryMongoClient.Object,
                _mockSecondaryMongoClient.Object);

            // Act
            _systemUnderTest.IsHealthy = false;

            // Assert
            _systemUnderTest.ActiveClient.Should().BeNull();
        }

        [TestMethod]
        public void ActiveClientIsPrimary_WhenPrimaryIsTrue()
        {
            // Arrange
            _systemUnderTest.Initialize(
                _mockPrimaryMongoClient.Object,
                _mockSecondaryMongoClient.Object);

            // Act
            _systemUnderTest.IsHealthy = true;
            _systemUnderTest.UsingPrimary = true;

            // Assert
            _systemUnderTest.ActiveClient.Should().Be(_mockPrimaryMongoClient.Object);
        }

        [TestMethod]
        public void ActiveClientIsSecondary_WhenIsPrimaryIsFalse()
        {
            // Arrange
            _systemUnderTest.Initialize(
                _mockPrimaryMongoClient.Object,
                _mockSecondaryMongoClient.Object);

            // Act
            _systemUnderTest.IsHealthy = true;
            _systemUnderTest.UsingPrimary = false;

            // Assert
            _systemUnderTest.ActiveClient.Should().Be(_mockSecondaryMongoClient.Object);
        }

        [TestMethod]
        public void Initialize_SetsHealthinessCorrectly()
        {
            // Act
            _systemUnderTest.Initialize(
                _mockPrimaryMongoClient.Object,
                _mockSecondaryMongoClient.Object);

            // Assert
            _systemUnderTest.IsHealthy.Should().BeTrue();
            _systemUnderTest.UsingPrimary.Should().BeTrue();
            _systemUnderTest.ActiveClient.Should().Be(_mockPrimaryMongoClient.Object);
        }

        [TestMethod]
        public void ReportAuthenticationFailureWithPrimary_WhenUsingPrimary_SwitchesToSecondaryClient()
        {
            // Arrange
            _systemUnderTest.Initialize(
                _mockPrimaryMongoClient.Object,
                _mockSecondaryMongoClient.Object);

            // Act
            _systemUnderTest.ReportAuthenticationFailure(AzureMongoClientType.Primary);

            // Assert
            _systemUnderTest.IsHealthy.Should().BeTrue();
            _systemUnderTest.UsingPrimary.Should().BeFalse();
            _systemUnderTest.ActiveClient.Should().Be(_mockSecondaryMongoClient.Object);
        }

        [TestMethod]
        public void ReportAuthenticationFailureWithSecondary_WhenUsingSecondary_SwitchesToNoActiveClient()
        {
            // Arrange
            _systemUnderTest.Initialize(
                _mockPrimaryMongoClient.Object,
                _mockSecondaryMongoClient.Object);
            _systemUnderTest.UsingPrimary = false;

            // Act
            _systemUnderTest.ReportAuthenticationFailure(AzureMongoClientType.Secondary);

            // Assert
            _systemUnderTest.IsHealthy.Should().BeFalse();
            _systemUnderTest.UsingPrimary.Should().BeFalse();
            _systemUnderTest.ActiveClient.Should().BeNull();
        }

        [TestMethod]
        public void ReportAuthenticationFailureWithSecondary_WhenUsingPrimary_DoesNotSwitch()
        {
            // Arrange
            _systemUnderTest.Initialize(
                _mockPrimaryMongoClient.Object,
                _mockSecondaryMongoClient.Object);

            // Act
            _systemUnderTest.ReportAuthenticationFailure(AzureMongoClientType.Secondary);

            // Assert
            _systemUnderTest.IsHealthy.Should().BeTrue();
            _systemUnderTest.UsingPrimary.Should().BeTrue();
            _systemUnderTest.ActiveClient.Should().Be(_mockPrimaryMongoClient.Object);
        }

        [TestMethod]
        public void ReportAuthenticationFailureWithPrimary_WhenUsingSecondary_DoesNotSwitch()
        {
            // Arrange
            _systemUnderTest.Initialize(
                _mockPrimaryMongoClient.Object,
                _mockSecondaryMongoClient.Object);
            _systemUnderTest.UsingPrimary = false;

            // Act
            _systemUnderTest.ReportAuthenticationFailure(AzureMongoClientType.Primary);

            // Assert
            _systemUnderTest.IsHealthy.Should().BeTrue();
            _systemUnderTest.UsingPrimary.Should().BeFalse();
            _systemUnderTest.ActiveClient.Should().Be(_mockSecondaryMongoClient.Object);
        }
    }
}
