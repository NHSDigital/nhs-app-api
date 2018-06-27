
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.Backend.Worker.UnitTests
{
    [TestClass]
    public class StringExtensionTests
    {
        [TestMethod]
        public void NhsNumberFormat_NullValue_ReturnsEmptyString()
        {
            string nhsNumber = null;
            string expectedValue = "";

            string result = nhsNumber.FormatToNhsNumber();

            result.Should().Be(expectedValue);
        }

        [TestMethod]
        public void NhsNumberFormat_EmptyString_ReturnsEmptyString()
        {
            string nhsNumber = "";
            string expectedValue = "";

            string result = nhsNumber.FormatToNhsNumber();

            result.Should().Be(expectedValue);
        }

        [TestMethod]
        public void NhsNumberFormat_ShortString_ReturnsShortString()
        {
            string nhsNumber = "1234";
            string expectedValue = "1234";

            string result = nhsNumber.FormatToNhsNumber();

            result.Should().Be(expectedValue);
        }

        [TestMethod]
        public void NhsNumberFormat_LongString_ReturnsLongString()
        {
            string nhsNumber = "0123456789101112";
            string expectedValue = "0123456789101112";

            string result = nhsNumber.FormatToNhsNumber();

            result.Should().Be(expectedValue);
        }

        [TestMethod]
        public void NhsNumberFormat_CorrectString_ReturnsFormattedString()
        {
            string nhsNumber = "0123456789";
            string expectedValue = "012 345 6789";

            string result = nhsNumber.FormatToNhsNumber();

            result.Should().Be(expectedValue);
        }
    }
}
