using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Emis;

namespace NHSOnline.HttpMocks.Spine
{
    [Route("spine")]
    public class SpineNominatedPharmacyController : Controller
    {
        private readonly IPatients _patients;

        public SpineNominatedPharmacyController(IPatients patients)
        {
            _patients = patients;
        }

        [HttpPost]
        [Route("sync-service")]
        public async Task<IActionResult> GeneralRequest()
        {
            using var reader = new StreamReader(Request.Body);
            var xmlBody = await reader.ReadToEndAsync();

            var data = SpineResponseBuilder.GetBetween(xmlBody, "person.id", "/></person.id");
            var nhsNumber = SpineResponseBuilder.GetBetween(data, "extension=\"", "\"");

            if (SpineResponseBuilder.IsNominatedPharmacyUpdateRequest(xmlBody))
            {
                return Ok();
            }

            var patient = _patients.LookupByNhsNumber(nhsNumber);

            if (patient == null)
            {
                return BadRequest("Patient cannot be found");
            }

            if (patient is EmisPatient emisPatient)
            {
                var behaviour = emisPatient.Behaviours.Get<IEmisNominatedPharmacyBehaviour>(() => new EmisExistingNominatedPharmacyBehaviour());
                return behaviour.Behave(emisPatient);
            }

            return BadRequest($"Patient '{patient?.Id}' ({patient?.GetType().Name ?? "Unknown"}) is not an EMIS patient");
        }
    }
}
