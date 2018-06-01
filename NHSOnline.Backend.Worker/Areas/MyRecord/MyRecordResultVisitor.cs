using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.Router.Demographics;

namespace NHSOnline.Backend.Worker.Areas.MyRecord
{
    internal class MyRecordResultVisitor : IMyRecordResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetMyRecordResult.SuccessfullyRetrieved result)
        {
            return new OkObjectResult(result);
        }

        public IActionResult Visit(GetMyRecordResult.SupplierSystemUnavailable result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(GetMyRecordResult.Unsuccessful result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
    }
}