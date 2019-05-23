using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.Backend.PfsApi.Areas.Configuration
{
    public class GetConfigurationResultVisitor : IGetConfigurationResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetConfigurationResult.Success result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(GetConfigurationResult.BadRequest result)
        {
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
        
        public IActionResult Visit(GetConfigurationResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}