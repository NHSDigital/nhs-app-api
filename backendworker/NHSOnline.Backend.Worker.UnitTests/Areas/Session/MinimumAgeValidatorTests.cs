using System;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Session;
using NHSOnline.Backend.Worker.Settings;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Session
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
            var todaysDateMinusOneDay = DateTime.Now.AddYears(-16).AddDays(1);
            
            _minimumAgeValidator.IsValid(todaysDateMinusOneDay, MinimumLinkageAge).Should().BeFalse();
        }
        
        [TestMethod]
        public void IsValid_ReturnsTrue_WhenBirthDateIsExactly16()
        {
            var todaysDateMinus16Years = DateTime.Now.AddYears(-16);
            
            _minimumAgeValidator.IsValid(todaysDateMinus16Years, MinimumLinkageAge).Should().BeTrue();
        }
        
        [TestMethod]
        public void IsValid_ReturnsTrue_WhenBirthDateIsOver16()
        {
            var todaysDateMinus17Years = DateTime.Now.AddYears(-16).AddDays(-1);
            
            _minimumAgeValidator.IsValid(todaysDateMinus17Years, MinimumLinkageAge).Should().BeTrue();
        }
    }
}
