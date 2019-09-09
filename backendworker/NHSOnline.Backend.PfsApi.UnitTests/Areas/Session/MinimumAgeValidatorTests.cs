using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Session
{
    [TestClass]
    public class MinimumAgeValidatorTests
    {
        private IMinimumAgeValidator _minimumAgeValidator;
        private const int MinimumLinkageAge = 16;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _minimumAgeValidator = new MinimumAgeValidator();
        }
        
        [TestMethod]
        public void IsValid_ReturnsFalse_WhenBirthDateIsUnder16()
        {
            // Arrange
            var todayMinusOneDay = DateTime.Now.AddYears(-16).AddDays(1);
            
            // Act
            var result = _minimumAgeValidator.IsValid(todayMinusOneDay, MinimumLinkageAge);
                
            // Assert
            result.Should().BeFalse();
        }
        
        [TestMethod]
        public void IsValid_ReturnsTrue_WhenBirthDateIsExactly16()
        {
            // Arrange
            var todayMinus16Years = DateTime.Now.AddYears(-16);
            
            // Act
            var result = _minimumAgeValidator.IsValid(todayMinus16Years, MinimumLinkageAge);
                
            // Assert
            result.Should().BeTrue();
        }
        
        [TestMethod]
        public void IsValid_ReturnsTrue_WhenBirthDateIsOver16()
        {
            // Arrange
            var todayMinus17Years = DateTime.Now.AddYears(-16).AddDays(-1);
            
            // Act
            var result = _minimumAgeValidator.IsValid(todayMinus17Years, MinimumLinkageAge);
                
            // Assert
            result.Should().BeTrue();
        }
    }
}
