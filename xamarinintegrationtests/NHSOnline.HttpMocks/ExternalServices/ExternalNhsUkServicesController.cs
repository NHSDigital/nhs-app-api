using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.HttpMocks.ExternalServices
{
    [Route("nhsuk")]
    public class ExternalNhsUkServicesController : Controller
    {

        [HttpGet("covid")]
        public IActionResult Covid()
        {
            return CreatePage("Covid");
        }

        [HttpGet("covid/conditions")]
        public IActionResult CovidConditions()
        {
            return CreatePage("Covid Conditions");
        }

        [HttpGet("help/login")]
        public IActionResult LoginHelp()
        {
            return CreatePage("Login Help");
        }

        [HttpGet("help/home")]
        public IActionResult HomeHelp()
        {
            return CreatePage("Home Help");
        }

        [HttpGet("conditions")]
        public IActionResult Conditions()
        {
            return CreatePage("Conditions");
        }

        [HttpGet("111")]
        public IActionResult OneOneOne()
        {
            return CreatePage("111");
        }

        [HttpGet("111wales")]
        public IActionResult OneOneOneWales()
        {
            return CreatePage("111 Wales");
        }

        [HttpGet("contactus")]
        public IActionResult ContactUs()
        {
            return CreatePage("Contact Us");
        }

        [HttpGet("myhealthonline")]
        public IActionResult MyHealthOnline()
        {
            return CreatePage("My Health Online");
        }

        private IActionResult CreatePage(string title)
        {
            return Content($@"
                <html>
                    <head>
                        <meta charset=""utf-8"">
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1, shrink-to-fit=no"">
                        <link rel=""stylesheet"" href=""https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css"" integrity=""sha384-9aIt2nRpC12Uk9gS9baDl411NQApFmC26EwAOH8WgZl5MYYxFfc+NcPb1dKGj7Sk"" crossorigin=""anonymous"">
                        <title>{title}</title>
                    </head>
                    <body>
                        <div class=""jumbotron"">
                            <h1 class=""display-4"">{title}</h1>
                        </div>
                        <div class=""container"">
                            <ul class=""list-group"">
                                <li class=""list-group-item"">Host: {Request.Host}</li>
                                <li class=""list-group-item"">Path: {Request.Path}</li>
                            </ul>
                            <h2 class=""display-5"">Query String</h2>
                            <ul class=""list-group"">
                                {string.Join("", Request.Query.Select(q => $@"<li class=""list-group-item"">{q.Key}: {q.Value}"))}
                            </ul>
                        </div>
                    </body>
                </html>",
                "text/html");
        }
    }
}