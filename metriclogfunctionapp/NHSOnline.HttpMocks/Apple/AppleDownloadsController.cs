using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.HttpMocks.Apple
{
    [Route("mockDownloadFile/v1/salesReports")]
    public sealed class AppleDownloadsController : ControllerBase
    {
        private readonly AppleSalesReportResponse _appleSalesReportResponse;

        public AppleDownloadsController(AppleSalesReportResponse appleSalesReportResponse)
        {
            _appleSalesReportResponse = appleSalesReportResponse;
        }

        [HttpGet]
        public IActionResult AppleDownloads()
        {
            var response = _appleSalesReportResponse.Content();
            return File(response.Content, response.ContentType);
        }
    }
}