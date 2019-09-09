using System;
using System.ComponentModel.DataAnnotations;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support.ValidationAttributes;
//using NHSOnline.Backend.PfsApi.ValidationAttributes;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.ValidationAttributes
{
    [TestClass]
    public class AcceptsAttributeTests
    {
        private const string MemberName = "FieldName";
        private const string DisplayName = "DisplayName";
        private Mock<IServiceProvider> _serviceProvider;
        private IFixture _fixture;
        private Mock<ILogger<AcceptsAttribute>> _acceptedValuesLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _acceptedValuesLogger = _fixture.Freeze<Mock<ILogger<AcceptsAttribute>>>();

            _serviceProvider = _fixture.Freeze<Mock<IServiceProvider>>();

            _serviceProvider
                .Setup(x => x.GetService(typeof(ILogger<SafeStringAttribute>)))
                .Returns(_acceptedValuesLogger.Object);
        }

        [TestMethod]
        [DataRow(TestPosition.Third)]
        [DataRow(TestPosition.Second)]
        public void IsValid_ReturnsSuccess_WhenEnumValueMatchesExpected(TestPosition testPosition)
        {
            // Arrange
            var attribute = new AcceptsAttribute(TestPosition.Third, TestPosition.Second);
            var validationContext = CreateValidationContext(attribute);

            // Act
            var result = attribute.GetValidationResult(testPosition, validationContext);

            // Assert
            result.Should().Be(ValidationResult.Success);
        }

        [TestMethod]
        public void IsValid_ReturnsSuccess_WhenComparingNullValues()
        {
            // Arrange
            var attribute = new AcceptsAttribute(null);
            var validationContext = CreateValidationContext(attribute);

            // Act
            var result = attribute.GetValidationResult(null, validationContext);

            // Assert
            result.Should().Be(ValidationResult.Success);
        }

        [TestMethod]
        public void IsValid_ReturnsSuccess_WhenComparingNullOtherValues()
        {
            // Arrange
            var attribute = new AcceptsAttribute("firstValue", null);
            var validationContext = CreateValidationContext(attribute);

            // Act
            var result = attribute.GetValidationResult(null, validationContext);

            // Assert
            result.Should().Be(ValidationResult.Success);
        }

        [TestMethod]
        [DataRow(TestPosition.None)]
        [DataRow(TestPosition.First)]
        [DataRow(TestPosition.Fourth)]
        public void IsValid_ReturnsError_WhenEnumValueDoesNotMatchExpected(TestPosition testPosition)
        {
            // Arrange
            var attribute = new AcceptsAttribute(TestPosition.Third, TestPosition.Second);
            var validationContext = CreateValidationContext(attribute);

            // Act
            var result = attribute.GetValidationResult(testPosition, validationContext);

            // Assert
            result.Should().NotBeNull();
            result.ErrorMessage.Should().Be(ConstructErrorMessage(testPosition, "Third, Second"));
        }

        [TestMethod]
        [DataRow("_invalid_")]
        [DataRow("beta")]
        [DataRow(TestPosition.Second)]
        public void IsValid_ReturnsError_WhenValueDoesNotMatchExpected(object value)
        {
            // Arrange
            var attribute = new AcceptsAttribute(null, "valid");
            var validationContext = CreateValidationContext(attribute);

            // Act
            var result = attribute.GetValidationResult(value, validationContext);

            // Assert
            result.Should().NotBeNull();
            result.ErrorMessage.Should().Be(ConstructErrorMessage(value, "null, valid"));
        }

        private ValidationContext CreateValidationContext(object instance)
        {
            var context = new ValidationContext(instance, _serviceProvider.Object, null)
            {
                MemberName = MemberName,
                DisplayName = DisplayName,
            };
            return context;
        }

        private static string ConstructErrorMessage(object value, string acceptedValuesText) =>
            $"'{value}' is not one of the accepted values ({acceptedValuesText})";

        public enum TestPosition
        {
            None = 0,
            First,
            Second,
            Third,
            Fourth
        }
    }
}