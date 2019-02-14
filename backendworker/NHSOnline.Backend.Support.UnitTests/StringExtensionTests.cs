using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Support.UnitTests
{
    [TestClass]
    public class StringExtensionTests
    {
        [TestMethod]
        public void NhsNumberFormat_NullValue_ReturnsEmptyString()
        {
            string nhsNumber = null;
            const string expectedValue = "";

            var result = nhsNumber.FormatToNhsNumber();

            result.Should().Be(expectedValue);
        }

        [TestMethod]
        public void NhsNumberFormat_EmptyString_ReturnsEmptyString()
        {
            const string nhsNumber = "";
            const string expectedValue = "";

            var result = nhsNumber.FormatToNhsNumber();

            result.Should().Be(expectedValue);
        }

        [TestMethod]
        public void NhsNumberFormat_ShortString_ReturnsShortString()
        {
            const string nhsNumber = "1234";
            const string expectedValue = "1234";

            var result = nhsNumber.FormatToNhsNumber();

            result.Should().Be(expectedValue);
        }

        [TestMethod]
        public void NhsNumberFormat_LongString_ReturnsLongString()
        {
            const string nhsNumber = "0123456789101112";
            const string expectedValue = "0123456789101112";

            var result = nhsNumber.FormatToNhsNumber();

            result.Should().Be(expectedValue);
        }
        
        [TestMethod]
        public void NhsNumberFormat_PreFormattedString_ReturnsFormattedString()
        {
            const string nhsNumber = "012 345 6789";
            const string expectedValue = "012 345 6789";

            var result = nhsNumber.FormatToNhsNumber();

            result.Should().Be(expectedValue);
        }

        [TestMethod]
        public void NhsNumberFormat_CorrectString_ReturnsFormattedString()
        {
            const string nhsNumber = "0123456789";
            const string expectedValue = "012 345 6789";

            var result = nhsNumber.FormatToNhsNumber();

            result.Should().Be(expectedValue);
        }
        
        [TestMethod]
        public void RemoveWhiteSpace_WhiteSpaceMidString_ReturnsStringNoWhiteSpace()
        {
            const string sourceString = "012 345 6789";
            const string expectedValue = "0123456789";

            var result = sourceString.RemoveWhiteSpace();

            result.Should().Be(expectedValue);
        }
        
        [TestMethod]
        public void RemoveWhiteSpace_WhiteSpacePreString_ReturnsStringNoWhiteSpace()
        {
            const string sourceString = "   0123456789";
            const string expectedValue = "0123456789";

            var result = sourceString.RemoveWhiteSpace();

            result.Should().Be(expectedValue);
        }
        
        [TestMethod]
        public void RemoveWhiteSpace_WhiteSpacePostString_ReturnsStringNoWhiteSpace()
        {
            const string sourceString = "0123456789   ";
            const string expectedValue = "0123456789";

            var result = sourceString.RemoveWhiteSpace();

            result.Should().Be(expectedValue);
        }
        
        [TestMethod]
        public void RemoveWhiteSpace_WhiteSpaceStartMidEndString_ReturnsStringNoWhiteSpace()
        {
            const string sourceString = "  01 23  456     7 89   ";
            const string expectedValue = "0123456789";

            var result = sourceString.RemoveWhiteSpace();

            result.Should().Be(expectedValue);
        }
    }
}
