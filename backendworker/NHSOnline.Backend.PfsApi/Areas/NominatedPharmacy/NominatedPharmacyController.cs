using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.NominatedPharmacy;
using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Auditing;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.PfsApi.GpSearch.Pharmacy;
using System;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using System.Linq;

namespace NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy
{
    [Route("patient")]
    public class NominatedPharmacyController : Controller
    {
        private readonly ILogger<NominatedPharmacyController> _logger;
        private readonly INominatedPharmacyService _nominatedPharmacyService;
        private readonly IPharmacyService _pharmacyService;
        private readonly IPharmacyDetailsToPharmacyDetailsResponseMapper _pharmacyDetailsToPharmacyDetailsResponseMapper;
        private readonly IPharmacySearchService _pharmacySearchService;
        private readonly INominatedPharmacyGatewayUpdateService _nominatedPharmacyGatewayUpdateService;
        private readonly IAuditor _auditor;
        private readonly INominatedPharmacyConfig _config;

        public NominatedPharmacyController(
            ILogger<NominatedPharmacyController> logger,
            INominatedPharmacyService nominatedPharmacyService,
            IPharmacyService pharmacyService,
            IPharmacyDetailsToPharmacyDetailsResponseMapper pharmacyDetailsToPharmacyDetailsResponseMapper,
            IPharmacySearchService pharmacySearchService,
            INominatedPharmacyGatewayUpdateService nominatedPharmacyGatewayUpdateService,
            IAuditor auditor,
            INominatedPharmacyConfig config
           )
        {
            _logger = logger;
            _nominatedPharmacyService = nominatedPharmacyService;
            _pharmacyService = pharmacyService;
            _pharmacyDetailsToPharmacyDetailsResponseMapper = pharmacyDetailsToPharmacyDetailsResponseMapper;
            _pharmacySearchService = pharmacySearchService;
            _nominatedPharmacyGatewayUpdateService = nominatedPharmacyGatewayUpdateService;
            _auditor = auditor;
            _config = config;
        }

        [HttpGet("nominated-pharmacy")]
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

                _logger.LogInformation($"Nominated pharmacy retrieved with ods code: { result.PharmacyOdsCode }");

                var pharmacyDetailResponse = await _pharmacyService.GetPharmacyDetail(result.PharmacyOdsCode);

                if (HttpStatusCodeExtensions.IsSuccessStatusCode(pharmacyDetailResponse.StatusCode))
                {
                    var pharmacyDetails = _pharmacyDetailsToPharmacyDetailsResponseMapper.Map(pharmacyDetailResponse.Pharmacy);
                    pharmacyDetails.PharmacyType = result.NominatedPharmacyType;
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

        [HttpPost("nominated-pharmacy")]
        public async Task<IActionResult> Update([FromBody] UpdateNominatedPharmacyRequest model)
        {
            _logger.LogEnter();
            UserSession userSession = HttpContext.GetUserSession();

            try
            {
                var result = await _nominatedPharmacyGatewayUpdateService.UpdateNominatedPharmacy(userSession.GpUserSession.NhsNumber, model.OdsCode);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while trying to update the patient's nominated pharmacy");
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpGet]
        [Route("pharmacies")]
        public async Task<IActionResult> Search([FromQuery] string postcode)
        {
            try
            {
                _logger.LogEnter();
                _logger.LogInformation($"Fetching Pharmacies for {postcode}");

                var pharmacySearchResponse = await _pharmacySearchService.Search(postcode);

                var pharmacies = Enumerable.Empty<PharmacyDetailsResponse>();

                if (HttpStatusCodeExtensions.IsSuccessStatusCode(pharmacySearchResponse.StatusCode))
                {
                    try
                    {
                        pharmacies = _pharmacyDetailsToPharmacyDetailsResponseMapper.Map(
                            pharmacySearchResponse.Pharmacies,
                            pharmacySearchResponse.PostcodeCoordinate);
                    }
                    catch(Exception e)
                    {
                        _logger.LogError(e, $"Error during mapping list of { nameof(Organisation) } to list of {nameof(PharmacyDetailsResponse)}");
                    }
                }

                await _auditor.Audit(Constants.AuditingTitles.SearchNominatedPharmacyAuditTypeResponse, $"Returning { pharmacies.Count() } pharmacies");

                return Ok(pharmacies);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
