using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.PfsApi.SpineSearch;

namespace NHSOnline.Backend.PfsApi.UnitTests.SpineSearch
{
    [TestClass]
    public class NhsAppSpinePdsUpdatePropertiesTests
    {
        private NhsAppSpinePdsUpdateProperties _spinePdsUpdateProperties;
        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _spinePdsUpdateProperties = _fixture.Create<NhsAppSpinePdsUpdateProperties>();
        }

        [TestMethod]
        public void Validate_ReturnsTrue_WhenAllPropertiesArePopulated()
        {
            // Act
            var result = _spinePdsUpdateProperties.Validate();

            // Assert
            result.Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void Validate_ReturnsFalse_WhenCpaIdIsEmpty(string value)
        {
            // Arrange
            _spinePdsUpdateProperties.CpaId = value;

            // Act
            var result = _spinePdsUpdateProperties.Validate();

            // Assert
            result.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void Validate_ReturnsFalse_WhenFromAsidIsEmpty(string value)
        {
            // Arrange
            _spinePdsUpdateProperties.FromAsid = value;

            // Act
            var result = _spinePdsUpdateProperties.Validate();

            // Assert
            result.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void Validate_ReturnsFalse_WhenToAsidIsEmpty(string value)
        {
            // Arrange
            _spinePdsUpdateProperties.ToAsid = value;

            // Act
            var result = _spinePdsUpdateProperties.Validate();

            // Assert
            result.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void Validate_ReturnsFalse_WhenToPartyIdIsEmpty(string value)
        {
            // Arrange
            _spinePdsUpdateProperties.ToPartyId = value;

            // Act
            var result = _spinePdsUpdateProperties.Validate();

            // Assert
            result.Should().BeFalse();
        }
    }
}
