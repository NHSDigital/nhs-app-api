using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.HttpMocks.WebIntegrations
{
    [Route("nhsuk")]
    public class NhsUkServicesController : Controller
    {
        [HttpGet("covid")]
        public IActionResult Covid()
        {
            return CreatePage("Covid");
        }

        [HttpGet("covidApp")]
        public IActionResult CovidApp()
        {
            return CreatePage("CovidApp");
        }

        [HttpGet("covid/conditions")]
        public IActionResult CovidConditions()
        {
            return CreatePage("Covid Conditions");
        }

        [HttpGet("healthAtoZ")]
        public IActionResult HealthAtoZ()
        {
            return CreatePage("Health A to Z");
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
            (string Title, string SubTitle, HttpRequest Request) model = (title, "", Request);
            return View("~/Views/WebIntegrations/InternalPage.cshtml", model);
        }
    }
}