using System;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitTestHelper;

namespace NHSOnline.Backend.Support.UnitTests
{
    [TestClass]
    public class IEnumerableExtensionsTests
    {
        private IFixture _fixture;
        private Mock<ILogger<IEnumerableExtensionsTests>> _mockLogger;
        private const string DuplicatesLogMessagePrefix = "Duplicate keys found when building dictionary: ";

        private class SourceObject
        {
            public string Id { get; set; }
            public string Description { get; set; }

            public SourceObject(string id, string description)
            {
                Id = id;
                Description = description;
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _mockLogger = _fixture.Freeze<Mock<ILogger<IEnumerableExtensionsTests>>>();
        }
        
        [TestMethod]
        public void ToDictionaryLogOnFailure_ThrowsArgumentExceptionForReasonsOtherThanDuplicates_NoLoggingAndExceptionRethrown()
        {
            // Arrange
            SourceObject[] sourceList = null;
            
            // Act
            Action act = () =>
                sourceList.ToDictionaryLogOnFailure(x => x.Id, x => x.Description, _mockLogger.Object);
            
            // Assert
            act.Should().Throw<ArgumentException>();
            _mockLogger.VerifyLogger(
                LogLevel.Information, 
                DuplicatesLogMessagePrefix,
                null, Times.Never());
        }
        
        [TestMethod]
        public void ToDictionaryLogOnFailure_NoDuplicateKeys_NoLoggingNorErrorsThrownAndDictionaryReturned()
        {
            // Arrange
            var sourceList = new[]
            {
                new SourceObject("1", "Slot 1"), 
                new SourceObject("2", "Slot 2")
            };
            var expectedResult = sourceList.ToDictionary(x => x.Id, x => x.Description);

            // Act
            var result = sourceList.ToDictionaryLogOnFailure(x => x.Id, x => x.Description, _mockLogger.Object);

            // Assert
            _mockLogger.VerifyLogger(
                LogLevel.Information, 
                DuplicatesLogMessagePrefix,
                null, Times.Never());
            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void ToDictionaryLogOnFailure_DuplicateKeysExist_DuplicatesAreLoggedAndExceptionRethrown()
        {
            // Arrange
            var sourceList = new[]
            {
                new SourceObject("1", "Slot 1"), 
                new SourceObject("2", "Slot 2"), 
                new SourceObject("1", "Slot 1 (new version)")
            };
            
            // Act
            Action act = () =>
                sourceList.ToDictionaryLogOnFailure(x => x.Id, x => x.Description, _mockLogger.Object);
            
            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("An item with the same key has already been added*");
            _mockLogger.VerifyLogger(
                LogLevel.Information, 
                DuplicatesLogMessagePrefix,
                null, Times.Exactly(1));
        }
    }
}