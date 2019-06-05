using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.PfsApi.GpSearch;
using NHSOnline.Backend.PfsApi.GpSearch.Models;

namespace NHSOnline.Backend.PfsApi.UnitTests.GpSearch.Models
{
    [TestClass]
    public class OrganisationModelTests
    {
        [TestMethod]
        public void OrganisationModel_GetOpeningTimesArray_ShouldReturnArrayOfOpeningTimes()
        {
            var model = new Organisation
            {
                OpeningTimes =
                    "[{\"WeekDay\":\"Monday\",\"Times\":\"09:00 - 18:00\",\"OpeningTimeType\":\"General\",\"AdditionalOpeningDate\":\"\",\"IsOpen\":true},"
                    +
                    "{\"WeekDay\":\"Tuesday\",\"Times\":\"09:00 - 18:00\",\"OpeningTimeType\":\"General\",\"AdditionalOpeningDate\":\"\",\"IsOpen\":true}]"
            };

            var openingTimesArray = model.GetOpeningTimesArray();

            openingTimesArray.Should().HaveCount(2);
            openingTimesArray.ToList()[0].WeekDay.Should().BeEquivalentTo(ResponseEnums.WeekDay.Monday);
        }

        [TestMethod]
        public void OrganisationModel_GetContactsArray_ShouldReturnArrayOfContactInformation()
        {
            var model = new Organisation
            {
                Contacts = "[{\"OrganisationContactType\":\"Primary\",\"OrganisationContactAvailabilityType\":\"Office hours\",\"OrganisationContactMethodType\":\"Telephone\",\"OrganisationContactValue\":\"01752 361 641\"},"
                +
                "{\"OrganisationContactType\":\"Primary\",\"OrganisationContactAvailabilityType\":\"Office hours\",\"OrganisationContactMethodType\":\"Email\",\"OrganisationContactValue\":\"nhspharmacy.plymouth.wellpharmacyfw039@nhs.net\"}]" 
            };

            var contactsArray = model.GetContactsArray();

            contactsArray.Should().HaveCount(2);
            contactsArray.ToList()[0].OrganisationContactValue.Should().BeEquivalentTo("01752 361 641");
        }
    }
}
