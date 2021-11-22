using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace NHSOnline.HttpMocks.WebIntegrations
{
    public class PkbController : Controller
    {
        private const string AuthHostName = "auth.nhslogin.stubs.local.bitraft.io";
        private const string PkbHostName = "pkb.stubs.local.bitraft.io";
        private const string SecurePkbHostName = "pkb.securestubs.local.bitraft.io";

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
            (Uri Url, HttpRequest Request) model = (new Uri($"https://{SecurePkbHostName}/authorized?phrPath={phrPath}"), Request );
            return View("~/Views/WebIntegrations/RedirectPage.cshtml", model);
        }

        [Host(PkbHostName)]
        [HttpGet("authorized")]
        public IActionResult InternalPage(
            [RequiredFromQuery(Name = "phrPath")] string phrPath)
        {
            (string Title, string SubTitle, string SecureHostName, HttpRequest Request) model = ("Pkb Internal Page", phrPath, SecurePkbHostName, Request);
            return View("~/Views/WebIntegrations/PkbInternalPage.cshtml", model);
        }

        /*
         *
         * NOTE:
         * Moved web integration functionality into PKB as the SSO webview (launched during
         * the loading of the Messages hub) results in multiple unhandled windows/contexts
         * being returned from Appium causing unpredictable behaviour in the tests
         *
         */

        [Host(PkbHostName)]
        [HttpGet("Calendar.html")]
        public IActionResult CalendarPage()
        {
            (string Title, HttpRequest Request) model = ("Web Integration Functionality - Calendar", Request);
            return View("~/Views/WebIntegrations/WebIntegrationFunctionalityPages/CalendarPage.cshtml", model);
        }

        [Host(PkbHostName)]
        [HttpGet("GoToPage.html")]
        public IActionResult GoToPagePage()
        {
            (string Title, HttpRequest Request) model = ("Web Integration Functionality - Go To Page", Request);
            return View("~/Views/WebIntegrations/WebIntegrationFunctionalityPages/GoToPagePage.cshtml", model);
        }

        [Host(PkbHostName)]
        [HttpGet("OpenBrowserOverlay.html")]
        public IActionResult OpenBrowserOverlayPage()
        {
            (string Title, HttpRequest Request) model = ("Web Integration Functionality - Open Browser Overlay", Request);
            return View("~/Views/WebIntegrations/WebIntegrationFunctionalityPages/OpenBrowserOverlayPage.cshtml", model);
        }

        [Host(PkbHostName)]
        [HttpGet("FileUpload.html")]
        public IActionResult FileUploadPage()
        {
            (string Title, HttpRequest Request) model = ("Web Integration Functionality - File Upload", Request);
            return View("~/Views/WebIntegrations/WebIntegrationFunctionalityPages/FileUploadPage.cshtml", model);
        }

        [Host(PkbHostName)]
        [HttpGet("DocumentDownload.html")]
        public IActionResult DocumentDownload()
        {
            var basePath =
                $"{Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent}/NHSOnline.HttpMocks/Resources";
            var passKitBase64 = System.IO.File.ReadAllText($"{basePath}/PKPass.txt");
            var imageBase64 = System.IO.File.ReadAllText($"{basePath}/HandAndFootXrayImage.txt");
            var corruptedFileBase64 = System.IO.File.ReadAllText($"{basePath}/CorruptedFile.txt");

            (string Title, HttpRequest Request, string ImageBase64String, string PkPassBase64String, string CorruptedBase64String) model =
                ("Web Integration Functionality - Document Download", Request, imageBase64, passKitBase64, corruptedFileBase64);
            return View("~/Views/WebIntegrations/WebIntegrationFunctionalityPages/DocumentDownloadPage.cshtml", model);
        }

        [Host(PkbHostName)]
        [HttpGet("LocationServices.html")]
        public IActionResult LocationServices()
        {
            (string Title, HttpRequest Request) model = ("Web Integration Functionality - Location Services", Request);
            return View("~/Views/WebIntegrations/WebIntegrationFunctionalityPages/LocationServicesPage.cshtml", model);
        }
    }
}