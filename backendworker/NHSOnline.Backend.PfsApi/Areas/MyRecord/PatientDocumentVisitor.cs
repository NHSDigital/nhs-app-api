using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.PatientRecord;

namespace NHSOnline.Backend.PfsApi.Areas.MyRecord
{
    internal class PatientDocumentVisitor : IPatientDocumentResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetPatientDocumentResult.Success result)
        {
            return new OkObjectResult(result);
        }

        public IActionResult Visit(GetPatientDocumentResult.BadGateway result)
        {
          return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
    }
}