using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Spine;

namespace NHSOnline.HttpMocks.Emis
{
    public sealed class EmisExistingNominatedPharmacyBehaviour : IEmisNominatedPharmacyBehaviour
    {
        public IActionResult Behave(EmisPatient patient)
        {
            return new OkObjectResult(SpineResponseBuilder.GetPdsTraceResponse(
                patient,
                true,
                SpineResponseBuilder.P1PharmacyType));
        }
    }
}

