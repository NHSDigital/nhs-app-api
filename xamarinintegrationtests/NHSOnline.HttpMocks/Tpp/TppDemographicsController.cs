using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Extensions;
using NHSOnline.HttpMocks.Tpp.Models;

namespace NHSOnline.HttpMocks.Tpp
{
    [Route("tpp")]
    public class TppDemographicsController : TppBaseController
    {
        public TppDemographicsController(IPatients patients) : base(patients)
        {
        }

        [HttpPost]
        [TppTypeHeader("PatientSelected")]
        public IActionResult Demographics([FromBody] PatientSelected request)
        {
            if (!ModelState.IsValid || request?.PatientId == null)
            {
                return BadRequest(ModelState);
            }

            var patientId = request.PatientId;
            var patient = GetPatients().LookupById(request.PatientId);
            if (patient is TppPatient tppPatient)
            {
                return ReturnXmlResult(patientId, BuildPatientSelectedResult(tppPatient));
            }

            return BadRequest($"Patent '{patientId}' ({patient?.GetType().Name ?? "Unknown"}) is not a TPP patient");
        }

        private string BuildPatientSelectedResult(TppPatient patient)
        {
            var patientSelectedReply = new PatientSelectedReply
            {
                Person = TppDefaults.DefaultTppPerson(patient)
            };

            return XmlHelper.SerializeXml<PatientSelectedReply>(patientSelectedReply);
        }
    }
}