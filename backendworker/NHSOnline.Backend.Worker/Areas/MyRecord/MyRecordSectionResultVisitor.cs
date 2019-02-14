using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.PatientRecord;

namespace NHSOnline.Backend.Worker.Areas.MyRecord
{
    public class MyRecordSectionResultVisitor : IMyRecordSectionResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetMyRecordSectionResult.SuccessfullyRetrieved result)
        {
            return new OkObjectResult(result);
        }

        public IActionResult Visit(GetMyRecordSectionResult.SupplierBadData result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(GetMyRecordSectionResult.Unsuccessful result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(GetMyRecordSectionResult.ErrorProcessingSecurityHeader errorProcessingSecurityHeader)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        public IActionResult Visit(GetMyRecordSectionResult.InvalidUserCredentials invalidUserCredentials)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(GetMyRecordSectionResult.InvalidRequest invalidRequest)
        {
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(GetMyRecordSectionResult.UnknownError unknownError)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(GetMyRecordSectionResult.InternalServerError internalServerError)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}