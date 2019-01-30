using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.Models
{
    [TestClass]
    public class ContactDetailsTests
    {
        [TestMethod]
        public void GetTelephoneArray_WhenPatientHasTelephoneNumbers_ReturnsTelephoneNumbersArray()
        {
            // Arrange
            var contactDetails = new ContactDetails()
            {
                TelephoneNumber = "   01243254363  ",
                MobileNumber = "    "
            };

            var expectedResult = new[]  {
                new PatientTelephoneNumber(){TelephoneNumber = "01243254363"}
            };

            // Act
            var actualResult =
                contactDetails.GetTelephoneArray();

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }
    }
}