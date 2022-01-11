using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.PatientRecord;

namespace NHSOnline.Backend.PfsApi.Areas.MyRecord
{
    internal class HistoricTestResultsVisitor : IHistoricTestResultsVisitor<IActionResult>
    {
        public IActionResult Visit(GetHistoricTestResultsResult.Success result)
        {
            return new OkObjectResult(result);
        }

        public IActionResult Visit(GetHistoricTestResultsResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
    }
}
