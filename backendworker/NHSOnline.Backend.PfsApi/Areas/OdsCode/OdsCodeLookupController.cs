using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.Areas.OdsCode.Models;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.OdsCode
{
    [Route("odscodelookup")]
    public class OdsCodeLookupController : Controller
    {
        private readonly ILogger<OdsCodeLookupController> _logger;
        private readonly IOdsCodeLookup _odsCodeLookup;

        public OdsCodeLookupController(ILogger<OdsCodeLookupController> logger, IOdsCodeLookup odsCodeLookup)
        {
            _logger = logger;
            _odsCodeLookup = odsCodeLookup;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery] string odsCode)
        {
            _logger.LogEnter();

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            
            if (string.IsNullOrEmpty(odsCode))
            {
                return BadRequest();
            }

            try
            {
                var isOdsCodeEnrolled = await _odsCodeLookup.LookupSupplier(odsCode);

                var response = new GetOdsCodeLookupResponse { IsGpSystemSupported = isOdsCodeEnrolled.HasValue };
                
                return new OkObjectResult(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed request to get supplier from ods code.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}