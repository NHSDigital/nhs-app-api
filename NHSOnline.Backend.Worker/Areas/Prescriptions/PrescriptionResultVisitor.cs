using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.Router.Prescription;

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
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
    }
}