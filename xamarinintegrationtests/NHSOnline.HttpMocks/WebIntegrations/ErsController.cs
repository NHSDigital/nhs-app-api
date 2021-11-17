using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace NHSOnline.HttpMocks.WebIntegrations
{
    [Host("ers.stubs.local.bitraft.io")]
    public class ErsController : Controller
    {
        [HttpGet("nhslogin")]
        public IActionResult NhsLogin()
        {
            (string Title, HttpRequest Request) model = ("eRS Web Integration", Request);
            return View("~/Views/WebIntegrations/ErsNhsLogin.cshtml", model);
        }

        [HttpGet("internal/page.html")]
        public IActionResult InternalPage()
        {
            (string Title, HttpRequest Request) model = ("eRS Internal Page", Request);
            return View("~/Views/WebIntegrations/ErsInternalPage.cshtml", model);
        }
    }
}
