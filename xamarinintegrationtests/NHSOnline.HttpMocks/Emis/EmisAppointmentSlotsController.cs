using System;
using System.Collections;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Emis.Models;

namespace NHSOnline.HttpMocks.Emis
{
    [Route("emis")]
    public class EmisAppointmentSlotsController : Controller
    {
        private readonly IPatients _patients;

        public EmisAppointmentSlotsController(IPatients patients)
        {
            _patients = patients;
        }

        [HttpGet("appointmentslots/meta")]
        public IActionResult AppointmentSlotsMeta([FromQuery] string? userPatientLinkToken)
        {
            if (!ModelState.IsValid || userPatientLinkToken == null || !userPatientLinkToken.Contains("linktoken_", StringComparison.InvariantCulture))
            {
                return BadRequest(ModelState);
            }

            return Json(new
            {
                SessionHolders = new ArrayList(),
                Locations = new ArrayList
                {
                    new Location
                    {
                        LocationId = "1",
                        LocationName = "Xamarin Practice"
                    }
                },
                Sessions = new ArrayList
                {
                   new ASession
                   {
                       SessionId = "123",
                       LocationId = "1"
                   }
                }
            });
        }

        [HttpGet("appointmentslots")]
        public IActionResult AppointmentSlots([FromQuery] string userPatientLinkToken)
        {
            var patientId = userPatientLinkToken.Replace("linktoken_", string.Empty, StringComparison.InvariantCulture);
            var patient = _patients.LookupById(patientId);
            if (patient is EmisPatient emisPatient)
            {
                var behaviour = emisPatient.Behaviours.Get<IEmisAppointmentSlotsBehaviour>(() => new EmisAppointmentSlotsDefaultBehaviour());
                return behaviour.Behave();
            }

            return BadRequest($"Patent '{patientId}' ({patient?.GetType().Name ?? "Unknown"}) is not an EMIS patient");
        }
    }
}