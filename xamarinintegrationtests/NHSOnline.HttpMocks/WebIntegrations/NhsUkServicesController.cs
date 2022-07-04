using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.HttpMocks.WebIntegrations
{
    [Route("nhsuk")]
    [Route("external/nhsuk")]
    public class NhsUkServicesController : Controller
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

        [HttpGet("digitalCovidPass")]
        public IActionResult DigitalCovidPass()
        {
            return CreatePage("Digital Covid Pass");
        }

        [HttpGet("healthAtoZ")]
        public IActionResult HealthAtoZ()
        {
            return CreatePage("Health A to Z");
        }

        [HttpGet("help-and-support/{*path}")]
        public IActionResult HelpAndSupport(string? path = null)
        {
            return View("~/Views/NhsUk/NhsUkHelpAndSupport.cshtml", path ?? "/");
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

        [HttpGet("online-consultations")]
        public IActionResult OnlineConsultations()
        {
            return CreatePage("Online Consultations");
        }

        [HttpGet("privacy")]
        public IActionResult PrivacyPolicy()
        {
            return CreatePage("NHS App privacy policy");
        }

        [HttpGet("{*path}")]
        public IActionResult UndefinedRoute(string? path = null)
        {
            return CreatePage($"Route not mocked", $"The path '{(path ?? "/")}' has not been mocked" );
        }

        private IActionResult CreatePage(string title, string subTitle = "")
        {
            (string Title, string SubTitle, HttpRequest Request) model = (title, subTitle, Request);
            return View("~/Views/WebIntegrations/InternalPage.cshtml", model);
        }
    }
}