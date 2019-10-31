using System.Linq;
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
                
                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }

                await _auditor.Audit(AuditingOperations.OrganDonationRegistrationAuditTypeRequest, "Attempting to register organ donation decision");

                var result = await Register(model);

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
                
                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }

                await _auditor.Audit(AuditingOperations.OrganDonationUpdateAuditTypeRequest, "Attempting to update organ donation decision");
                
                var result = await Update(model);

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

                await _auditor.Audit(AuditingOperations.OrganDonationWithdrawAuditTypeRequest, "Attempting to withdraw organ donation decision");
                
                var result = await Withdraw(model);

                result.Accept(new OrganDonationWithdrawAuditingVisitor(_auditor));

                return result.Accept(new OrganDonationWithdrawResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }
        
        private async Task<OrganDonationRegistrationResult> Update(OrganDonationRegistrationRequest model)
        {
            var userSession = HttpContext.GetUserSession();

            if (!_validator.IsPutValid(model))
            {
                _logger.LogError("Invalid request body supplied to registration update request");
                return new OrganDonationRegistrationResult.BadRequest();
            }

            return await _organDonationService.Update(model, userSession);
        }
        
        private async Task<OrganDonationRegistrationResult> Register(OrganDonationRegistrationRequest model)
        {
            var userSession = HttpContext.GetUserSession();

            if (!_validator.IsPostValid(model))
            {
                _logger.LogError("Invalid request body supplied to registration request");
                return new OrganDonationRegistrationResult.BadRequest();
            }

            return await _organDonationService.Register(model, userSession);
        }

        private async Task<OrganDonationWithdrawResult> Withdraw(OrganDonationWithdrawRequest model)
        {
            var userSession = HttpContext.GetUserSession();

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