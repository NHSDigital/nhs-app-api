using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ApiSupport;
using NHSOnline.Backend.NominatedPharmacy;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Auditing;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy;
using NHSOnline.Backend.PfsApi.GpSearch.Pharmacy;

namespace NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy
{
    [Route("patient/pharmacy/nominated"), PfsSecurityMode]
    public class NominatedPharmacyController : Controller
    {
        private readonly ILogger<NominatedPharmacyController> _logger;
        private readonly INominatedPharmacyService _nominatedPharmacyService;
        private readonly IPharmacyService _pharmacyService;
        private readonly IPharmacyDetailsToPharmacyDetailsResponseMapper _pharmacyDetailsToPharmacyDetailsResponseMapper;
        private readonly IAuditor _auditor;
        
        public NominatedPharmacyController(
            ILogger<NominatedPharmacyController> logger,
            INominatedPharmacyService nominatedPharmacyService,
            IPharmacyService pharmacyService,
            IPharmacyDetailsToPharmacyDetailsResponseMapper pharmacyDetailsToPharmacyDetailsResponseMapper,
            IAuditor auditor)
        {
            _logger = logger;
            _nominatedPharmacyService = nominatedPharmacyService;
            _pharmacyService = pharmacyService;
            _pharmacyDetailsToPharmacyDetailsResponseMapper = pharmacyDetailsToPharmacyDetailsResponseMapper;
            _auditor = auditor;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            _logger.LogEnter();
            UserSession userSession = HttpContext.GetUserSession();

            var result = await _nominatedPharmacyService.GetNominatedPharmacy(userSession.GpUserSession.NhsNumber);

            if (HttpStatusCodeExtensions.IsSuccessStatusCode(result.HttpStatusCode))
            {
                if (string.IsNullOrEmpty(result.PharmacyOdsCode))
                {
                    await _auditor.Audit(Constants.AuditingTitles.GetNominatedPharmacy, "Patient does not have a nominated pharmacy set");
                    _logger.LogInformation($"No nominated pharmacy. Returning Success.");
                    return new OkResult();
                }

                _logger.LogInformation($"Nominated pharmacy retrieved. Requesting pharmacy ods code: { result.PharmacyOdsCode }");

                var pharmacyDetailResponse = await _pharmacyService.GetPharmacyDetail(result.PharmacyOdsCode);

                if (HttpStatusCodeExtensions.IsSuccessStatusCode(pharmacyDetailResponse.StatusCode))
                {
                    var pharmacyDetails = _pharmacyDetailsToPharmacyDetailsResponseMapper.Map(pharmacyDetailResponse.Pharmacy);
                    await _auditor.Audit(Constants.AuditingTitles.GetNominatedPharmacy, "Successfully retrieved nominated pharmacy");
                    return new OkObjectResult(pharmacyDetails);
                }
                else
                {
                    _logger.LogInformation($"Error retrieving pharmacy using pharmacy ods code: { result.PharmacyOdsCode }, HttpStatusCode: { result.HttpStatusCode }");
                }
            }
            else
            {
                _logger.LogInformation($"Error retrieving nominated pharmacy from Spine - HttpStatusCode { result.HttpStatusCode }");
            }

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
