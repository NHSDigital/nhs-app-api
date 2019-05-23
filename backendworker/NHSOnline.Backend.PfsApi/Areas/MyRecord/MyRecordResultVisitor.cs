using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.PatientRecord;

namespace NHSOnline.Backend.PfsApi.Areas.MyRecord
{
    internal class MyRecordResultVisitor : IMyRecordResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetMyRecordResult.Success result)
        {
            return new OkObjectResult(result);
        }

        public IActionResult Visit(GetMyRecordResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(GetMyRecordResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}