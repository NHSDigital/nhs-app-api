using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.PatientRecord;

namespace NHSOnline.Backend.PfsApi.Areas.MyRecord
{
    internal class DetailedTestResultVisitor : IDetailedTestResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetDetailedTestResult.Success result)
        {
            return new OkObjectResult(result);
        }

        public IActionResult Visit(GetDetailedTestResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
    }
}