using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Appointments.Models
{
    [TestClass]
    public class AppointmentBookRequestTests
    {
        private static Fixture _fixture;
        private AppointmentBookRequest _systemUnderTest;
        private ValidationContext _context;
        private List<ValidationResult> _validationResults;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            _systemUnderTest = _fixture.Create<AppointmentBookRequest>();
            _context = new ValidationContext(_systemUnderTest, null, null);
            _validationResults = new List<ValidationResult>();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void AppointmentBookRequest_SlotId_Required(string slotId)
        {
            // Arrange
            _systemUnderTest.SlotId = slotId;

            // Act
            var valid = Validator.TryValidateObject(_systemUnderTest, _context, _validationResults, true);

            // Assert
            valid.Should().BeFalse();
            _validationResults.Count.Should().Be(1);
            _validationResults.Single().MemberNames.Should().BeEquivalentTo(nameof(AppointmentBookRequest.SlotId));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void AppointmentBookRequest_BookingReason_Required(string bookingReason)
        {
            // Arrange
            _systemUnderTest.BookingReason = bookingReason;

            // Act
            var valid = Validator.TryValidateObject(_systemUnderTest, _context, _validationResults, true);

            // Assert
            valid.Should().BeFalse();
            _validationResults.Count.Should().Be(1);
            _validationResults.Single().MemberNames.Should().BeEquivalentTo(nameof(AppointmentBookRequest.BookingReason));
        }

        [DataTestMethod]
        [DataRow(151)]
        [DataRow(152)]
        [DataRow(1000)]
        public void AppointmentBookRequest_BookingReason_Above150CharactersInvalid(int stringLength)
        {
            // Arrange
            _systemUnderTest.BookingReason = string.Join(string.Empty, _fixture.CreateMany<char>(stringLength));

            // Act
            var valid = Validator.TryValidateObject(_systemUnderTest, _context, _validationResults, true);

            // Assert

            _systemUnderTest.BookingReason.Length.Should().Be(stringLength);
            valid.Should().BeFalse();
            
            _validationResults.Count.Should().Be(1);
            _validationResults.Single().MemberNames.Should().BeEquivalentTo(nameof(AppointmentBookRequest.BookingReason));
        }

        [DataTestMethod]
        [DataRow(150)]
        [DataRow(149)]
        [DataRow(5)]
        public void AppointmentBookRequest_BookingReason_UpTo150CharactersValid(int stringLength)
        {
            // Arrange
            _systemUnderTest.BookingReason = string.Join(string.Empty, _fixture.CreateMany<char>(stringLength));

            // Act
            var valid = Validator.TryValidateObject(_systemUnderTest, _context, _validationResults, true);

            // Assert

            _systemUnderTest.BookingReason.Length.Should().Be(stringLength);
            valid.Should().BeTrue();
        }
    }
}
