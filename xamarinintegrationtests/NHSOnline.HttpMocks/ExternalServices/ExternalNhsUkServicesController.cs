using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.HttpMocks.ExternalServices
{
    [Route("nhsuk")]
    public class ExternalNhsUkServicesController : Controller
    {

        [HttpGet("covid")]
        public IActionResult CovidService()
        {
            return Content($@"
                <html>
                    <body>
                        <h1>Covid Service</h1>
                    </body>
                </html>",
                "text/html");
        }

        [HttpGet("conditions")]
        public IActionResult ConditionsService()
        {
            return Content($@"
                <html>
                    <body>
                        <h1>Conditions Service</h1>
                    </body>
                </html>",
                "text/html");
        }

        [HttpGet("111")]
        public IActionResult OneOneOneService()
        {
            return Content($@"
                <html>
                    <body>
                        <h1>111 Service</h1>
                    </body>
                </html>",
                "text/html");
        }
    }
}