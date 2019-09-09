using System;
using AutoFixture;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.PatientRecord
{
    [TestClass]
    public class PatientRecordStringExtensionsTests
    {
        private Fixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
        }
        [TestMethod]
        public void SafeParseToNullableDateTimeOffset_CorrectString_ReturnsDateTimeOffset()
        {
            // Arrange
            const string dateTimeString = "2016-09-12T12:34:03.0Z";
            DateTimeOffset? expectedValue = new DateTimeOffset(2016,9,12,12,34,3,new TimeSpan(0));

            // Act
            var result = dateTimeString.SafeParseToNullableDateTimeOffset();

            // Assert
            result.Should().Be(expectedValue);
        }
        
        [TestMethod]
        public void SafeParseToNullableDateTimeOffset_InvalidString_ReturnsNull()
        {
            // Arrange
            var notADateTimeString = _fixture.Create<string>();

            // Act
            var result = notADateTimeString.SafeParseToNullableDateTimeOffset();

            // Assert
            result.Should().BeNull();
        }
        
        [TestMethod]
        public void SafeParseToNullableDateTimeOffset_NullString_ReturnsNull()
        {
            // Arrange
            const string dateTimeString = null;

            // Act
            var result = dateTimeString.SafeParseToNullableDateTimeOffset();

            // Assert
            result.Should().BeNull();
        }
    }
}