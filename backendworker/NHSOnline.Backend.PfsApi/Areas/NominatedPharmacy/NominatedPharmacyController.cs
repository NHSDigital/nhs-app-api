using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.NominatedPharmacy;
using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.PfsApi.GpSearch.Pharmacy;
using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.NominatedPharmacy.Models;
using NHSOnline.Backend.PfsApi.Filters;
using NHSOnline.Backend.PfsApi.GpSearch;
using NHSOnline.Backend.PfsApi.GpSearch.Models.Pharmacy;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy
{
    [JourneyFeatureFilterAttribute(JourneyFeature.NominatedPharmacy)]
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

            await _auditor.Audit(AuditingOperations.GetNominatedPharmacyRequest, "Attempting to get Nominated Pharmacy");

            var getNominatedPharmacyResult = await GetNominatedPharmacy(); 
            
            _logger.LogExit();

            await getNominatedPharmacyResult.Accept(new GetNominatedPharmacyResultAuditingVisitor(_auditor, _logger));
            return getNominatedPharmacyResult.Accept(new GetNominatedPharmacyResultVisitor());
        }

        [HttpPost("nominated-pharmacy")]
        public async Task<IActionResult> Update([FromBody] UpdateNominatedPharmacyRequest model)
        {
            _logger.LogEnter();

            await _auditor.Audit(AuditingOperations.UpdatedNominatedPharmacyRequest,
                $"Attempting to update Nominated Pharmacy for OdsCode { model.OdsCode }");

            UserSession userSession = HttpContext.GetUserSession();

            var result = await UpdateNominatedPharmacy(model, userSession);
            
            _logger.LogExit();
            
            await result.Accept(new UpdateNominatedPharmacyResponseAuditingVisitor(_auditor, _logger));
            return result.Accept(new UpdateNominatedPharmacyResponseVisitor());
        }

        [HttpGet]
        [Route("pharmacies")]
        public async Task<IActionResult> Search([FromQuery] string searchTerm)
        {
            _logger.LogEnter();
            await _auditor.Audit(AuditingOperations.SearchNominatedPharmacyAuditTypeRequest, 
                $"Attempting to search for nominated pharmacy with search term { searchTerm } ");

            var pharmacySearchResult = await SearchForNominatedPharmacy(ModelState, searchTerm);

            _logger.LogExit();

            await pharmacySearchResult.Accept(new PharmacySearchResponseAuditingVisitor(_auditor, _logger));
            return pharmacySearchResult.Accept(new PharmacySearchResponseVisitor());
        }
        
        private async Task<PharmacySearchResult> SearchForNominatedPharmacy( ModelStateDictionary modelState, string searchTerm)
        {
            if (!modelState.IsValid)
            {
                return new PharmacySearchResult.ModelValidationError();
            }
                
            _logger.LogInformation($"Fetching Pharmacies for {searchTerm}");
            return await _pharmacySearchService.Search(searchTerm);
        }
        
        
        private async Task<UpdateNominatedPharmacyResponse> UpdateNominatedPharmacy(UpdateNominatedPharmacyRequest model, UserSession userSession)
        {
            if (!_config.IsNominatedPharmacyEnabled)
            {
                _logger.LogInformation("Nominated pharmacy feature is disabled");
                return new UpdateNominatedPharmacyResponse.NominatedPharmacyIsDisabled();
            }
            
            if (!ModelState.IsValid)
            {
                _logger.LogError("Update nominated pharmacy : bad request");
                return new UpdateNominatedPharmacyResponse.BadRequest();
            }

            try
            {
                return await _nominatedPharmacyGatewayUpdateService.UpdateNominatedPharmacy(
                    userSession.GpUserSession.NhsNumber, model.OdsCode, userSession.CitizenIdUserSession);
            }
            catch (Exception ex)
            {               
                _logger.LogError(ex, $"An error occurred while trying to update the patient's nominated pharmacy");
                return new UpdateNominatedPharmacyResponse.InternalServerError(HttpStatusCode.InternalServerError);
            }  
        }

        private async Task<GetNominatedPharmacyResult> GetNominatedPharmacy()
        {
            if (!_config.IsNominatedPharmacyEnabled)
            {
                return new GetNominatedPharmacyResult.ConfigNotEnabled();
            }

            UserSession userSession = HttpContext.GetUserSession();

            var isGpPracticeEpsEnabledResult =
                await _gpSearchService.IsGpPracticeEPSEnabled(userSession.GpUserSession.OdsCode);

            if (!HttpStatusCodeExtensions.IsSuccessStatusCode(isGpPracticeEpsEnabledResult.HttpStatusCode))
            {
                return new GetNominatedPharmacyResult.GpPracticeFailure(
                    userSession.GpUserSession.OdsCode, isGpPracticeEpsEnabledResult.HttpStatusCode);
            }

            if (!isGpPracticeEpsEnabledResult.IsGpEpsEnabled)
            {
                return new GetNominatedPharmacyResult.GpPracticeEpsNotEnabled(userSession.GpUserSession.OdsCode);
            }

            var result = await _nominatedPharmacyService.GetNominatedPharmacy(
                userSession.GpUserSession.NhsNumber, userSession.CitizenIdUserSession);

            if (!result.IsSuccess())
            {
                return result;
            }

            if (string.IsNullOrEmpty(result.GetNominatedPharmacyResponse.PharmacyOdsCode))
            {
                return new GetNominatedPharmacyResult.NoNominatedPharmacy();
            }

            _logger.LogInformation(
                $"Nominated pharmacy retrieved with ods code: {result.GetNominatedPharmacyResponse.PharmacyOdsCode}");

            var pharmacyDetailResponse =
                await _pharmacyService.GetPharmacyDetail(result.GetNominatedPharmacyResponse.PharmacyOdsCode);

            if (!HttpStatusCodeExtensions.IsSuccessStatusCode(pharmacyDetailResponse.StatusCode))
            {
                return new GetNominatedPharmacyResult.PharmacyDetailFailure(
                    result.GetNominatedPharmacyResponse.PharmacyOdsCode, pharmacyDetailResponse.StatusCode);
            }

            if (!_pharmacyService.IsValidPharmacySubType(pharmacyDetailResponse))
            {
                return new GetNominatedPharmacyResult.InvalidPharmacySubtype();
            }

            if (result is GetNominatedPharmacyResult.Success success)
            {
                var pharmacyDetails = _pharmacyDetailsToPharmacyDetailsResponseMapper.Map(pharmacyDetailResponse.Pharmacy);
                pharmacyDetails.PharmacyType = result.GetNominatedPharmacyResponse.NominatedPharmacyType;

                success.PharmacyDetails = pharmacyDetails;
                return success;
            }

            return result;
        }
    }
}
