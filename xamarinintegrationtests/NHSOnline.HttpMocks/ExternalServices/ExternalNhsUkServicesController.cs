using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.HttpMocks.ExternalServices
{
    [Route("nhsuk")]
    public class ExternalNhsUkServicesController : Controller
    {

        [HttpGet("covid")]
        public IActionResult Covid()
        {
            return Content($@"
                <html>
                    <body>
                        <h1>Covid</h1>
                    </body>
                </html>",
                "text/html");
        }

        [HttpGet("covid/conditions")]
        public IActionResult CovidConditions()
        {
            return Content($@"
                <html>
                    <body>
                        <h1>Covid Conditions</h1>
                    </body>
                </html>",
                "text/html");
        }

        [HttpGet("help/login")]
        public IActionResult LoginHelp()
        {
            return Content($@"
                <html>
                    <body>
                        <h1>Login Help</h1>
                    </body>
                </html>",
                "text/html");
        }

        [HttpGet("conditions")]
        public IActionResult Conditions()
        {
            return Content($@"
                <html>
                    <body>
                        <h1>Conditions</h1>
                    </body>
                </html>",
                "text/html");
        }

        [HttpGet("111")]
        public IActionResult OneOneOne()
        {
            return Content($@"
                <html>
                    <body>
                        <h1>111</h1>
                    </body>
                </html>",
                "text/html");
        }
    }
}