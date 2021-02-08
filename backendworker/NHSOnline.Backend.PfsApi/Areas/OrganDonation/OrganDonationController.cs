using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.PfsApi.Filters;
using NHSOnline.Backend.PfsApi.OrganDonation;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.OrganDonation
{
    [ApiVersionRoute("patient/organdonation")]
    [ProxyingNotAllowed]
    public class OrganDonationController : Controller
    {
        private readonly ILogger<OrganDonationController> _logger;
        private readonly IAuditor _auditor;
        private readonly IOrganDonationValidationService _validator;
        private readonly IMetricLogger _metricLogger;
        private readonly IOrganDonationService _organDonationService;
        private readonly IGpSystemFactory _gpSystemFactory;

        public OrganDonationController(
            ILogger<OrganDonationController> logger,
            IGpSystemFactory gpSystemFactory,
            IOrganDonationService organDonationService,
            IAuditor auditor,
            IOrganDonationValidationService validator,
            IMetricLogger metricLogger
        )
        {
            _logger = logger;
            _auditor = auditor;
            _validator = validator;
            _metricLogger = metricLogger;
            _organDonationService = organDonationService;
            _gpSystemFactory = gpSystemFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Get([UserSession] P9UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                await _auditor.PreOperationAudit(AuditingOperations.GetOrganDonationAuditTypeRequest,
                    "Attempting to get organ donation record");

                _logger.LogInformation($"Fetching DemographicsService for supplier: {userSession.GpUserSession.Supplier.ToString()}");

                var demographicsService = _gpSystemFactory.CreateGpSystem(userSession.GpUserSession.Supplier)
                    .GetDemographicsService();

                _logger.LogDebug("Fetching Demographics");
                var demographicsResult = await demographicsService.GetDemographics(
                    new GpLinkedAccountModel(userSession.GpUserSession));

                var result = await _organDonationService.GetOrganDonation(demographicsResult, userSession);

                await result.Accept(new OrganDonationAuditingVisitor(_auditor, _logger, _metricLogger, userSession));
                return result.Accept(new OrganDonationResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(
            [FromBody]OrganDonationRegistrationRequest model,
            [UserSession] P9UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }

                await _auditor.PreOperationAudit(AuditingOperations.OrganDonationRegistrationAuditTypeRequest, "Attempting to register organ donation decision");

                var result = await Register(model, userSession);

                await result.Accept(new OrganDonationRegistrationAuditingVisitor(_auditor, _logger, _metricLogger, userSession));
                return result.Accept(new OrganDonationRegistrationVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put(
            [FromBody] OrganDonationRegistrationRequest model,
            [UserSession] P9UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }

                await _auditor.PreOperationAudit(AuditingOperations.OrganDonationUpdateAuditTypeRequest, "Attempting to update organ donation decision");

                var result = await Update(model, userSession);

                await result.Accept(new OrganDonationRegistrationUpdateAuditingVisitor(_auditor, _metricLogger, userSession));
                return result.Accept(new OrganDonationRegistrationUpdateVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(
            [FromBody] OrganDonationWithdrawRequest model,
            [UserSession] P9UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                await _auditor.PreOperationAudit(AuditingOperations.OrganDonationWithdrawAuditTypeRequest, "Attempting to withdraw organ donation decision");

                var result = await Withdraw(model, userSession);

                await result.Accept(new OrganDonationWithdrawAuditingVisitor(_auditor, _metricLogger, userSession));

                return result.Accept(new OrganDonationWithdrawResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<OrganDonationRegistrationResult> Update(
            OrganDonationRegistrationRequest model,
            P9UserSession userSession)
        {
            if (!_validator.IsPutValid(model))
            {
                _logger.LogError("Invalid request body supplied to registration update request");
                return new OrganDonationRegistrationResult.BadRequest();
            }

            return await _organDonationService.Update(model, userSession);
        }

        private async Task<OrganDonationRegistrationResult> Register(
            OrganDonationRegistrationRequest model,
            P9UserSession userSession)
        {
            if (!_validator.IsPostValid(model))
            {
                _logger.LogError("Invalid request body supplied to registration request");
                return new OrganDonationRegistrationResult.BadRequest();
            }

            return await _organDonationService.Register(model, userSession);
        }

        private async Task<OrganDonationWithdrawResult> Withdraw(
            OrganDonationWithdrawRequest model,
            P9UserSession userSession)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogModelStateValidationFailure(ModelState);
                return new OrganDonationWithdrawResult.BadRequest();
            }

            if (!_validator.IsDeleteValid(model))
            {
                _logger.LogError("Invalid request body supplied to withdraw request");
                return new OrganDonationWithdrawResult.BadRequest();
            }

            return await _organDonationService.Withdraw(model, userSession);
        }
    }
}
