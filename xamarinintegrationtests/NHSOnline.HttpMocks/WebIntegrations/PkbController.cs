using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Linq;

namespace NHSOnline.HttpMocks.WebIntegrations
{
    [Host("pkb.stubs.local.bitraft.io")]
    public class PkbController : Controller
    {
        [HttpGet("nhs-login/login")]
        public IActionResult InternalPage()
        {
            return Content($@"
                <html>
                    <head>
                        <meta charset='utf-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1, shrink-to-fit=no'>
                        <link rel='stylesheet' href='https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css' integrity='sha384-9aIt2nRpC12Uk9gS9baDl411NQApFmC26EwAOH8WgZl5MYYxFfc+NcPb1dKGj7Sk' crossorigin='anonymous'>
                        <title>Pkb Internal Page</title>
                        <script type='text/javascript' src='http://web.local.bitraft.io:3000/js/v1/nhsapp.js'></script>
                    </head>
                    <body>
                        <div class='jumbotron'>
                            <h1 class='display-4'>Pkb Internal Page</h1>
                        </div>
                        <div class='container'>
                            <h2 class='display-5'>Headers</h2>
                            <ul class='list-group'>
                                <li class='list-group-item'>Host: {Request.Host}</li>
                                <li class='list-group-item'>Path: {Request.Path}</li>
                            </ul>
                            <h2 class='display-5'>Query String</h2>
                            <ul class='list-group'>
                                {string.Join(string.Empty, Request.Query.Select(q => $@"<li class='list-group-item'>{q.Key}: {q.Value}"))}
                            </ul>
                            <h2 class='display-5'>Headers</h2>
                            <ul class='list-group'>
                                {string.Join(string.Empty, Request.Headers.Select(q => $@"<li class='list-group-item'>{q.Key}: {q.Value}"))}
                            </ul>
                        </div>
                    </body>
                </html>",
                "text/html");
        }
    }
}
