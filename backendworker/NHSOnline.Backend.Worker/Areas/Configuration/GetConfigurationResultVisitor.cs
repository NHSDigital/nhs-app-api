using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.Backend.Worker.Areas.Configuration
{
    public class GetConfigurationResultVisitor : IGetConfigurationResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetConfigurationResult.SuccessfullyRetrieved result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(GetConfigurationResult.MissingDetailsResult result)
        {
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(GetConfigurationResult.InvalidNativeAppVersionResult result)
        {
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(GetConfigurationResult.InvalidDeviceNameResult result)
        {
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
        
        public IActionResult Visit(GetConfigurationResult.ErrorRetrievingConfigResult result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}