using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.Backend.Support.UnitTests
{
    [TestClass]
    public class CalculateAgeTests
    {
        [TestMethod]
        public void CalculateAgeInMonthsAndYears_ReturnsEmptyAgeDataObjectWhenDateOfBirthIsNull()
        {
            // Arrange
            DateTime? dateOfBirth = null;
            var ageDataObject = new AgeData
            {
                AgeMonths = null,
                AgeYears = null
            };

            // Act
            var calculatedAge = CalculateAge.CalculateAgeInMonthsAndYears(dateOfBirth);

            // Assert
            calculatedAge.AgeMonths.Should().Be(ageDataObject.AgeMonths);
            calculatedAge.AgeYears.Should().Be(ageDataObject.AgeYears);
        }
        
        
        [TestMethod]
        public void CalculateAgeInMonthsAndYears_ReturnsCorrectAgeDataObjectWhenDateOfBirthIsValidAndGreaterThan1Year()
        {
            // Arrange
            DateTime? dateOfBirth = (DateTime.Now).AddMonths(-2);
            dateOfBirth = dateOfBirth.Value.AddYears(-5);

            var ageDataObject = new AgeData
            {
                //If the age is above 1, then the ageMonths will be 0
                AgeMonths = 0,
                AgeYears = 5
            };

            // Act
            var calculatedAge = CalculateAge.CalculateAgeInMonthsAndYears(dateOfBirth);

            // Assert
            calculatedAge.AgeMonths.Should().Be(ageDataObject.AgeMonths);
            calculatedAge.AgeYears.Should().Be(ageDataObject.AgeYears);
        }

        [TestMethod]
        public void CalculateAgeInMonthsAndYears_ReturnsCorrectAgeDataObjectWhenDateOfBirthIsValidAndLessThan1Year()
        {
            // Arrange
            DateTime? dateOfBirth = (DateTime.Now).AddMonths(-5);
            dateOfBirth = dateOfBirth.Value.AddYears(0);

            var ageDataObject = new AgeData
            {
                AgeMonths = 5,
                AgeYears = 0
            };

            // Act
            var calculatedAge = CalculateAge.CalculateAgeInMonthsAndYears(dateOfBirth);

            // Assert
            calculatedAge.AgeMonths.Should().Be(ageDataObject.AgeMonths);
            calculatedAge.AgeYears.Should().Be(ageDataObject.AgeYears);
        }
    }
}