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

        public IActionResult Visit(GetAuthTokenResult.Unauthorized result)
        {
            return new StatusCodeResult(StatusCodes.Status401Unauthorized);
        }
    }
}