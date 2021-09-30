using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.HttpMocks.WebIntegrations
{
    [Route("external")]
    public class NhsUkExternalServicesController : Controller
    {
        [HttpGet("nhsuk/nhs-app-contact-us")]
        public IActionResult ContactUsForm()
        {
            (string Title, HttpRequest Request) model = ("Contact Us", Request );
            return View("~/Views/NhsUk/NhsUkContactUs.cshtml", model );
        }
    }
}