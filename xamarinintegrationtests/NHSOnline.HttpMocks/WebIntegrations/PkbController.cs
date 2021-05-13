using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace NHSOnline.HttpMocks.WebIntegrations
{
    public class PkbController : Controller
    {
        private const string AuthHostName = "auth.nhslogin.stubs.local.bitraft.io";
        private const string PkbHostName = "pkb.stubs.local.bitraft.io";

        [Host(PkbHostName)]
        [HttpGet("nhs-login/login")]
        public IActionResult LoginPage(
            [RequiredFromQuery(Name = "phrPath")] string phrPath)
        {
            (Uri Url, HttpRequest Request) model = (new Uri($"http://{AuthHostName}:8080/authorize?phrPath={phrPath}"), Request );
            return View("~/Views/WebIntegrations/RedirectPage.cshtml", model);
        }

        [Host(AuthHostName)]
        [HttpGet("authorize")]
        public IActionResult NhsLoginAuthorize(
            [RequiredFromQuery(Name = "phrPath")] string phrPath)
        {
            (Uri Url, HttpRequest Request) model = (new Uri($"http://{PkbHostName}:8080/authorized?phrPath={phrPath}"), Request );
            return View("~/Views/WebIntegrations/RedirectPage.cshtml", model);
        }

        [Host(PkbHostName)]
        [HttpGet("authorized")]
        public IActionResult InternalPage(
            [RequiredFromQuery(Name = "phrPath")] string phrPath)
        {
            (string Title, string SubTitle, HttpRequest Request) model = ("Pkb Internal Page", phrPath, Request);
            return View("~/Views/WebIntegrations/InternalPage.cshtml", model);
        }
    }
}