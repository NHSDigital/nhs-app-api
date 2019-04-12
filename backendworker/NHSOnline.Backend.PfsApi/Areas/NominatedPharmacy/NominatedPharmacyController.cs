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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NHSOnline.Backend.ApiSupport;

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
        private readonly IAuditor _auditor;

        public NominatedPharmacyController(
            ILogger<NominatedPharmacyController> logger,
            INominatedPharmacyService nominatedPharmacyService,
            IPharmacyService pharmacyService,
            IPharmacyDetailsToPharmacyDetailsResponseMapper pharmacyDetailsToPharmacyDetailsResponseMapper,
            IPharmacySearchService pharmacySearchService,
            IAuditor auditor
           )
        {
            _logger = logger;
            _nominatedPharmacyService = nominatedPharmacyService;
            _pharmacyService = pharmacyService;
            _pharmacyDetailsToPharmacyDetailsResponseMapper = pharmacyDetailsToPharmacyDetailsResponseMapper;
            _pharmacySearchService = pharmacySearchService;
            _auditor = auditor;
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

            var result = await _nominatedPharmacyService.GetNominatedPharmacy(userSession.GpUserSession.NhsNumber);

            if (HttpStatusCodeExtensions.IsSuccessStatusCode(result.HttpStatusCode))
            {
                if (string.IsNullOrEmpty(result.PertinentSerialChangeNumber))
                {
                    _logger.LogError("Missing pertinentSerialChangeNumber which is required for the update request");
                    return new StatusCodeResult((int)HttpStatusCode.BadGateway);
                }
                
                _logger.LogInformation($"Nominated pharmacy retrieved. Updating nominated pharmacy to: {model.OdsCode}");            

                var updateNominatedPharmacyResult = await _nominatedPharmacyService
                    .UpdateNominatedPharmacy(userSession.GpUserSession.NhsNumber,
                        model.OdsCode, result.PertinentSerialChangeNumber);

                if (HttpStatusCodeExtensions.IsSuccessStatusCode(updateNominatedPharmacyResult.HttpStatusCode))
                {
                    _logger.LogInformation($"Successfully updated nominated pharmacy to {result.PharmacyOdsCode}");
                    await _auditor.Audit(Constants.AuditingTitles.UpdatedNominatedPharmacy, "Successfully updated nominated pharmacy");
                    return Ok();

                }
                else
                {
                    _logger.LogInformation($"Error updating nominated pharmacy{ result.PharmacyOdsCode }");
                    await _auditor.Audit(Constants.AuditingTitles.UpdatedNominatedPharmacy, "Failed to update nominated pharmacy");
                    return new StatusCodeResult((int)updateNominatedPharmacyResult.HttpStatusCode);
                }
            }
            else
            {
                _logger.LogInformation($"Error retrieving nominated pharmacy from Spine - HttpStatusCode { result.HttpStatusCode }");
            }

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
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
