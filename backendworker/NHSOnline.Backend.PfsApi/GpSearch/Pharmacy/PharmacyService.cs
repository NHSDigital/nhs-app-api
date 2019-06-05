using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.PfsApi.GpSearch.Models.Pharmacy;

namespace NHSOnline.Backend.PfsApi.GpSearch.Pharmacy
{
    public class PharmacyService : IPharmacyService
    {
        private readonly ILogger<PharmacyService> _logger;
        private readonly IGpLookupClient _gpLookupClient;

        public PharmacyService(
            ILogger<PharmacyService> logger,
            IGpLookupClient gpLookupClient
           )
        {
            _logger = logger;
            _gpLookupClient = gpLookupClient;
        }

        public async Task<PharmacyDetailResponse> GetPharmacyDetail(string odsCode)
        {
            try
            {
                _logger.LogEnter();
                var isValid = new ValidateAndLog(_logger)
                    .IsNotNullOrWhitespace(odsCode, nameof(odsCode))
                    .IsValid();

                if (!isValid)
                {
                    _logger.LogError("OdsCode provided must not be null or white space");
                    throw new ArgumentException("Argument must not be null or white space");
                }

                try
                {
                    var pharmacyGetResult = await _gpLookupClient.PharmacySearch(GetPharmacySearchData(odsCode));

                    if (!pharmacyGetResult.HasSuccessResponse)
                    {
                        _logger.LogError(
                            $"Unsuccessful request to get pharmacy detail for odsCode {odsCode} using Nhs Search Service, Status code: " +
                            $"{(int) pharmacyGetResult.StatusCode}");
                        return new PharmacyDetailResponse(pharmacyGetResult.StatusCode);
                    }

                    var pharmacy = pharmacyGetResult?.Body?.Organisations?.FirstOrDefault();
                    
                    if (pharmacy == null)
                    {
                        _logger.LogInformation($"NHS Search service returned no pharmacy detail for: {odsCode}");
                        return new PharmacyDetailResponse(HttpStatusCode.NotFound);
                    }
                    return new PharmacyDetailResponse(pharmacyGetResult.StatusCode, pharmacy);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, $"Search for Nhs GP Practice Failed for: {odsCode} ");
                    return new PharmacyDetailResponse(HttpStatusCode.ServiceUnavailable);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing odsCode: {odsCode} ");
                return new PharmacyDetailResponse(HttpStatusCode.InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private static OrganisationSearchData GetPharmacySearchData(string odsCode)
        {
            return new OrganisationSearchData
            {
                Top = 1,
                Search = "*",
                Filter = $"NACSCode eq '{odsCode}'",
                Count = true,
            };
        }
    }
}