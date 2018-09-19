using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.Conventions;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.Support;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.Areas.Linkage
{
    [Route("patient/linkage"),CidSecurityMode]
    public class LinkageController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly IOdsCodeLookup _odsCodeLookup;
        private readonly ILogger<LinkageController> _logger;
        private readonly IAuditor _auditor;

        public LinkageController(
            ILoggerFactory loggerFactory,
            IGpSystemFactory gpSystemFactory,
            IOdsCodeLookup odsCodeLookup,
            IAuditor auditor) {
            _logger = loggerFactory.CreateLogger<LinkageController>();
            _odsCodeLookup = odsCodeLookup;
            _gpSystemFactory = gpSystemFactory;
            _auditor = auditor;
        }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> Get(
            [FromHeader(Name = Constants.HttpHeaders.NhsNumber)] string nhsNumber,
            [FromHeader(Name = Constants.HttpHeaders.OdsCode)] string odsCode,
            [FromHeader(Name = Constants.HttpHeaders.IdentityToken)] string identityToken)
        {
            try
            {
                _logger.LogEnter(nameof(Get));

                if (!GetArgumentsAreValid(nhsNumber, odsCode, identityToken))
                {
                    return BadRequest();
                }

                var gpSystemOption = await GetGpSystem(odsCode);
                if (!gpSystemOption.HasValue)
                {
                    _logger.LogError(
                        $"No GP system was found for OdsCode {odsCode} provided in header { Constants.HttpHeaders.OdsCode }.");
                    return new StatusCodeResult(StatusCodes.Status501NotImplemented);
                }

                var gpSystem = gpSystemOption.ValueOrFailure();
                var linkageService = gpSystem.GetLinkageService();

                _auditor.AuditWithExplicitNhsNumber(nhsNumber, gpSystem.Supplier,
                    Constants.AuditingTitles.GetLinkageDetailsAuditTypeRequest, "Attempting to get linkage details.");

                var result = await linkageService.GetLinkageKey(nhsNumber, odsCode, identityToken);
                
                return result.Accept(new LinkageResultAuditingVisitor(
                    _auditor, gpSystem.Supplier, nhsNumber, Constants.AuditingTitles.GetLinkageDetailsAuditTypeResponse));
            }
            finally
            {
                _logger.LogExit(nameof(Get));
            }
            
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] CreateLinkageRequest createLinkageRequest)
        {
            try
            {
                _logger.LogEnter(nameof(Post));

                if (!CreateLinkageRequestBodyIsValid(createLinkageRequest))
                {
                    return BadRequest();
                }

                var gpSystemOption = await GetGpSystem(createLinkageRequest.OdsCode);
                if (!gpSystemOption.HasValue)
                {
                    _logger.LogError(
                        $"No GP system was found for OdsCode {createLinkageRequest.OdsCode} provided in header { Constants.HttpHeaders.OdsCode }.");
                    return new StatusCodeResult(StatusCodes.Status501NotImplemented);
                }

                var gpSystem = gpSystemOption.ValueOrFailure();
                var linkageService = gpSystem.GetLinkageService();

                _auditor.AuditWithExplicitNhsNumber(createLinkageRequest.NhsNumber, gpSystem.Supplier,
                    Constants.AuditingTitles.CreateLinkageKeyAuditTypeRequest, "Attempting to create linkage key.");

                var result = await linkageService.CreateLinkageKey(createLinkageRequest);

                return result.Accept(new LinkageResultAuditingVisitor(
                    _auditor, gpSystem.Supplier, createLinkageRequest.NhsNumber, Constants.AuditingTitles.CreateLinkageKeyAuditTypeResponse));
            }
            finally
            {
                _logger.LogExit(nameof(Post));
            }
        }

        private bool GetArgumentsAreValid(string nhsNumber, string odsCode, string identityToken)
        {
            var argumentsAreValid = true;

            if (string.IsNullOrWhiteSpace(nhsNumber))
            {
                _logger.LogError($"The header { Constants.HttpHeaders.NhsNumber }, has not been supplied in the request.");
                argumentsAreValid = false;
            }

            if (string.IsNullOrWhiteSpace(odsCode))
            {
                _logger.LogError($"The header { Constants.HttpHeaders.OdsCode }, has not been supplied in the request.");
                argumentsAreValid = false;
            }

            if (string.IsNullOrWhiteSpace(identityToken))
            {
                _logger.LogError($"The header { Constants.HttpHeaders.IdentityToken }, has not been supplied in the request.");
                argumentsAreValid = false;
            }

            return argumentsAreValid;
        }

        private bool CreateLinkageRequestBodyIsValid(CreateLinkageRequest createLinkageRequest)
        {
            var argumentsAreValid = true;

            if (string.IsNullOrWhiteSpace(createLinkageRequest.NhsNumber))
            {
                _logger.LogError($"The value for { nameof(createLinkageRequest.NhsNumber) }, has not been supplied in the request.");
                argumentsAreValid = false;
            }

            if (string.IsNullOrWhiteSpace(createLinkageRequest.OdsCode))
            {
                _logger.LogError($"The value for { nameof(createLinkageRequest.OdsCode) }, has not been supplied in the request.");
                argumentsAreValid = false;
            }

            if (string.IsNullOrWhiteSpace(createLinkageRequest.IdentityToken))
            {
                _logger.LogError($"The value for { nameof(createLinkageRequest.IdentityToken) }, has not been supplied in the request.");
                argumentsAreValid = false;
            }

            if (string.IsNullOrWhiteSpace(createLinkageRequest.EmailAddress))
            {
                _logger.LogError($"The value for { nameof(createLinkageRequest.EmailAddress) }, has not been supplied in the request.");
                argumentsAreValid = false;
            }

            return argumentsAreValid;
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
    }
}
