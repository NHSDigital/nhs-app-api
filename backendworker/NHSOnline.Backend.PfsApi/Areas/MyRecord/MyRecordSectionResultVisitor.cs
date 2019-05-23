using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.PatientRecord;

namespace NHSOnline.Backend.PfsApi.Areas.MyRecord
{
    public class MyRecordSectionResultVisitor : IMyRecordSectionResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetMyRecordSectionResult.Success result)
        {
            return new OkObjectResult(result);
        }

        public IActionResult Visit(GetMyRecordSectionResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(GetMyRecordSectionResult.BadRequest result)
        {
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(GetMyRecordSectionResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}