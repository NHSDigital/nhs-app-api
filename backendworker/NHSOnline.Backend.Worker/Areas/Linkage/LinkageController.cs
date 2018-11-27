using System;
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
            [FromHeader(Name = Constants.HttpHeaders.Surname)] string surname,
            [FromHeader(Name = Constants.HttpHeaders.DateOfBirth)] DateTime dateOfBirth,
            [FromHeader(Name = Constants.HttpHeaders.OdsCode)] string odsCode,
            [FromHeader(Name = Constants.HttpHeaders.IdentityToken)] string identityToken)
        {
            try
            {
                _logger.LogEnter();

                if (string.IsNullOrWhiteSpace(odsCode))
                {
                    return BadRequest();
                }

                var getLinkageRequest = new GetLinkageRequest()
                {
                    NhsNumber = nhsNumber,
                    Surname = surname,
                    DateOfBirth = dateOfBirth,
                    OdsCode = odsCode,
                    IdentityToken = identityToken
                };

                var gpSystemOption = await GetGpSystem(odsCode);
                if (!gpSystemOption.HasValue)
                {
                    _logger.LogError(
                        $"No GP system was found for OdsCode {odsCode} provided in header { Constants.HttpHeaders.OdsCode }.");
                    return new StatusCodeResult(StatusCodes.Status501NotImplemented);
                }

                var gpSystem = gpSystemOption.ValueOrFailure();

                var validationService = gpSystem.GetLinkageRequestValidationService();
                if (!validationService.Validate(getLinkageRequest))
                {
                    _logger.LogError($"Invalid parameters or parameters missing from get linkage request");
                    return BadRequest();
                }
                
                var linkageService = gpSystem.GetLinkageService();

                await _auditor.AuditWithExplicitNhsNumber(nhsNumber, gpSystem.Supplier,
                    Constants.AuditingTitles.GetLinkageDetailsAuditTypeRequest, "Attempting to get linkage details.");

                var result = await linkageService.GetLinkageKey(getLinkageRequest);
                
                return result.Accept(new LinkageResultAuditingVisitor(
                    _auditor, gpSystem.Supplier, nhsNumber, Constants.AuditingTitles.GetLinkageDetailsAuditTypeResponse));
            }
            finally
            {
                _logger.LogExit();
            }
            
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] CreateLinkageRequest createLinkageRequest)
        {
            try
            {
                _logger.LogEnter();
                
                if (string.IsNullOrWhiteSpace(createLinkageRequest.OdsCode))
                {
                    return BadRequest();
                }

                var gpSystemOption = await GetGpSystem(createLinkageRequest.OdsCode);
                if (!gpSystemOption.HasValue)
                {
                    _logger.LogError(
                        $"No GP system was found for OdsCode {createLinkageRequest.OdsCode} provided in header {Constants.HttpHeaders.OdsCode}.");
                    return new StatusCodeResult(StatusCodes.Status501NotImplemented);
                }

                var gpSystem = gpSystemOption.ValueOrFailure();
                
                var validationService = gpSystem.GetLinkageRequestValidationService();
                if (!validationService.Validate(createLinkageRequest))
                {
                    _logger.LogError($"Invalid parameters or parameters missing from get linkage request");
                    return BadRequest();
                }
                
                var linkageService = gpSystem.GetLinkageService();

                await _auditor.AuditWithExplicitNhsNumber(createLinkageRequest.NhsNumber, gpSystem.Supplier,
                    Constants.AuditingTitles.CreateLinkageKeyAuditTypeRequest, "Attempting to create linkage key.");

                var result = await linkageService.CreateLinkageKey(createLinkageRequest); 

                return result.Accept(new LinkageResultAuditingVisitor(
                    _auditor, gpSystem.Supplier, createLinkageRequest.NhsNumber,
                    Constants.AuditingTitles.CreateLinkageKeyAuditTypeResponse));
            }
            finally
            {
                _logger.LogExit();
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
    }
}
