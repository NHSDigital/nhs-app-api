using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.Backend.PfsApi.Areas.Configuration
{
    public class GetConfigurationResultVisitorV2: IGetConfigurationResultVisitorV2<IActionResult>
    {
        public IActionResult Visit(GetConfigurationResultV2.Success result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(GetConfigurationResultV2.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}