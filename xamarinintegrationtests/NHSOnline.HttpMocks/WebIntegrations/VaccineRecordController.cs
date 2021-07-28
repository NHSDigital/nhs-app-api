using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace NHSOnline.HttpMocks.WebIntegrations
{
    [Host("nhsd.stubs.local.bitraft.io")]
    public class VaccineRecordController : Controller
    {
        [HttpGet("sso")]
        public IActionResult VaccineRecordPage()
        {
            (string Title, HttpRequest Request) model = ("Vaccine Record Internal Page", Request);
            return View("~/Views/WebIntegrations/VaccineRecordPage.cshtml", model);
        }
    }
}