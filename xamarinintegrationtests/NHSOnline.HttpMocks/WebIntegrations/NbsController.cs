using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace NHSOnline.HttpMocks.WebIntegrations
{
    [Host("nbs.stubs.local.bitraft.io")]
    public class NbsController : Controller
    {
        [HttpGet("nhs-login/start-page")]
        public IActionResult NbsInternalPage()
        {
            (string Title, HttpRequest Request) model = ("NBS Internal Page", Request);
            return View("~/Views/WebIntegrations/NbsPage.cshtml", model);
        }
    }
}