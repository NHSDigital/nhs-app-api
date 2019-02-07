using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Worker.Support.Logging;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.OrganDonation;

namespace NHSOnline.Backend.Worker.Areas.OrganDonation
{
    [Route("patient/organdonation"), PfsSecurityMode]
    public class OrganDonationController : Controller
    {
        private readonly ILogger<OrganDonationController> _logger;
        private readonly IAuditor _auditor;
        private readonly IOrganDonationService _organDonationService;
        private readonly IGpSystemFactory _gpSystemFactory;

        public OrganDonationController(
            ILogger<OrganDonationController> logger,
            IGpSystemFactory gpSystemFactory,
            IOrganDonationService organDonationService,
            IAuditor auditor
        )
        {
            _logger = logger;
            _auditor = auditor;
            _organDonationService = organDonationService;
            _gpSystemFactory = gpSystemFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogEnter();

                await _auditor.Audit(Constants.AuditingTitles.GetOrganDonationAuditTypeRequest,
                    "Attempting to get organ donation record");

                var userSession = HttpContext.GetUserSession();

                _logger.LogInformation($"Fetching DemographicsService for supplier: {userSession.GpUserSession.Supplier.ToString()}");

                var demographicsService = _gpSystemFactory.CreateGpSystem(userSession.GpUserSession.Supplier)
                    .GetDemographicsService();

                _logger.LogDebug("Fetching Demographics");
                var myRecordGetResult = await demographicsService.GetDemographics(userSession.GpUserSession);

                var result = await _organDonationService.GetOrganDonation(myRecordGetResult, userSession);

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

                await _auditor.Audit(Constants.AuditingTitles.OrganDonationRegistrationAuditTypeRequest,
                    "Attempting to register organ donation decision");
                
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
    }
}