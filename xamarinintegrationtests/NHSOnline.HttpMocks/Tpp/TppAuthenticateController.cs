using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Tpp.Models;

namespace NHSOnline.HttpMocks.Tpp
{
    [Route("Tpp")]
    public sealed class TppAuthenticateController: Controller
    {
        private readonly IPatients _patients;

        public TppAuthenticateController(IPatients patients)
        {
            _patients = patients;
        }

        [Produces("application/xml")]
        [HttpPost]
        [TppTypeHeader("Authenticate")]
        public IActionResult Authenticate([FromBody] Authenticate request)
        {
            if (!ModelState.IsValid || request?.AccountId == null)
            {
                return BadRequest(ModelState);
            }

            var patientId = request.AccountId;
            var patient = _patients.LookupById(request.AccountId);
            if (patient is TppPatient tppPatient)
            {
                return Authenticate(tppPatient);
            }

            return BadRequest($"Patent '{patientId}' ({patient?.GetType().Name ?? "Unknown"}) is not a TPP patient");
        }

        private IActionResult Authenticate(TppPatient patient)
        {
            var name = patient.PersonalDetails.Name;
            var person = new Person
            {
                PatientId = patient.Id,
                DateOfBirth = patient.PersonalDetails.Age.DateOfBirth.ToString("yyyy-MM-dd'T'HH:mm:ss.f'Z'", CultureInfo.InvariantCulture),
                Gender = patient.PersonalDetails.Gender.ToString(),
                NationalId = new NationalId
                {
                    Type = "NHS",
                    Value = patient.NhsNumber.FormattedStringValue
                },
                PersonName = new PersonName
                {
                    Name = $"{name.Title} {name.GivenName} {name.FamilyName}"
                }
            };

            var authenticateReply = new AuthenticateReply
            {
                PatientId = patient.Id,
                OnlineUserId = patient.Id,
                User = new User { Person = person },
                Uuid = new Guid(),
                Suid = patient.Id,
                People = { person },
                Registration = new Registration
                {
                    PatientAccess = 
                    {
                        new PatientAccess
                        {
                            PatientId = patient.Id,
                            SiteDetails = new SiteDetails
                            {
                                UnitName = "Practice X",
                                Address = new Address
                                {
                                    AddressText = "1 Cool Drive, Leeds",
                                }
                            }
                        }
                    },
                }
            };

            Request.HttpContext.Response.Headers.Add("Suid", patient.Id);
            return Ok(authenticateReply);
        }
    }
}