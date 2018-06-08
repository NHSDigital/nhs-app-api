using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;

namespace NHSOnline.Backend.Worker.Areas.MyRecord
{
    internal class AllergyResultVisitor : IAllergyResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetAllergyResult.UserHasNoAccess result)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }
        
        public IActionResult Visit(GetAllergyResult.SuccessfullyRetrieved result)
        {
            return new OkObjectResult(result);
        }

        public IActionResult Visit(GetAllergyResult.Unsuccessful result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(GetAllergyResult.SupplierBadData result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}