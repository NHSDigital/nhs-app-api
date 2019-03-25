using System;
using System.ComponentModel.DataAnnotations;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support.ValidationAttributes;
using UnitTestHelper;

namespace NHSOnline.Backend.Support.UnitTests.ValidationAttributes
{
    [TestClass]
    public class SafeStringAttributeTests
    {
        private const string MemberName = "FieldName";
        private const string DisplayName = "DisplayName";
        private SafeStringAttribute _attribute;
        private Mock<IServiceProvider> _serviceProvider;
        private IFixture _fixture;
        private ValidationContext _validationContext;
        private Mock<ILogger<SafeStringAttribute>> _safeStringLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _safeStringLogger = _fixture.Freeze<Mock<ILogger<SafeStringAttribute>>>();

            _attribute = new SafeStringAttribute();
            _serviceProvider = _fixture.Freeze<Mock<IServiceProvider>>();

            _serviceProvider
                .Setup(x => x.GetService(typeof(ILogger<SafeStringAttribute>)))
                .Returns(_safeStringLogger.Object);

            _validationContext = CreateValidationContext(_attribute);
        }

        [TestMethod]
        public void IsValid_ReturnsSuccess_WhenValueIsNull()
        {
            // Act
            var result = _attribute.GetValidationResult(null, _validationContext);

            // Assert
            Assert.AreEqual(ValidationResult.Success, result);
        }
        
        [TestMethod]
        public void IsValid_ReturnsError_WhenValueIsNotString()
        {
            // Act
            var result = _attribute.GetValidationResult(1, _validationContext);

            // Assert
            Assert.AreEqual("The value was not deemed safe", result.ErrorMessage);
        }

        [DataTestMethod]
        [DataRow("xxx<script>xxx")]
        [DataRow("xxx<SCRIPT>xxx")]
        [DataRow("xxx<sCrIpT>xxx")]
        public void IsValid_ReturnsError_WhenTextContainsScriptTag(string valueToTest)
        {
            // Act
            var result = _attribute.GetValidationResult(valueToTest, _validationContext);

            // Assert
            Assert.AreEqual("The value was not deemed safe", result.ErrorMessage);
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
    }
}
