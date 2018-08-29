using System;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Appointments;
using NHSOnline.Backend.Worker.Areas.Session;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.Settings;
using NHSOnline.Backend.Worker.Support.Temporal;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Session
{
    [TestClass]
    public class MinimumAgeValidatorTests
    {
        private IMinimumAgeValidator _minimumAgeValidator;
        private IOptions<ConfigurationSettings> _options;

        [TestInitialize]
        public void TestInitialize()
        {
            _options = new Mock<IOptions<ConfigurationSettings>>().Object;
            _options = Options.Create(new ConfigurationSettings
            {
                MinimumAge = 16
            });
            
            _minimumAgeValidator = new MinimumAgeValidator(_options);
        }
        
        [TestMethod]
        public void IsValid_ReturnsFalse_WhenBirthDateIsUnder16()
        {
            var todaysDateMinusOneDay = DateTime.Now.AddYears(-16).AddDays(1);
            
            _minimumAgeValidator.IsValid(todaysDateMinusOneDay).Should().BeFalse();
        }
        
        [TestMethod]
        public void IsValid_ReturnsTrue_WhenBirthDateIsExactly16()
        {
            var todaysDateMinus16Years = DateTime.Now.AddYears(-16);
            
            _minimumAgeValidator.IsValid(todaysDateMinus16Years).Should().BeTrue();
        }
        
        [TestMethod]
        public void IsValid_ReturnsTrue_WhenBirthDateIsOver16()
        {
            var todaysDateMinus17Years = DateTime.Now.AddYears(-16).AddDays(-1);
            
            _minimumAgeValidator.IsValid(todaysDateMinus17Years).Should().BeTrue();
        }
    }
}
