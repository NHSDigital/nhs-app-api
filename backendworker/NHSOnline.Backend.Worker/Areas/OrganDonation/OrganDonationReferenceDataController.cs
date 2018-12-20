using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Conventions;
using NHSOnline.Backend.Worker.OrganDonation;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.Areas.OrganDonation
{
    [Route("patient/organdonation/referencedata"), PfsSecurityMode]
    public class OrganDonationReferenceDataController : Controller
    {
        
        private readonly ILogger<OrganDonationReferenceDataController> _logger;
        private readonly IAuditor _auditor;
        private readonly IOrganDonationService _organDonationService;

        public OrganDonationReferenceDataController(
            ILogger<OrganDonationReferenceDataController> logger,
            IOrganDonationService organDonationService,
            IAuditor auditor)
        {
            _logger = logger;
            _auditor = auditor;
            _organDonationService = organDonationService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogEnter();

                await _auditor.Audit(Constants.AuditingTitles.GetOrganDonationReferenceDataAuditTypeRequest,
                    "Attempting to get organ donation reference data");

                _logger.LogDebug("Fetching organ donation reference data");
                var result = await _organDonationService.GetReferenceData();

                result.Accept(new OrganDonationReferenceDataAuditingVisitor(_auditor));

                return result.Accept(new OrganDonationReferenceDataResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}