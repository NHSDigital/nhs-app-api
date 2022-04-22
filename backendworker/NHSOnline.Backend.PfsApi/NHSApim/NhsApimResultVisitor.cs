using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.Backend.PfsApi.NHSApim
{
    public class NhsApimResultVisitor : INhsApimResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetAuthTokenResult.Success result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(GetAuthTokenResult.BadRequest result)
        {
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(GetAuthTokenResult.Forbidden result)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }
    }
}