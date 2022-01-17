using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord;

namespace NHSOnline.Backend.PfsApi.Areas.MyRecord
{
    internal class PatientDocumentResultVisitor : IPatientDocumentResultVisitor<IActionResult>
    {
        private readonly ILogger<PatientDocumentController> _logger;
        public PatientDocumentResultVisitor(ILogger<PatientDocumentController> logger)
        {
            _logger = logger;
        }
        public IActionResult Visit(GetPatientDocumentResult.Success result)
        {
            _logger.LogInformation($"Accessing document type {result.Response.Type} which is set to viewable as {result.Response.IsViewable} and downloadable as {result.Response.IsDownloadable}");
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(GetPatientDocumentResult.BadGateway result)
        {
          return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
    }
}