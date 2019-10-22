using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.CidApi.Areas.Linkage
{
    [ApiVersionRoute("patient/linkage")]
    public class LinkageController : Controller
    {
        private readonly ILogger<LinkageController> _logger;
        private readonly IAuditor _auditor;
        private readonly IMinimumAgeValidator _minimumAgeValidator;
        private readonly ConfigurationSettings _settings;
        private readonly IOdsCodeMassager _odsCodeMassager;
        private readonly IGpSystemResolver _gpSystemResolver;

        public LinkageController(
            ILogger<LinkageController> logger,
            IAuditor auditor,
            IMinimumAgeValidator minimumAgeValidator,
            ConfigurationSettings settings,
            IOdsCodeMassager odsCodeMassager,
            IGpSystemResolver gpSystemResolver)
        {
            _logger = logger;
            _auditor = auditor;
            _minimumAgeValidator = minimumAgeValidator;
            _settings = settings;
            _odsCodeMassager = odsCodeMassager;
            _gpSystemResolver = gpSystemResolver ?? throw new ArgumentNullException(nameof(gpSystemResolver));
            
            _settings.Validate();
        }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> Get(
            [FromHeader(Name = Constants.HttpHeaders.NhsNumber)]
            string nhsNumber,
            [FromHeader(Name = Constants.HttpHeaders.Surname)]
            string surname,
            [FromHeader(Name = Constants.HttpHeaders.DateOfBirth)]
            DateTime dateOfBirth,
            [FromHeader(Name = Constants.HttpHeaders.OdsCode)]
            string odsCode,
            [FromHeader(Name = Constants.HttpHeaders.IdentityToken)]
            string identityToken)
        {
            try
            {
                _logger.LogEnter();
                var cidOdsCode = odsCode;

                return await PerformOperationOnGpSystem(string.IsNullOrWhiteSpace(odsCode), odsCode, async gpSystem =>
                {
                    var getLinkageRequest = new GetLinkageRequest()
                    {
                        NhsNumber = nhsNumber,
                        Surname = surname,
                        DateOfBirth = dateOfBirth,
                        OdsCode = odsCode,
                        IdentityToken = identityToken
                    };

                    var validationService = gpSystem.GetLinkageValidationService();
                    if (!validationService.IsGetValid(getLinkageRequest))
                    {
                        _logger.LogError($"Invalid parameters or parameters missing from get linkage request");
                        return BadRequest();
                    }

                    var linkageService = gpSystem.GetLinkageService();

                    await _auditor.AuditRegistrationEvent(nhsNumber, gpSystem.Supplier,
                        AuditingOperations.GetLinkageDetailsAuditTypeRequest,
                        "Attempting to get linkage details.");

                    var result = await linkageService.GetLinkageKey(getLinkageRequest);

                    if (_odsCodeMassager.IsEnabled && result is LinkageResult.SuccessfullyRetrieved successResult)
                    {
                        successResult.Response.OdsCode = cidOdsCode;
                    }

                    await result.Accept(new LinkageResultAuditingVisitor<LinkageController>(
                        _auditor, _logger, gpSystem.Supplier, nhsNumber,
                        AuditingOperations.GetLinkageDetailsAuditTypeResponse));

                    return await result.Accept(new LinkageResultVisitor(_logger));
                });
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
                var cidOdsCode = createLinkageRequest.OdsCode;

                return await PerformOperationOnGpSystem(
                    string.IsNullOrWhiteSpace(createLinkageRequest.OdsCode), createLinkageRequest.OdsCode,
                    async (gpSystem) =>
                    {
                        var validationService = gpSystem.GetLinkageValidationService();
                        if (!validationService.IsPostValid(createLinkageRequest))
                        {
                            _logger.LogError($"Invalid parameters or parameters missing from create linkage request");
                            return BadRequest();
                        }

                        if (!HasValidDateOfBirthForLinkage(createLinkageRequest.DateOfBirth))
                        {
                            return await HandleUnderMinimumAge(createLinkageRequest, gpSystem);
                        }

                        var linkageService = gpSystem.GetLinkageService();

                        await _auditor.AuditRegistrationEvent(createLinkageRequest.NhsNumber, gpSystem.Supplier,
                            AuditingOperations.CreateLinkageKeyAuditTypeRequest,
                            "Attempting to create linkage key.");

                        var result = await linkageService.CreateLinkageKey(createLinkageRequest);

                        if (_odsCodeMassager.IsEnabled && result is LinkageResult.SuccessfullyCreated createdResult)
                        {
                            createdResult.Response.OdsCode = cidOdsCode;
                        }

                        await result.Accept(new LinkageResultAuditingVisitor<LinkageController>(
                            _auditor, _logger, gpSystem.Supplier, createLinkageRequest.NhsNumber,
                            AuditingOperations.CreateLinkageKeyAuditTypeResponse));

                        return await result.Accept(new LinkageResultVisitor(_logger));
                    });
            }
            finally
            {
                _logger.LogExit();
            }
        }
        
        private async Task<IActionResult> PerformOperationOnGpSystem(bool isValidCheck, string odsCode,
            Func<IGpSystem, Task<IActionResult>> operation)
        {
            if (isValidCheck)
            {
                return new BadRequestResult();
            }

            odsCode = _odsCodeMassager.CheckOdsCode(odsCode);

            var gpSystemOption = await _gpSystemResolver.ResolveFromOdsCode(odsCode);
            if (!gpSystemOption.HasValue)
            {
                _logger.LogError(
                    $"No GP system was found for OdsCode {odsCode} provided in header {Constants.HttpHeaders.OdsCode}.");
                return new StatusCodeResult(StatusCodes.Status501NotImplemented);
            }

            var gpSystem = gpSystemOption.ValueOrFailure();

            return await operation(gpSystem);
        }

        private async Task<IActionResult> HandleUnderMinimumAge(CreateLinkageRequest createLinkageRequest,
            IGpSystem gpSystem)
        {
            _logger.LogWarning("Linkage details request unsuccessful - patient non competent or under 16.");

            var linkageResult = new LinkageResult.ErrorCase(Im1ConnectionErrorCodes.InternalCode.UnderMinimumAgeOrNonCompetent);

            await linkageResult.Accept(new LinkageResultAuditingVisitor<LinkageController>(
                _auditor, _logger, gpSystem.Supplier, createLinkageRequest.NhsNumber,
                AuditingOperations.CreateLinkageKeyAuditTypeResponse));

            return await linkageResult.Accept(new LinkageResultVisitor(_logger));
        }

        private bool HasValidDateOfBirthForLinkage(DateTime? dateOfBirth)
        {
            if (!dateOfBirth.HasValue)
            {
                _logger.LogError("Missing date of birth");
                return false;
            }

            if (!_minimumAgeValidator.IsValid(dateOfBirth.Value, _settings.MinimumLinkageAge))
            {
                _logger.LogWarning("Failed to meet the minimum linkage age requirement.");
                return false;
            }

            return true;
        }
    }
}