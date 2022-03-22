using System;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Emis
{
    [Route("emis")]
    public class EmisCoursesController: Controller
    {
        private readonly IPatients _patients;

        public EmisCoursesController(IPatients patients)
        {
            _patients = patients;
        }

        [HttpGet("courses")]
        public IActionResult GetPrescriptions([FromQuery] string? userPatientLinkToken)
        {
            if (!ModelState.IsValid || userPatientLinkToken == null || !userPatientLinkToken.Contains("linktoken_", StringComparison.InvariantCulture))
            {
                return BadRequest(ModelState);
            }

            var patientId = userPatientLinkToken.Replace("linktoken_", string.Empty, StringComparison.InvariantCulture);
            var patient = _patients.LookupById(patientId);

            if (patient is EmisPatient emisPatient)
            {
                var behaviour = emisPatient.Behaviours.Get<IEmisCoursesBehaviour>(() => new EmisCoursesDefaultBehaviour());
                return behaviour.Behave();
            }

            return BadRequest($"Patent '{patientId}' ({patient?.GetType().Name ?? "Unknown"}) is not an EMIS patient");
        }

        [HttpPost("prescriptionrequests")]
        public IActionResult Post()
        {
            return Ok();
        }
    }
}