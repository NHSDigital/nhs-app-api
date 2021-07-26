using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace NHSOnline.HttpMocks.WebIntegrations
{
    [Host("netcompany.stubs.local.bitraft.io")]
    public class NetCompanyController : Controller
    {
        [HttpGet("covid-status-sso")]
        public IActionResult NetCompanyPage()
        {
            (string Title, HttpRequest Request) model = ("NetCompany Internal Page", Request);
            return View("~/Views/WebIntegrations/NetCompanyPage.cshtml", model);
        }
    }
}