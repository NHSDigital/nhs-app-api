using System;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Spine
{
    [Route("spine")]
    public class SpineController : Controller
    {
        private readonly IPatients _patients;

        public SpineController(IPatients patients)
        {
            _patients = patients;
        }

        [Produces("application/xml")]
        [HttpPost("sync-service")]
        public IActionResult SyncService([FromQuery] string? userPatientLinkToken)
        {
            if (!ModelState.IsValid || userPatientLinkToken == null || !userPatientLinkToken.Contains("linktoken_", StringComparison.InvariantCulture))
            {
                return BadRequest(ModelState);
            }

            var patientId = userPatientLinkToken.Replace("linktoken_", string.Empty, StringComparison.InvariantCulture);
            var patient = _patients.LookupById(patientId);

            if (patient is EmisPatient emisPatient)
            {
                var behaviour = emisPatient.Behaviours.Get<ISpineBehaviour>(() => new SpineDefaultBehaviour());
                return behaviour.Behave();
            }

            return BadRequest($"Patent '{patientId}' ({patient?.GetType().Name ?? "Unknown"}) is not an EMIS patient");
        }
    }
}

