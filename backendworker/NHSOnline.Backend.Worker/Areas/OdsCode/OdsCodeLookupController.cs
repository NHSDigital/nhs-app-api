using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Conventions;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.Areas.OdsCode
{
    [Route("odscodelookup"), PfsSecurityMode]
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

            if (string.IsNullOrEmpty(odsCode))
            {
                return BadRequest();
            }

            try
            {
                var isOdsCodeEnrolled = await _odsCodeLookup.LookupSupplier(odsCode);

                return new GetOdsCodeLookupResult.SuccessfullyRetrieved(
                    isOdsCodeEnrolled.HasValue).Accept(new GetOdsCodeLookupResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed request to get supplier from ods code.");
                return new
                    GetOdsCodeLookupResult.ErrorRetrievingOdsCode().Accept(new GetOdsCodeLookupResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}