using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.Support;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.Areas.Im1Connection
{
    [Route("patient/im1connection"),CidSecurityMode]
    public class Im1ConnectionController : Controller
    {
        private readonly IOdsCodeLookup _odsCodeLookup;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<Im1ConnectionController> _logger;
        private readonly IAuditor _auditor;

        public Im1ConnectionController(
            IOdsCodeLookup odsCodeLookup, 
            IGpSystemFactory gpSystemFactory,
            ILogger<Im1ConnectionController> logger,
            IAuditor auditor)
        {
            _odsCodeLookup = odsCodeLookup ?? throw new ArgumentNullException(nameof(odsCodeLookup));
            _gpSystemFactory =
                gpSystemFactory ?? throw new ArgumentNullException(nameof(gpSystemFactory));
            _logger = logger;
            _auditor = auditor;
        }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> Get(
            [FromHeader(Name = Constants.HttpHeaders.ConnectionToken)] string connectionToken,
            [FromHeader(Name = Constants.HttpHeaders.OdsCode)] string odsCode
        )
        {
            try
            {
                _logger.LogEnter();
                if (odsCode != null)
                {
                    odsCode = OdsCodeMassager.CheckOdsCode(odsCode, _logger);
                }


                var argumentValidator = new ValidateAndLog(_logger)
                    .IsNotNullOrWhitespace(connectionToken, Constants.HttpHeaders.ConnectionToken)
                    .IsNotNullOrWhitespace(odsCode, Constants.HttpHeaders.OdsCode);

                if (!argumentValidator.IsValid())
                {
                    return BadRequest();
                }
    
                var gpSystemOption = await GetGpSystem(odsCode);
                if (!gpSystemOption.HasValue)
                {
                    _logger.LogError(
                        $"No GP system was found for OdsCode {odsCode} provided in header {Constants.HttpHeaders.OdsCode}.");
    
                    return new StatusCodeResult(StatusCodes.Status501NotImplemented);
                }
    
                var gpSystem = gpSystemOption.ValueOrFailure();
                var tokenValidationService = gpSystem.GetTokenValidationService();
                if (!tokenValidationService.IsValidConnectionTokenFormat(connectionToken))
                {
                    _logger.LogError(
                        $"ConnectionToken provided in header {Constants.HttpHeaders.ConnectionToken} is invalid.");
                    return BadRequest();
                }

                var im1ConnectionService = gpSystem.GetIm1ConnectionService();
                var verifyResult = await im1ConnectionService.Verify(connectionToken, odsCode);
                
                _logger.LogDebug($"{nameof(gpSystem.GetIm1ConnectionService)} completed");

                await verifyResult.Accept(new Im1ConnectionVerifyAuditingVisitor(_auditor, _logger, gpSystem.Supplier));
                return verifyResult.Accept(new Im1ConnectionVerifyResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] PatientIm1ConnectionRequest model)
        {
            try
            {
                _logger.LogEnter();
                model.OdsCode = OdsCodeMassager.CheckOdsCode(model.OdsCode, _logger);

                var gpSystemOption = await GetGpSystem(model.OdsCode);
                if (!gpSystemOption.HasValue)
                {
                    _logger.LogError(
                        $"No GP system was found for OdsCode {model.OdsCode} provided in header {Constants.HttpHeaders.OdsCode}.");
                    return new StatusCodeResult(StatusCodes.Status501NotImplemented);
                }

                var gpSystem = gpSystemOption.ValueOrFailure();
                var im1ConnectionService = gpSystem.GetIm1ConnectionService();
                var registerResult = await im1ConnectionService.Register(model);

                await registerResult.Accept(new Im1ConnectionRegisterAuditingVisitor(_auditor, _logger, gpSystem.Supplier));
                return registerResult.Accept(new Im1ConnectionRegisterResultVisitor(Request));
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<Option<IGpSystem>> GetGpSystem(string odsCode)
        {
            var supplier = await _odsCodeLookup.LookupSupplier(odsCode);

            try
            {
                _logger.LogDebug($"Fetch GP System: '{supplier}'.");
                return supplier.HasValue 
                    ? Option.Some(_gpSystemFactory.CreateGpSystem(supplier.ValueOrFailure())) 
                    : Option.None<IGpSystem>();
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Failed to create GP System for supplier: {supplier.ValueOrFailure()}.");
                return Option.None<IGpSystem>();
            }
        }
    }
}
