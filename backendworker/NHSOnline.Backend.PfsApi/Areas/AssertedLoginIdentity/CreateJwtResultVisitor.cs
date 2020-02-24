using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity;

namespace NHSOnline.Backend.PfsApi.Areas.AssertedLoginIdentity
{
    public class CreateJwtResultVisitor : ICreateJwtResultVisitor<IActionResult>
    {
        public IActionResult Visit(CreateJwtResult.Success result)
        {
            return new CreatedResult(string.Empty, result.Response);
        }

        public IActionResult Visit(CreateJwtResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}