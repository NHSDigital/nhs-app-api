using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Moq;
using NHSOnline.Backend.Worker.Support;
using NHSOnline.Backend.Worker.UnitTests.Areas;

namespace NHSOnline.Backend.Worker.UnitTests.Support
{
    [TestClass]
    public sealed class ValidateAndLogTests
    {
        private IFixture _fixture;
        private Mock<ILogger> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _logger = _fixture.Freeze<Mock<ILogger>>();
        }

        [TestMethod]
        public void Valid_ReturnsTrue()
        {
            var validator = new ValidateAndLog(_logger.Object)
                .IsNotNullOrWhitespace("StringTest", "StringTest")
                .IsNotNull(DateTime.Now, "DateTime");


            validator.IsValid().Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow(null, "Null")]
        [DataRow("", "Empty")]
        public void Invalid_String_ReturnsFalse(string stringValue, string label)
        {
            var validator = new ValidateAndLog(_logger.Object)
                .IsNotNullOrWhitespace(stringValue, label)
                .IsNotNull(DateTime.Now, "DateTime");

            validator.IsValid().Should().BeFalse();
        }

        [TestMethod]
        public void Invalid_NullDateTime_ReturnsFalse()
        {
            var validator = new ValidateAndLog(_logger.Object)
                .IsNotNullOrWhitespace("StringTest", "StringTest")
                .IsNotNull(null, "DateTime");

            validator.IsValid().Should().BeFalse();
        }
    }
}
