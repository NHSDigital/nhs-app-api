using System.Net;
using System.Threading.Tasks;
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
using NHSOnline.Backend.PfsApi.GpSearch;

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
        private readonly INominatedPharmacyConfigurationSettings _config;
        private readonly IGpSearchService _gpSearchService;

        public NominatedPharmacyController(
            ILogger<NominatedPharmacyController> logger,
            INominatedPharmacyService nominatedPharmacyService,
            IPharmacyService pharmacyService,
            IPharmacyDetailsToPharmacyDetailsResponseMapper pharmacyDetailsToPharmacyDetailsResponseMapper,
            IPharmacySearchService pharmacySearchService,
            INominatedPharmacyGatewayUpdateService nominatedPharmacyGatewayUpdateService,
            IAuditor auditor,
            INominatedPharmacyConfigurationSettings config,
            IGpSearchService gpSearchService
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
            _gpSearchService = gpSearchService;
        }

        [HttpGet("nominated-pharmacy")]
        public async Task<IActionResult> Get()
        {
            _logger.LogEnter();

            if (!_config.IsNominatedPharmacyEnabled)
            {
                _logger.LogInformation("Nominated pharmacy feature is disabled");
                return new OkObjectResult(new PharmacyDetailsResponse { NominatedPharmacyEnabled = false });
            }

            UserSession userSession = HttpContext.GetUserSession();
            
            var isGpPracticeEpsEnabledResult = await _gpSearchService.IsGpPracticeEPSEnabled(userSession.GpUserSession.OdsCode);

            if (!HttpStatusCodeExtensions.IsSuccessStatusCode(isGpPracticeEpsEnabledResult.HttpStatusCode))
            {
                return new StatusCodeResult(
                    GetErrorStatusCode(
                        $"Error retrieving GP practice with ods code { userSession.GpUserSession.OdsCode }",
                        isGpPracticeEpsEnabledResult.HttpStatusCode));
            }

            if (!isGpPracticeEpsEnabledResult.IsGpEpsEnabled)
            {
                _logger.LogInformation($"GP practice with ods code { userSession.GpUserSession.OdsCode } not enabled for electronic prescription service");
                return new OkObjectResult(new PharmacyDetailsResponse { NominatedPharmacyEnabled = false });
            }

            var result = await _nominatedPharmacyService.GetNominatedPharmacy(userSession.GpUserSession.NhsNumber);

            if (!HttpStatusCodeExtensions.IsSuccessStatusCode(result.HttpStatusCode))
            {
                return new StatusCodeResult(GetErrorStatusCode("Error retrieving nominated pharmacy ods code from Spine with status code",
                    result.HttpStatusCode));
            }
            if (!result.HasValidPharmacyType)
            {
                _logger.LogInformation("Invalid nominated pharmacy type or multiple pharmacy types exist");
                return new OkObjectResult(new PharmacyDetailsResponse { NominatedPharmacyEnabled = result.HasValidPharmacyType });
            }

            if (string.IsNullOrEmpty(result.PharmacyOdsCode))
            {
                _logger.LogInformation("No nominated pharmacy. Returning Success.");
                return new OkObjectResult(new PharmacyDetailsResponse { NominatedPharmacyEnabled = result.HasValidPharmacyType });
            }

            _logger.LogInformation($"Nominated pharmacy retrieved with ods code: { result.PharmacyOdsCode }");

            var pharmacyDetailResponse = await _pharmacyService.GetPharmacyDetail(result.PharmacyOdsCode);

            if (!HttpStatusCodeExtensions.IsSuccessStatusCode(pharmacyDetailResponse.StatusCode))
            {
                return new StatusCodeResult(GetErrorStatusCode("Error retrieving pharmacy using pharmacy ods code with status code",
                    result.HttpStatusCode));
            }

            var pharmacyDetails = _pharmacyDetailsToPharmacyDetailsResponseMapper.Map(pharmacyDetailResponse.Pharmacy);
            pharmacyDetails.PharmacyType = result.NominatedPharmacyType;
            await _auditor.Audit(Support.Constants.AuditingTitles.GetNominatedPharmacy, "Successfully retrieved nominated pharmacy");

            return new OkObjectResult(
                new PharmacyDetailsResponse
                {
                    PharmacyDetails = pharmacyDetails,
                    NominatedPharmacyEnabled = result.HasValidPharmacyType
                });
        }

        [HttpPost("nominated-pharmacy")]
        public async Task<IActionResult> Update([FromBody] UpdateNominatedPharmacyRequest model)
        {
            _logger.LogEnter();

            if (!_config.IsNominatedPharmacyEnabled)
            {
                _logger.LogInformation("Nominated pharmacy feature is disabled");
                return new StatusCodeResult((int)HttpStatusCode.Forbidden);
            }

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
        public async Task<IActionResult> Search([FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogEnter();
                _logger.LogInformation($"Fetching Pharmacies for {searchTerm}");

                var pharmacySearchResponse = await _pharmacySearchService.Search(searchTerm);
                
                if (!HttpStatusCodeExtensions.IsSuccessStatusCode(pharmacySearchResponse.StatusCode))
                {
                    return new StatusCodeResult(
                        GetErrorStatusCode("Error searching for pharmacies", pharmacySearchResponse.StatusCode));
                }

                try
                {
                    var pharmacies = Enumerable.Empty<PharmacyDetails>();

                    pharmacies = _pharmacyDetailsToPharmacyDetailsResponseMapper.Map(
                        pharmacySearchResponse.Pharmacies,
                        pharmacySearchResponse.PostcodeCoordinate);

                    await _auditor.Audit(Support.Constants.AuditingTitles.SearchNominatedPharmacyAuditTypeResponse, $"Returning { pharmacies.Count() } pharmacies");

                    return Ok(pharmacies);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error during mapping list of { nameof(Organisation) } to list of {nameof(PharmacyDetailsResponse)}");
                    return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private int GetErrorStatusCode(string errorMessage, HttpStatusCode statusCode)
        {
            if (statusCode == HttpStatusCode.InternalServerError)
            {
                _logger.LogError($"{errorMessage} returning: {(int)HttpStatusCode.InternalServerError}");
            }
            else
            {
                statusCode = HttpStatusCode.BadGateway;
                _logger.LogError($"{errorMessage} returning: {(int)HttpStatusCode.BadGateway}");
            }
            return (int)statusCode;
        }
    }
}
