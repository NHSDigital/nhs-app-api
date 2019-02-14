using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.PatientRecord
{
    [TestClass]
    public class PatientRecordStringExtensionsTests
    {
        [TestMethod]
        public void SafeParseToNullableDateTimeOffset_CorrectString_ReturnsDateTimeOffset()
        {
            const string dateTimeString = "2016-09-12T12:34:03.0Z";
            DateTimeOffset? expectedValue = new DateTimeOffset(2016,9,12,12,34,3,new TimeSpan(0));

            var result = dateTimeString.SafeParseToNullableDateTimeOffset();

            result.Should().Be(expectedValue);
        }
        
        [TestMethod]
        public void SafeParseToNullableDateTimeOffset_InvalidString_ReturnsNull()
        {
            const string dateTimeString = "isdfoasdfakljdsf";

            var result = dateTimeString.SafeParseToNullableDateTimeOffset();

            result.Should().BeNull();
        }
        
        [TestMethod]
        public void SafeParseToNullableDateTimeOffset_NullString_ReturnsNull()
        {
            const string dateTimeString = null;

            var result = dateTimeString.SafeParseToNullableDateTimeOffset();

            result.Should().BeNull();
        }
    }
}