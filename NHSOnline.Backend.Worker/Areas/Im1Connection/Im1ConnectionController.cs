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
        private readonly ISystemProviderFactory _systemProviderFactory;
        private readonly ILogger<Im1ConnectionController> _logger;

        public Im1ConnectionController(
            IOdsCodeLookup odsCodeLookup, 
            ISystemProviderFactory systemProviderFactory,
            ILoggerFactory loggerFactory)
        {
            _odsCodeLookup = odsCodeLookup ?? throw new ArgumentNullException(nameof(odsCodeLookup));
            _systemProviderFactory =
                systemProviderFactory ?? throw new ArgumentNullException(nameof(systemProviderFactory));
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

            var systemProviderOption = await GetSystemProvider(odsCode);
            if (!systemProviderOption.HasValue)
            {
                _logger.LogDebug(
                    $"No system provider was found for OdsCode {odsCode} provided in header {Constants.Headers.OdsCode}.");

                // If no system provider is returned it is because the supplier could not be determined from the ODS
                // code.  This, in turn, is because the ODS code is not found so a "Not Found" response is returned.
                return NotFound();
            }

            var systemProvider = systemProviderOption.ValueOrFailure();
            var tokenValidationService = systemProvider.GetTokenValidationService();
            if (!tokenValidationService.IsValidConnectionTokenFormat(connectionToken))
            {
                _logger.LogError(
                    $"ConnectionToken {connectionToken} provided in header {Constants.Headers.ConnectionToken} is not a Guid.");
                return BadRequest();
            }

            var im1ConnectionService = systemProvider.GetIm1ConnectionService();
            var verifyResult = await im1ConnectionService.Verify(connectionToken, odsCode);

            return verifyResult.Accept(new Im1ConnectionVerifyResultVisitor());
        }

        [HttpPost, TimeoutExceptionFilter, AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] PatientIm1ConnectionRequest model)
        {
            var systemProviderOption = await GetSystemProvider(model.OdsCode);
            if (!systemProviderOption.HasValue)
            {
                _logger.LogDebug(
                    $"No system provider was found for OdsCode {model.OdsCode} provided in header {Constants.Headers.OdsCode}.");
                return NotFound();
            }

            var systemProvider = systemProviderOption.ValueOrFailure();
            var im1ConnectionService = systemProvider.GetIm1ConnectionService();
            var registerResult = await im1ConnectionService.Register(model);

            return registerResult.Accept(new Im1ConnectionRegisterResultVisitor(Request));
        }

        private async Task<Option<ISystemProvider>> GetSystemProvider(string odsCode)
        {
            var supplier = await _odsCodeLookup.LookupSupplier(odsCode);
            if (!supplier.HasValue)
            {
                return Option.None<ISystemProvider>();
            }

            return Option.Some(_systemProviderFactory.CreateSystemProvider(supplier.ValueOrFailure()));
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
