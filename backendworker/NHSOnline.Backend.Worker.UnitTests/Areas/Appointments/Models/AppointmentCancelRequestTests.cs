using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Appointments.Models
{
    [TestClass]
    public class AppointmentCancelRequestTests
    {
        private Fixture _fixture;
        private AppointmentCancelRequest _systemUnderTest;
        private ValidationContext _context;
        private List<ValidationResult> _validationResults;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            _systemUnderTest = _fixture.Create<AppointmentCancelRequest>();
            _context = new ValidationContext(_systemUnderTest, null, null);
            _validationResults = new List<ValidationResult>();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void AppointmentBookRequest_AppointmentId_Required(string appointmentId)
        {
            // Arrange
            _systemUnderTest.AppointmentId = appointmentId;

            // Act
            var valid = Validator.TryValidateObject(_systemUnderTest, _context, _validationResults, true);

            // Assert
            valid.Should().BeFalse();
            _validationResults.Count.Should().Be(1);
            _validationResults.Single().MemberNames.Should().BeEquivalentTo(nameof(AppointmentCancelRequest.AppointmentId));
        }
    }
}