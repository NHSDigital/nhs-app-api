using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.Router.Prescriptions;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions
{
    internal class PrescriptionResultVisitor : IPrescriptionResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetPrescriptionsResult.SuccessfullyRetrieved result)
        {
            return new OkObjectResult(result);
        }

        public IActionResult Visit(GetPrescriptionsResult.Unsuccessful result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(GetPrescriptionsResult.SupplierBadData result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}