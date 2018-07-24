using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.Support;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.Areas.Im1Connection
{
    [Route("patient/im1connection")]
    public class Im1ConnectionController : Controller
    {
        private readonly IOdsCodeLookup _odsCodeLookup;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<Im1ConnectionController> _logger;

        public Im1ConnectionController(
            IOdsCodeLookup odsCodeLookup, 
            IGpSystemFactory gpSystemFactory,
            ILoggerFactory loggerFactory)
        {
            _odsCodeLookup = odsCodeLookup ?? throw new ArgumentNullException(nameof(odsCodeLookup));
            _gpSystemFactory =
                gpSystemFactory ?? throw new ArgumentNullException(nameof(gpSystemFactory));
            _logger = loggerFactory.CreateLogger<Im1ConnectionController>();
        }

        [HttpGet, TimeoutExceptionFilter, AllowAnonymous]
        public async Task<IActionResult> Get(
            [FromHeader(Name = Constants.Headers.ConnectionToken)]
            string connectionToken,
            [FromHeader(Name = Constants.Headers.OdsCode)] string odsCode
        )
        {
            try
            {
                _logger.LogEnter(nameof(Get));
    
                if (!ArgumentsAreValid(connectionToken, odsCode))
                {
                    return BadRequest();
                }
    
                var gpSystemOption = await GetGpSystem(odsCode);
                if (!gpSystemOption.HasValue)
                {
                    _logger.LogError(
                        $"No GP system was found for OdsCode {odsCode} provided in header {Constants.Headers.OdsCode}.");
    
                    return new StatusCodeResult(StatusCodes.Status501NotImplemented);
                }
    
                var gpSystem = gpSystemOption.ValueOrFailure();
                var tokenValidationService = gpSystem.GetTokenValidationService();
                if (!tokenValidationService.IsValidConnectionTokenFormat(connectionToken))
                {
                    _logger.LogError(
                        $"ConnectionToken provided in header {Constants.Headers.ConnectionToken} is invalid.");
                    return BadRequest();
                }
    
                var im1ConnectionService = gpSystem.GetIm1ConnectionService();
                var verifyResult = await im1ConnectionService.Verify(connectionToken, odsCode);
    
                _logger.LogDebug("Get Im1Connection completed");
                return verifyResult.Accept(new Im1ConnectionVerifyResultVisitor());
            }
            finally
            {
                _logger.LogExit(nameof(Get));
            }
        }

        [HttpPost, TimeoutExceptionFilter, AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] PatientIm1ConnectionRequest model)
        {
            try
            {
                _logger.LogEnter(nameof(Post));
                var gpSystemOption = await GetGpSystem(model.OdsCode);
                if (!gpSystemOption.HasValue)
                {
                    _logger.LogError(
                        $"No GP system was found for OdsCode {model.OdsCode} provided in header {Constants.Headers.OdsCode}.");
                    return new StatusCodeResult(StatusCodes.Status501NotImplemented);
                }

                var gpSystem = gpSystemOption.ValueOrFailure();
                var im1ConnectionService = gpSystem.GetIm1ConnectionService();
                var registerResult = await im1ConnectionService.Register(model);

                return registerResult.Accept(new Im1ConnectionRegisterResultVisitor(Request));
            }
            finally
            {
                _logger.LogExit(nameof(Post));
            }
        }

        private async Task<Option<IGpSystem>> GetGpSystem(string odsCode)
        {
            var supplier = await _odsCodeLookup.LookupSupplier(odsCode);
            if (!supplier.HasValue)
            {
                return Option.None<IGpSystem>();
            }

            return Option.Some(_gpSystemFactory.CreateGpSystem(supplier.ValueOrFailure()));
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

            return argumentsAreValid;
        }
    }
}
