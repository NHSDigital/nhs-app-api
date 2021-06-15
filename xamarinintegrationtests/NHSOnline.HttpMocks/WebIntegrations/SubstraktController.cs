using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace NHSOnline.HttpMocks.WebIntegrations
{
    public class SubstraktController : Controller
    {
        private const string AuthHostName = "auth.nhslogin.stubs.local.bitraft.io";
        private const string SubstraktHostName = "substrakt.stubs.local.bitraft.io";
        private const string SubstraktFirstDomainHostName = "substraktDomain1.stubs.local.bitraft.io";

        [Host(SubstraktHostName)]
        [HttpGet("jump/update-details")]
        public IActionResult LoginPage()
        {
            (Uri Url, HttpRequest Request) model = (new Uri($"http://{AuthHostName}:8080/authorizeSubstrakt"), Request );
            return View("~/Views/WebIntegrations/RedirectPage.cshtml", model);
        }

        [Host(AuthHostName)]
        [HttpGet("authorizeSubstrakt")]
        public IActionResult NhsLoginAuthorize()
        {
            (Uri Url, HttpRequest Request) model = (new Uri($"http://{SubstraktFirstDomainHostName}:8080/firstDomain"), Request );
            return View("~/Views/WebIntegrations/RedirectPage.cshtml", model);
        }

        [Host(SubstraktFirstDomainHostName)]
        [HttpGet("firstDomain")]
        public IActionResult InternalPage()
        {
            (string Title, HttpRequest Request) model = ("Substrakt First Domain Internal Page", Request);
            return View("~/Views/WebIntegrations/SubstraktFirstDomainPage.cshtml", model);
        }
    }
}