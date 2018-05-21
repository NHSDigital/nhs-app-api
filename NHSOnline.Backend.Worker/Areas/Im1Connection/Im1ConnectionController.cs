using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.Areas.Im1Connection
{
    [Route("patient/im1connection")]
    public class Im1ConnectionController : Controller
    {
        private readonly IOdsCodeLookup _odsCodeLookup;
        private readonly IBridgeFactory _bridgeFactory;
        private readonly ILogger<Im1ConnectionController> _logger;

        public Im1ConnectionController(
            IOdsCodeLookup odsCodeLookup, 
            IBridgeFactory bridgeFactory,
            ILoggerFactory loggerFactory)
        {
            _odsCodeLookup = odsCodeLookup ?? throw new ArgumentNullException(nameof(odsCodeLookup));
            _bridgeFactory =
                bridgeFactory ?? throw new ArgumentNullException(nameof(bridgeFactory));
            _logger = loggerFactory.CreateLogger<Im1ConnectionController>();
        }

        [HttpGet, TimeoutExceptionFilter, AllowAnonymous]
        public async Task<IActionResult> Get(
            [FromHeader(Name = Constants.Headers.ConnectionToken)]
            string connectionToken,
            [FromHeader(Name = Constants.Headers.OdsCode)] string odsCode
        )
        {
            if (!ArgumentsAreValid(connectionToken, odsCode))
            {
                return BadRequest();
            }

            var bridgeOption = await GetBridge(odsCode);
            if (!bridgeOption.HasValue)
            {
                _logger.LogDebug(
                    $"No bridge was found for OdsCode {odsCode} provided in header {Constants.Headers.OdsCode}.");

                // If no bridge is returned it is because the supplier could not be determined from the ODS
                // code.  This, in turn, is because the ODS code is not found so a "Not Found" response is returned.
                return NotFound();
            }

            var bridge = bridgeOption.ValueOrFailure();
            var tokenValidationService = bridge.GetTokenValidationService();
            if (!tokenValidationService.IsValidConnectionTokenFormat(connectionToken))
            {
                _logger.LogError(
                    $"ConnectionToken {connectionToken} provided in header {Constants.Headers.ConnectionToken} is not a Guid.");
                return BadRequest();
            }

            var im1ConnectionService = bridge.GetIm1ConnectionService();
            var verifyResult = await im1ConnectionService.Verify(connectionToken, odsCode);

            return verifyResult.Accept(new Im1ConnectionVerifyResultVisitor());
        }

        [HttpPost, TimeoutExceptionFilter, AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] PatientIm1ConnectionRequest model)
        {
            var bridgeOption = await GetBridge(model.OdsCode);
            if (!bridgeOption.HasValue)
            {
                _logger.LogDebug(
                    $"No bridge was found for OdsCode {model.OdsCode} provided in header {Constants.Headers.OdsCode}.");
                return NotFound();
            }

            var bridge = bridgeOption.ValueOrFailure();
            var im1ConnectionService = bridge.GetIm1ConnectionService();
            var registerResult = await im1ConnectionService.Register(model);

            return registerResult.Accept(new Im1ConnectionRegisterResultVisitor(Request));
        }

        private async Task<Option<IBridge>> GetBridge(string odsCode)
        {
            var supplier = await _odsCodeLookup.LookupSupplier(odsCode);
            if (!supplier.HasValue)
            {
                return Option.None<IBridge>();
            }

            return Option.Some(_bridgeFactory.CreateBridge(supplier.ValueOrFailure()));
        }
        
        private bool ArgumentsAreValid(string connectionToken, string odsCode)
        {
            var argumentsAreValid = true;

            if (string.IsNullOrEmpty(connectionToken))
            {
                _logger.LogError($"The header {Constants.Headers.ConnectionToken}, has not been supplied in the request"); 
                argumentsAreValid = false;
            }

            if (string.IsNullOrEmpty(odsCode))
            {
                _logger.LogError($"The header {Constants.Headers.OdsCode}, has not been supplied in the request.");
                argumentsAreValid = false;
            }
            else
            {
                if (!Regex.IsMatch(odsCode, Constants.OdsCodeFormats.GpPracticeEnglandWales))
                {
                    _logger.LogError($"The OdsCode {odsCode} provided in header {Constants.Headers.OdsCode} " +
                                     $"does not match format {Constants.OdsCodeFormats.GpPracticeEnglandWales}.");
                    argumentsAreValid = false;
                }
            }

            return argumentsAreValid;
        }
    }
}
