using System;
using System.Globalization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.Support.UnitTests
{
    [TestClass]
    public class CalculateAgeTests
    {
        [TestMethod]
        public void CalculateAgeInMonthsAndYears_ReturnsEmptyAgeDataObjectWhenDateOfBirthIsNull()
        {
            // Arrange
            var now = new DateTime(2020, 04, 12, 04, 30, 00, DateTimeKind.Local);
            DateTime? dateOfBirth = null;
            var expectedAgeData = new AgeData
            {
                AgeMonths = null,
                AgeYears = null
            };

            // Act
            var calculateAge = CreateCalculateAge(now);
            var actualAgeData = calculateAge.CalculateAgeInMonthsAndYears(dateOfBirth);

            // Assert
            actualAgeData.Should().BeEquivalentTo(expectedAgeData);
        }

        [TestMethod]
        [DataRow("2021-04-12", "2021-04-12T04:30:00", 0)]
        [DataRow("2021-04-12", "2021-04-13T04:30:00", 0)]
        [DataRow("2021-04-12", "2021-05-11T04:30:00", 0)]
        [DataRow("2021-04-12", "2021-05-12T04:30:00", 1)]
        [DataRow("2021-04-12", "2021-05-13T04:30:00", 1)]
        [DataRow("2021-04-12", "2021-10-11T04:30:00", 5)]
        [DataRow("2021-04-12", "2021-10-12T04:30:00", 6)]
        [DataRow("2021-04-12", "2021-10-12T04:30:00", 6)]
        [DataRow("2021-04-12", "2021-12-31T04:30:00", 8)]
        [DataRow("2021-04-12", "2022-01-01T04:30:00", 8)]
        [DataRow("2021-04-12", "2022-04-11T04:30:00", 11)]
        public void CalculateAgeInMonthsAndYears_AgeLessThanZero_ReturnsNumberOfMonths(string dateOfBirth, string now, int expectedMonths)
        {
            // Arrange
            var dateOfBirthDate = DateTime.ParseExact(dateOfBirth, "yyyy-MM-dd", null, DateTimeStyles.AssumeLocal);
            var nowDate = DateTime.ParseExact(now, "yyyy-MM-ddTHH:mm:ss", null, DateTimeStyles.AssumeLocal);

            var ageDataObject = new AgeData
            {
                AgeMonths = expectedMonths,
                AgeYears = 0
            };

            // Act
            var calculateAge = CreateCalculateAge(nowDate);
            var calculatedAge = calculateAge.CalculateAgeInMonthsAndYears(dateOfBirthDate);

            // Assert
            calculatedAge.AgeMonths.Should().Be(ageDataObject.AgeMonths);
            calculatedAge.AgeYears.Should().Be(ageDataObject.AgeYears);
        }

        [TestMethod]
        [DataRow("2019-04-12", "2021-04-12T04:30:00")]
        [DataRow("2019-03-13", "2021-04-12T04:30:00")]
        [DataRow("2019-02-14", "2021-04-12T04:30:00")]
        [DataRow("2019-01-15", "2021-04-12T04:30:00")]
        [DataRow("2018-12-16", "2021-04-12T04:30:00")]
        [DataRow("2010-07-17", "2021-04-12T04:30:00")]
        [DataRow("1980-09-30", "2021-04-12T04:30:00")]
        public void CalculateAgeInMonthsAndYears_AgeGreaterThanZero_ReturnsZeroMonths(string dateOfBirth, string now)
        {
            // Arrange
            var dateOfBirthDate = DateTime.ParseExact(dateOfBirth, "yyyy-MM-dd", null, DateTimeStyles.AssumeLocal);
            var nowDate = DateTime.ParseExact(now, "yyyy-MM-ddTHH:mm:ss", null, DateTimeStyles.AssumeLocal);

            // Act
            var calculateAge = CreateCalculateAge(nowDate);
            var calculatedAge = calculateAge.CalculateAgeInMonthsAndYears(dateOfBirthDate);

            // Assert
            calculatedAge.AgeMonths.Should().Be(0);
        }

        [TestMethod]
        [DataRow("2020-08-31", "2020-08-31T04:30:00", 0)]
        [DataRow("2020-08-31", "2020-09-01T04:30:00", 0)]
        [DataRow("2020-08-31", "2020-09-30T04:30:00", 0)]
        [DataRow("2020-08-31", "2020-10-01T04:30:00", 1)]
        [DataRow("2020-08-31", "2021-02-28T04:30:00", 5)]
        [DataRow("2020-08-31", "2021-03-01T04:30:00", 6)]
        public void CalculateAgeInMonthsAndYears_DateOfBirthOnThirtyFirstDayOfMonth_AgeInMonthsAccountsForDaysInMonth(
            string dateOfBirth,
            string now,
            int expectedMonths)
        {
            // Arrange
            var dateOfBirthDate = DateTime.ParseExact(dateOfBirth, "yyyy-MM-dd", null, DateTimeStyles.AssumeLocal);
            var nowDate = DateTime.ParseExact(now, "yyyy-MM-ddTHH:mm:ss", null, DateTimeStyles.AssumeLocal);

            // Act
            var calculateAge = CreateCalculateAge(nowDate);
            var calculatedAge = calculateAge.CalculateAgeInMonthsAndYears(dateOfBirthDate);

            // Assert
            calculatedAge.AgeMonths.Should().Be(expectedMonths);
        }

        [TestMethod]
        public void CalculateAgeInMonthsAndYears_DateOfBirthEarlierInYear_ReturnsAgeIncludingCurrentYear()
        {
            // Arrange
            var now = new DateTime(2020, 04, 12, 04, 30, 00, DateTimeKind.Local);
            var dateOfBirthDate = new DateTime(2000, 04, 11, 0, 0, 0, DateTimeKind.Local);

            // Act
            var calculateAge = CreateCalculateAge(now);
            var calculatedAge = calculateAge.CalculateAgeInMonthsAndYears(dateOfBirthDate);

            // Assert
            calculatedAge.AgeYears.Should().Be(20);
        }

        [TestMethod]
        public void CalculateAgeInMonthsAndYears_DateOfBirthToday_ReturnsAgeNotIncludingCurrentYear()
        {
            // Arrange
            var now = new DateTime(2020, 04, 12, 04, 30, 00, DateTimeKind.Local);
            var dateOfBirthDate = new DateTime(2000, 04, 12, 0, 0, 0, DateTimeKind.Local);

            // Act
            var calculateAge = CreateCalculateAge(now);
            var calculatedAge = calculateAge.CalculateAgeInMonthsAndYears(dateOfBirthDate);

            // Assert
            calculatedAge.AgeYears.Should().Be(20);
        }

        [TestMethod]
        public void CalculateAgeInMonthsAndYears_DateOfBirthLaterInYear_ReturnsAgeNotIncludingCurrentYear()
        {
            // Arrange
            var now = new DateTime(2020, 04, 12, 04, 30, 00, DateTimeKind.Local);
            var dateOfBirthDate = new DateTime(2000, 04, 13, 0, 0, 0, DateTimeKind.Local);

            // Act
            var calculateAge = CreateCalculateAge(now);
            var calculatedAge = calculateAge.CalculateAgeInMonthsAndYears(dateOfBirthDate);

            // Assert
            calculatedAge.AgeYears.Should().Be(19);
        }

        [TestMethod]
        [DataRow("1980-04-12", "2021-04-10T04:30:00", 40)]
        [DataRow("1980-04-12", "2021-04-11T04:30:00", 40)]
        [DataRow("1980-04-12", "2021-04-12T04:30:00", 41)]
        [DataRow("1980-04-12", "2021-04-13T04:30:00", 41)]
        [DataRow("1980-04-12", "2021-04-14T04:30:00", 41)]
        public void CalculateAgeInMonthsAndYears_DateOfBirthInLeapYear_DoesNotAffectAgeCalculation(
            string dateOfBirth,
            string now,
            int expectedYears)
        {
            // Arrange
            var dateOfBirthDate = DateTime.ParseExact(dateOfBirth, "yyyy-MM-dd", null, DateTimeStyles.AssumeLocal);
            var nowDate = DateTime.ParseExact(now, "yyyy-MM-ddTHH:mm:ss", null, DateTimeStyles.AssumeLocal);

            // Act
            var calculateAge = CreateCalculateAge(nowDate);
            var calculatedAge = calculateAge.CalculateAgeInMonthsAndYears(dateOfBirthDate);

            // Assert
            calculatedAge.AgeYears.Should().Be(expectedYears);
        }

        [TestMethod]
        [DataRow("1981-04-12", "2020-04-10T04:30:00", 38)]
        [DataRow("1981-04-12", "2020-04-11T04:30:00", 38)]
        [DataRow("1981-04-12", "2020-04-12T04:30:00", 39)]
        [DataRow("1981-04-12", "2020-04-13T04:30:00", 39)]
        [DataRow("1981-04-12", "2020-04-14T04:30:00", 39)]
        public void CalculateAgeInMonthsAndYears_CurrentDateInLeapYear_DoesNotAffectAgeCalculation(
            string dateOfBirth,
            string now,
            int expectedYears)
        {
            // Arrange
            var dateOfBirthDate = DateTime.ParseExact(dateOfBirth, "yyyy-MM-dd", null, DateTimeStyles.AssumeLocal);
            var nowDate = DateTime.ParseExact(now, "yyyy-MM-ddTHH:mm:ss", null, DateTimeStyles.AssumeLocal);

            // Act
            var calculateAge = CreateCalculateAge(nowDate);
            var calculatedAge = calculateAge.CalculateAgeInMonthsAndYears(dateOfBirthDate);

            // Assert
            calculatedAge.AgeYears.Should().Be(expectedYears);
        }

        [TestMethod]
        [DataRow("1980-04-12", "2020-04-10T04:30:00", 39)]
        [DataRow("1980-04-12", "2020-04-11T04:30:00", 39)]
        [DataRow("1980-04-12", "2020-04-12T04:30:00", 40)]
        [DataRow("1980-04-12", "2020-04-13T04:30:00", 40)]
        [DataRow("1980-04-12", "2020-04-14T04:30:00", 40)]
        public void CalculateAgeInMonthsAndYears_CurrentDateAndBirthDateInLeapYear_DoesNotAffectAgeCalculation(
            string dateOfBirth,
            string now,
            int expectedYears)
        {
            // Arrange
            var dateOfBirthDate = DateTime.ParseExact(dateOfBirth, "yyyy-MM-dd", null, DateTimeStyles.AssumeLocal);
            var nowDate = DateTime.ParseExact(now, "yyyy-MM-ddTHH:mm:ss", null, DateTimeStyles.AssumeLocal);

            // Act
            var calculateAge = CreateCalculateAge(nowDate);
            var calculatedAge = calculateAge.CalculateAgeInMonthsAndYears(dateOfBirthDate);

            // Assert
            calculatedAge.AgeYears.Should().Be(expectedYears);
        }

        private CalculateAge CreateCalculateAge(DateTime now)
        {
            var timeProviderMock = new Mock<ICurrentDateTimeProvider>();
            timeProviderMock.Setup(x => x.LocalNow).Returns(now);
            return new CalculateAge(timeProviderMock.Object);
        }
    }
}