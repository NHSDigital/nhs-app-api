using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace NHSOnline.HttpMocks.WebIntegrations
{
    [Host("silverintegrationtestprovider.stubs.local.bitraft.io")]
    public class SilverIntegrationTestProviderController : Controller
    {
        [HttpGet("index.html")]
        public IActionResult InternalPage()
        {
            return Content($@"
                <html>
                    <head>
                        <meta charset='utf-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1, shrink-to-fit=no'>
                        <link rel='stylesheet' href='https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css' integrity='sha384-9aIt2nRpC12Uk9gS9baDl411NQApFmC26EwAOH8WgZl5MYYxFfc+NcPb1dKGj7Sk' crossorigin='anonymous'>
                        <title>Silver Integration Test Provider Web Integration Internal Page</title>
                    </head>
                    <body>
                        <div class='jumbotron'>
                            <h1 class='display-4'>Silver Integration Test Provider Internal Page</h1>
                        </div>
                    </body>
                </html>",
                "text/html");
        }
    }
}
