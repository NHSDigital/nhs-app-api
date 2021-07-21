using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Emis
{
    public class EmisDemographicsDefaultBehaviour : IEmisDemographicsBehaviour
    {
        public IActionResult Behave(EmisPatient patient)
        { 
            var name = patient.PersonalDetails.Name;
            
            return new JsonResult(new
            {
                nhsNumber = patient.NhsNumber,
                patientName = $"{name.GivenName} {name.GivenName}",
                dateOfBirth = patient.PersonalDetails.Age.DateOfBirthISO86012004,
                sex = patient.PersonalDetails.Gender,
                address = ""
            });
        }
    }
}