using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Emis
{
    [Route("emis/demographics")]
    public class EmisDemographicsController : Controller
    {
        private readonly IPatients _patients;

        public EmisDemographicsController(IPatients patients)
        {
            _patients = patients;
        }

        [HttpGet]
        public IActionResult Demographics([FromQuery] string? userPatientLinkToken)
        {
            if (!ModelState.IsValid || userPatientLinkToken == null || !userPatientLinkToken.Contains("linktoken_", StringComparison.InvariantCulture))
            {
                return BadRequest(ModelState);
            }

            var patientId = userPatientLinkToken.Replace("linktoken_", string.Empty, StringComparison.InvariantCulture);
            var patient = _patients.LookupById(patientId);
            if (patient is EmisPatient emisPatient)
            {
                var behaviour = emisPatient.Behaviours.Get<IEmisDemographicsBehaviour>(() => new EmisDemographicsDefaultBehaviour());
                return behaviour.Behave(emisPatient);
            }

            return BadRequest($"Patent '{patientId}' ({patient?.GetType().Name ?? "Unknown"}) is not an EMIS patient");
        }
    }
}