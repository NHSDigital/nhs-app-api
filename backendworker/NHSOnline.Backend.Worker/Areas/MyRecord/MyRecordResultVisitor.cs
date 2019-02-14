using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.PatientRecord;

namespace NHSOnline.Backend.Worker.Areas.MyRecord
{
    internal class MyRecordResultVisitor : IMyRecordResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetMyRecordResult.SuccessfullyRetrieved result)
        {
            return new OkObjectResult(result);
        }

        public IActionResult Visit(GetMyRecordResult.SupplierBadData result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(GetMyRecordResult.Unsuccessful result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(GetMyRecordResult.ErrorProcessingSecurityHeader errorProcessingSecurityHeader)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        public IActionResult Visit(GetMyRecordResult.InvalidUserCredentials invalidUserCredentials)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(GetMyRecordResult.InvalidRequest invalidRequest)
        {
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(GetMyRecordResult.UnknownError unknownError)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(GetMyRecordResult.InternalServerError internalServerError)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}