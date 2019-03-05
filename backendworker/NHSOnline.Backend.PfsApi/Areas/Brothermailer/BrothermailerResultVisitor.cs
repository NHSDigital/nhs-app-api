using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.PfsApi.Brothermailer;
using NHSOnline.Backend.PfsApi.Brothermailer.Models;

namespace NHSOnline.Backend.PfsApi.Areas.Brothermailer
{
    public class BrothermailerResultVisitor: IBrothermailerResultVisitor<IActionResult>
    {
        public IActionResult Visit(BrothermailerResult.SuccessfullyRetrieved result)
        {
            return new OkResult();
        }

        public IActionResult Visit(BrothermailerResult.Unsuccessful result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        
        public IActionResult Visit(BrothermailerResult.BrothermailerServiceUnavailable result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(BrothermailerResult.BadRequest result)
        {
            return new BadRequestResult();
        }
    }
}