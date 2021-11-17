using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace NHSOnline.HttpMocks.WebIntegrations
{
    [Host("deeplinklauncher.stubs.local.bitraft.io")]
    public class DeepLinkLauncherController : Controller
    {
        [HttpGet("deeplinks")]
        public IActionResult DeepLink()
        {
            (string Title, HttpRequest Request) model = ("Deep Link Integration", Request);
            return View("~/Views/WebIntegrations/DeepLinkLauncher.cshtml", model);
        }

        [HttpGet("deeplinkswebintegration")]
        public IActionResult DeepLinkWebIntegration()
        {
            return View("~/Views/WebIntegrations/DeepLinkWebIntegrationLauncher.cshtml");
        }
    }
}