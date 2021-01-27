using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Microtest
{
    [Route("microtest/patient/demographics")]
    public class MicrotestDemographicsController : Controller
    {
        private readonly IPatients _patients;

        public MicrotestDemographicsController(IPatients patients)
        {
            _patients = patients;
        }

        [HttpGet]
        public IActionResult Demographics()
        {
            if (!Request.Headers.TryGetValue("NHSO-Nhs-Number", out var nhsNumber))
            {
                return BadRequest("Missing NHSO-Nhs-Number header");
            }

            var patient = _patients.LookupByNhsNumber(nhsNumber);

            if (patient is MicrotestPatient emisPatient)
            {
                return Demographics(emisPatient);
            }

            return BadRequest(
                $"Patent '{nhsNumber}' ({patient?.GetType().Name ?? "Unknown"}) is not an Microtest patient");
        }

        private IActionResult Demographics(MicrotestPatient patient)
        {
            var personalDetails = patient.PersonalDetails;
            var name = personalDetails.Name;
            return Json(new
                {
                    demographics = new
                    {
                        patient_recno = 9,
                        title = name.Title,
                        surname = name.FamilyName,
                        forenames1 = name.GivenName,
                        forenames2 = string.Empty,
                        dob = personalDetails.Age.DateOfBirthISO86012004,
                        sex = personalDetails.Gender.ToString(),
                        nhs = patient.NhsNumber.StringValue,
                        housename = "",
                        roadname = "3 Orchard Close",
                        locality = "Barton-Le-Clay",
                        post_town = "Bedford",
                        county = "Norfold",
                        postcode = "MK45 4LD",
                        email = personalDetails.ContactDetails.EmailAddress,
                        telephone1 = "1234567890",
                        telephone1_type = "",
                        telephone2 = "2345678901",
                        telephone2_type = "",
                        nominated_pharmacy = "",
                        nominated_pharmacy_name = "",
                        appliance_contractor = "",
                        appliance_contractor_name = "",
                        assigned_gp = "Dr P Disorderly",
                        usual_gp = "Dr P Disorderly",
                        notes_location_name = "Cherry Practice Surgery"
                    }
                }
            );
        }
    }
}