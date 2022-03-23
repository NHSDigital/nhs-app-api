using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.HttpMocks.Download
{
    [Route("mockDownloadFile")]
    public sealed class DownloadFileController : ControllerBase
    {
        private readonly DownloadResponse _response;

        public DownloadFileController(DownloadResponse response)
        {
            _response = response;
        }

        [HttpGet]
        [Route("{filename}")]
        public IActionResult DownloadFile(string filename)
        {
            var file = _response.GetResponse(filename);
            if (file == null)
            {
                return new NotFoundObjectResult("File to be downloaded not set up in test");
            }
            return File(file.Content, file.ContentType);
        }
    }
}