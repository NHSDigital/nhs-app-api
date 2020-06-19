using System;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Emis.Models;

namespace NHSOnline.HttpMocks.Emis
{
    [Route("emis/sessions")]
    public class EmisSessionController : Controller
    {
        private readonly IPatients _patients;

        public EmisSessionController(IPatients patients)
        {
            _patients = patients;
        }

        [HttpPost("endusersession")]
        public IActionResult CreateEndUserSession()
        {
            return Json(new {EndUserSessionId = Guid.NewGuid().ToString()});
        }

        [HttpPost]
        public IActionResult CreateSession([FromBody]SessionsPostRequest body)
        {
            if (!ModelState.IsValid || body?.AccessIdentityGuid == null)
            {
                return BadRequest(ModelState);
            }

            string accessIdentityGuid = body.AccessIdentityGuid;
            var patient = _patients.LookupById(accessIdentityGuid);
            if (patient is EmisPatient emisPatient)
            {
                return CreateSession(emisPatient);
            }

            return BadRequest($"Patent '{accessIdentityGuid}' ({patient?.GetType().Name ?? "Unknown"}) is not an EMIS patient");
        }

        private IActionResult CreateSession(EmisPatient patient)
        {
            var name = patient.PersonalDetails.Name;
            var userPatientLinks = new[]
            {
                new
                {
                    name.Title,
                    FirstName = name.GivenName,
                    Surname = name.FamilyName,
                    patient.UserPatientLinkToken,
                    patient.OdsCode,
                    NationalPracticeCode = patient.OdsCode,
                    AssociationType = "Self"
                }
            };

            return Json(new
            {
                ApplicationLinkLevel = "Linked",
                LastAccessTime = DateTime.Now.AddDays(-1),
                UserPatientLinks = userPatientLinks,
                patient.SessionId,
                name.Title,
                FirstName = name.GivenName,
                Surname = name.FamilyName
            });
        }
    }
}
