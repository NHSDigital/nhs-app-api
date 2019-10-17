using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.PfsApi.OrganDonation;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.OrganDonation
{
    [Route("patient/organdonation")]
    public class OrganDonationController : Controller
    {
        private readonly ILogger<OrganDonationController> _logger;
        private readonly IAuditor _auditor;
        private readonly IOrganDonationValidationService _validator;
        private readonly IOrganDonationService _organDonationService;
        private readonly IGpSystemFactory _gpSystemFactory;

        public OrganDonationController(
            ILogger<OrganDonationController> logger,
            IGpSystemFactory gpSystemFactory,
            IOrganDonationService organDonationService,
            IAuditor auditor, 
            IOrganDonationValidationService validator
        )
        {
            _logger = logger;
            _auditor = auditor;
            _validator = validator;
            _organDonationService = organDonationService;
            _gpSystemFactory = gpSystemFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogEnter();

                await _auditor.Audit(AuditingOperations.GetOrganDonationAuditTypeRequest,
                    "Attempting to get organ donation record");

                var userSession = HttpContext.GetUserSession();

                _logger.LogInformation($"Fetching DemographicsService for supplier: {userSession.GpUserSession.Supplier.ToString()}");

                var demographicsService = _gpSystemFactory.CreateGpSystem(userSession.GpUserSession.Supplier)
                    .GetDemographicsService();

                _logger.LogDebug("Fetching Demographics");
                var demographicsResult = await demographicsService.GetDemographics(
                    new GpLinkedAccountModel(userSession.GpUserSession));

                var result = await _organDonationService.GetOrganDonation(demographicsResult, userSession);

                await result.Accept(new OrganDonationAuditingVisitor(_auditor, _logger));
                return result.Accept(new OrganDonationResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]OrganDonationRegistrationRequest model)
        {
            try
            {
                _logger.LogEnter();

                if (!_validator.IsPostValid(model))
                {
                    _logger.LogError("Invalid request body supplied to registration request");
                    return BadRequest();
                }

                await _auditor.Audit(AuditingOperations.OrganDonationRegistrationAuditTypeRequest, "Attempting to register organ donation decision");
                
                var userSession = HttpContext.GetUserSession();

                var result = await _organDonationService.Register(model, userSession);

                await result.Accept(new OrganDonationRegistrationAuditingVisitor(_auditor, _logger));
                return result.Accept(new OrganDonationRegistrationVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] OrganDonationRegistrationRequest model)
        {
            try
            {
                _logger.LogEnter();

                if (!_validator.IsPutValid(model))
                {
                    _logger.LogError("Invalid request body supplied to registration update request");
                    return BadRequest();
                }

                await _auditor.Audit(AuditingOperations.OrganDonationUpdateAuditTypeRequest, "Attempting to update organ donation decision");

                var userSession = HttpContext.GetUserSession();

                var result = await _organDonationService.Update(model, userSession);

                result.Accept(new OrganDonationRegistrationUpdateAuditingVisitor(_auditor));

                return result.Accept(new OrganDonationRegistrationUpdateVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] OrganDonationWithdrawRequest model)
        {
            try
            {
                _logger.LogEnter();
                
                if (!_validator.IsDeleteValid(model))
                {
                    _logger.LogError("Invalid request body supplied to withdraw request");
                    return BadRequest();
                }

                await _auditor.Audit(AuditingOperations.OrganDonationWithdrawAuditTypeRequest, "Attempting to withdraw organ donation decision");

                var userSession = HttpContext.GetUserSession();

                var result = await _organDonationService.Withdraw(model, userSession);

                result.Accept(new OrganDonationWithdrawAuditingVisitor(_auditor));

                return result.Accept(new OrganDonationWithdrawResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}