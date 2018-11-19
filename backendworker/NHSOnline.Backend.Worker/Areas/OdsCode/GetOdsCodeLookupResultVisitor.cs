using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.Backend.Worker.Areas.OdsCode
{
    public class GetOdsCodeLookupResultVisitor : IGetOdsCodeLookupResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetOdsCodeLookupResult.SuccessfullyRetrieved result)
        {
            return new OkObjectResult(result.Response);
        }
        
        public IActionResult Visit(GetOdsCodeLookupResult.ErrorRetrievingOdsCode result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}