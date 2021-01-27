using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Vision.Models;

namespace NHSOnline.HttpMocks.Vision
{
    [Route("vision")]
    public class VisionPatientFacingServicesController : Controller
    {
        private readonly IPatients _patients;
        private readonly IEnumerable<IVisionSoapRequestHandler> _handlers;

        public VisionPatientFacingServicesController(
            IPatients patients,
            IEnumerable<IVisionSoapRequestHandler> handlers)
        {
            _patients = patients;
            _handlers = handlers;
        }

        [Route("PatientFacingServices")]
        [Produces("text/xml")]
        [HttpPost]
        public IActionResult SoapRequest([FromBody] VisionRequestEnvelope request)
        {
            var method = request?.Body?.VisionRequest?.ServiceDefinition?.Name;
            var patientId = request?.Body?.VisionRequest?.ServiceHeader?.Credentials?.RosuAccountId;
            if (!ModelState.IsValid || method == null || patientId == null)
            {
                return BadRequest(ModelState);
            }

            var patient = _patients.LookupById(patientId);
            if (patient is VisionPatient visionPatient)
            {
                return SoapRequest(method, visionPatient);
            }

            return BadRequest($"Patent '{patientId}' ({patient?.GetType().Name ?? "Unknown"}) is not a Vision patient");
        }

        private IActionResult SoapRequest(string method, VisionPatient patient)
        {
            foreach (var handler in _handlers.Where(h => h.Method == method))
            {
                var response = handler.Handle(patient);
                return Ok(response);
            }

            return BadRequest($"Method not supported: {method}");
        }
    }
}