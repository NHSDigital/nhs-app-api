using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.Backend.NominatedPharmacy.Models
{
    public class UpdateNominatedPharmacyResponseVisitor  : IUpdateNominatedPharmacyResponseVisitor<IActionResult>
    {
        public IActionResult Visit(UpdateNominatedPharmacyResponse.Success result)
        {
            return new OkResult();
        }

        public IActionResult Visit(UpdateNominatedPharmacyResponse.GetNominatedPharmacyFailure result)
        {
            return new StatusCodeResult(result.StatusCode.StatusCode);
        }

        public IActionResult Visit(UpdateNominatedPharmacyResponse.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        public IActionResult Visit(UpdateNominatedPharmacyResponse.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(UpdateNominatedPharmacyResponse.UpdatedButStillOldCode result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(UpdateNominatedPharmacyResponse.NominatedPharmacyIsDisabled result)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(UpdateNominatedPharmacyResponse.BadRequest result)
        {
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
    }
}