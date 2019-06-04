using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ApiSupport;
using NHSOnline.Backend.CidApi.Areas.Linkage;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Auditing;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.CidApi.Areas.Im1Connection
{
    [ApiVersion("1")]
    [ApiVersionRoute("patient/im1connection")]
    public class Im1ConnectionController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<Im1ConnectionController> _logger;
        private readonly IAuditor _auditor;
        private readonly IOdsCodeMassager _odsCodeMassager;
        private readonly IRetrieveLinkageKeysService _retrieveLinkageKeysService;
        private readonly IIm1ConnectionErrorCodes _im1ErrorCodes;
        private readonly IIm1ConnectionErrorCodes _errorCodes;

        public Im1ConnectionController(
            IGpSystemFactory gpSystemFactory,
            ILogger<Im1ConnectionController> logger,
            IAuditor auditor,
            IOdsCodeMassager odsCodeMassager,
            IRetrieveLinkageKeysService retrieveLinkageKeysService,
            IIm1ConnectionErrorCodes im1ErrorCodes,
            IIm1ConnectionErrorCodes linkageErrorCodes)
        {

            _logger = logger;
            _auditor = auditor;
            _odsCodeMassager = odsCodeMassager;
            _retrieveLinkageKeysService = retrieveLinkageKeysService;
            _im1ErrorCodes = im1ErrorCodes;
            _errorCodes = linkageErrorCodes;
            _gpSystemFactory = gpSystemFactory ?? throw new ArgumentNullException(nameof(gpSystemFactory));
        }
        
        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> Get(
            [FromHeader(Name = Constants.HttpHeaders.ConnectionToken)] string connectionToken,
            [FromHeader(Name = Constants.HttpHeaders.OdsCode)] string odsCode)
        {
            try
            {
                _logger.LogEnter();
                
                if (odsCode != null)
                {
                    odsCode = _odsCodeMassager.CheckOdsCode(odsCode);
                }

                var validator = new Im1ConnectionValidator(_logger);
                return await PerformOperationOnGpSystem(!validator.IsGetValid(connectionToken, odsCode), odsCode, async (gpSystem) =>
                {
                    var tokenValidationService = gpSystem.GetTokenValidationService();
                    if (!tokenValidationService.IsValidConnectionTokenFormat(connectionToken))
                    {
                        _logger.LogError(
                            $"ConnectionToken provided in header {Constants.HttpHeaders.ConnectionToken} is invalid.");
                        return new BadRequestResult();
                    }

                    var im1ConnectionService = gpSystem.GetIm1ConnectionService();
                    var verifyResult = await im1ConnectionService.Verify(connectionToken, odsCode);

                    await verifyResult.Accept(
                        new Im1ConnectionVerifyAuditingVisitor(_auditor, _logger, gpSystem.Supplier));
                    return verifyResult.Accept(new Im1ConnectionVerifyResultVisitor());
                });
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
                
                if (model.OdsCode != null)
                {
                    model.OdsCode = _odsCodeMassager.CheckOdsCode(model.OdsCode);
                }

                var validator = new Im1ConnectionValidator(_logger);

                return await PerformOperationOnGpSystem(!validator.IsPostValid(model), model.OdsCode, async (gpSystem) =>
                {
                    var im1ConnectionService = gpSystem.GetIm1ConnectionService();
                    var registerResult = await im1ConnectionService.Register(model);

                    await registerResult.Accept(
                        new Im1ConnectionRegisterAuditingVisitor(_auditor, _logger, _im1ErrorCodes,
                            gpSystem.Supplier));
                    return registerResult.Accept(new Im1ConnectionRegisterResultVisitor(Request));
                });
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [ApiVersion("2")]
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] Im1RegistrationRequest im1RegistrationRequest)
        {
            try
            {
                var validator = new Im1ConnectionValidator(_logger);
                
                if (im1RegistrationRequest.OdsCode != null)
                {
                    im1RegistrationRequest.OdsCode = _odsCodeMassager.CheckOdsCode(im1RegistrationRequest.OdsCode);
                }

                var isCreateLinkageRequestValid = validator.IsCreateLinkageRequestValid(im1RegistrationRequest);

                var isPatientIm1ConnectionRequestValid =
                    validator.IsPatientIm1ConnectionRequestValid(im1RegistrationRequest);
                var isValid = !isCreateLinkageRequestValid && !isPatientIm1ConnectionRequestValid;

                return await PerformOperationOnGpSystem(isValid, im1RegistrationRequest.OdsCode,
                    async (gpSystem) => await HandleRegistrationRequest(im1RegistrationRequest,
                        gpSystem,
                        isPatientIm1ConnectionRequestValid));
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<IActionResult> HandleRegistrationRequest(
            Im1RegistrationRequest im1RegistrationRequest,
            IGpSystem gpSystem,
            bool isPatientIm1ConnectionRequestValid)
        {
            var registrationRequest = CreatePatientIm1RegisterRequest(im1RegistrationRequest);

            if (!isPatientIm1ConnectionRequestValid)
            {
                _logger.LogInformation("Patient Im1 Connection Request invalid, requires linkage result to register");
                
                var retrieveLinkageRequest = CreateRetrieveLinkageKeysRequest(im1RegistrationRequest);
                var retrieveLinkageResult = await _retrieveLinkageKeysService.RetrieveLinkageKey(retrieveLinkageRequest, gpSystem);

                if (ValidLinkageDetails(retrieveLinkageResult, out var linkageDetails))
                {
                    _logger.LogInformation("Valid linkage details found for Im1 Registration");
                    registrationRequest.AccountId = linkageDetails.AccountId;
                    registrationRequest.OdsCode = linkageDetails.OdsCode;
                    registrationRequest.LinkageKey = linkageDetails.LinkageKey;
                }
                else
                {
                    await retrieveLinkageResult.Accept(
                        new LinkageResultAuditingVisitor<Im1ConnectionController>(_auditor,
                            _logger,
                            _errorCodes,
                            gpSystem.Supplier,
                            im1RegistrationRequest.NhsNumber,
                            Constants.AuditingTitles.CreateLinkageKeyAuditTypeResponse));
                    return await retrieveLinkageResult.Accept(new LinkageV2ResultVisitor(_errorCodes));
                }
            }
            else
            {
                _logger.LogInformation("Patient Im1 Connection Request valid for registration, linkage call not required");
            }

            var im1ConnectionService = gpSystem.GetIm1ConnectionService();
            var registerResult = await im1ConnectionService.Register(registrationRequest);

            await registerResult.Accept(
                new Im1ConnectionRegisterAuditingVisitor(_auditor, _logger, _im1ErrorCodes,
                    gpSystem.Supplier));
            return registerResult.Accept(new Im1ConnectionV2RegisterResultVisitor(Request, _im1ErrorCodes));
        }

        private static bool ValidLinkageDetails(LinkageResult linkageResult, out LinkageResponse linkageResponse)
        {
            linkageResponse = null;
            switch (linkageResult)
            {
                case LinkageResult.SuccessfullyCreated created:
                    linkageResponse = created.Response;
                    return true;
                case LinkageResult.SuccessfullyRetrieved retrieved:
                    linkageResponse = retrieved.Response;
                    return true;
                case LinkageResult.SuccessfullyRetrievedAlreadyExists retrievedAlreadyExists:
                    linkageResponse = retrievedAlreadyExists.Response;
                    return true;
            }
            return false;
        }

        private async Task<IActionResult> PerformOperationOnGpSystem(bool isValidCheck, string odsCode,
            Func<IGpSystem, Task<IActionResult>> operation)
        {
            if (isValidCheck)
            {
                return new BadRequestResult();
            }

            var gpSystemOption = await _gpSystemFactory.LookupGpSystem(odsCode);
            if (!gpSystemOption.HasValue)
            {
                _logger.LogError(
                    $"No GP system was found for OdsCode {odsCode} provided in header {Constants.HttpHeaders.OdsCode}.");
                return new StatusCodeResult(StatusCodes.Status501NotImplemented);
            }

            var gpSystem = gpSystemOption.ValueOrFailure();

            return await operation(gpSystem);
        }

        private static PatientIm1ConnectionRequest CreatePatientIm1RegisterRequest(
            Im1RegistrationRequest im1RegistrationRequest)
        {
            return new PatientIm1ConnectionRequest
            {
                AccountId = im1RegistrationRequest.AccountId,
                DateOfBirth = im1RegistrationRequest.DateOfBirth.Value,
                LinkageKey = im1RegistrationRequest.LinkageKey,
                OdsCode = im1RegistrationRequest.OdsCode,
                Surname = im1RegistrationRequest.Surname
            };
        }

        private static RetrieveLinkageKeysRequest CreateRetrieveLinkageKeysRequest(
            Im1RegistrationRequest im1RegistrationRequest)
        {
            return new RetrieveLinkageKeysRequest
            {
                NhsNumber = im1RegistrationRequest.NhsNumber,
                Surname = im1RegistrationRequest.Surname,
                DateOfBirth = im1RegistrationRequest.DateOfBirth.Value,
                OdsCode = im1RegistrationRequest.OdsCode,
                IdentityToken = im1RegistrationRequest.IdentityToken,
                EmailAddress = im1RegistrationRequest.EmailAddress
            };
        }
    }
}