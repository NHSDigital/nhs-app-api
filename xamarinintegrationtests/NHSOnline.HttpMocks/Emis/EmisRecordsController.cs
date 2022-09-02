using System;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Emis
{
    [Route("emis")]
    public class EmisRecordsController : Controller
    {
        private readonly IPatients _patients;

        public EmisRecordsController(IPatients patients)
        {
            _patients = patients;
        }
        [HttpGet("record")]
        public IActionResult RecordsGet([FromQuery] string? userPatientLinkToken)
        {
            if (!ModelState.IsValid || userPatientLinkToken == null || !userPatientLinkToken.Contains("linktoken_", StringComparison.InvariantCulture))
            {
                return BadRequest(ModelState);
            }

            var patientId = userPatientLinkToken.Replace("linktoken_", string.Empty, StringComparison.InvariantCulture);
            var patient = _patients.LookupById(patientId);

            if (patient is EmisPatient emisPatient)
            {
                var behaviour = emisPatient.Behaviours.Get<IEmisRecordsBehaviour>(() => new EmisRecordsDefaultBehaviour());
                return behaviour.Behave();
            }

            return BadRequest($"Patent '{patientId}' ({patient?.GetType().Name ?? "Unknown"}) is not an EMIS patient");
        }
    }
}