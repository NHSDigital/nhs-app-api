using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Emis
{
    public class EmisDemographicsLinkedProfileBehaviour: IEmisDemographicsLinkedProfileBehaviour
    {
        public IActionResult Behave(EmisPatient patient)
        {
            var name = patient.PersonalDetails.Name;

            return new JsonResult(new
            {
                nhsNumber = patient.NhsNumber,
                patientName = $"{name.GivenName} {name.GivenName}",
                firstName = name.GivenName,
                surname = name.FamilyName,
                dateOfBirth = patient.PersonalDetails.Age.DateOfBirthISO86012004,
                sex = patient.PersonalDetails.Gender,
                address = "",
                patientIdentifiers = new []{
                    new {
                        identifierValue = patient.NhsNumber.StringValue,
                        identifierType = "NhsNumber"
                    }
                }
            });
        }
    }
}