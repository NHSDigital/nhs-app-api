using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace NHSOnline.HttpMocks.WebIntegrations
{
    [Host("silverintegrationtestprovider.stubs.local.bitraft.io")]
    public class SilverIntegrationTestProviderController : Controller
    {
        [HttpGet("index.html")]
        public IActionResult InternalPage()
        {
            (string Title, string SubTitle, HttpRequest Request) model = ("Silver Integration Test Provider Internal Page", "", Request);
            return View("~/Views/WebIntegrations/TestProviderInternalPage.cshtml", model);
        }

        [HttpGet("FileUpload.html")]
        public IActionResult FileUpload()
        {
            (string Title, HttpRequest Request) model = ("Silver Integration Test Provider File Upload Page", Request);
            return View("~/Views/WebIntegrations/WebIntegrationFunctionalityPages/FileUploadPage.cshtml", model);
        }

        [HttpGet("Calendar.html")]
        public IActionResult Calendar()
        {
            (string Title, HttpRequest Request) model = ("Silver Integration Test Provider Calendar Page", Request);
            return View("~/Views/WebIntegrations/WebIntegrationFunctionalityPages/CalendarPage.cshtml", model);
        }

        [HttpGet("LocationServices.html")]
        public IActionResult LocationServices()
        {
            (string Title, HttpRequest Request) model = ("Silver Integration Test Provider Location Services Page",
                Request);
            return View("~/Views/WebIntegrations/WebIntegrationFunctionalityPages/LocationServicesPage.cshtml", model);
        }

        [HttpGet("DocumentDownload.html")]
        public IActionResult DocumentDownload()
        {
            (string Title, HttpRequest Request) model = ("Silver Integration Test Provider Document Download Page", Request);
            return View("~/Views/WebIntegrations/WebIntegrationFunctionalityPages/DocumentDownloadPage.cshtml", model);
        }
    }
}
