using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Worker.OrganDonation.ApiModels;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    internal class OrganDonationReferenceDataService 
    {
        
        private const string ReferenceDataCacheKey = "_organDonationReferenceData";
        private readonly ILogger<OrganDonationReferenceDataService> _logger;

        private readonly IMapper<OrganDonationResponse<ReferenceDataResponse>, OrganDonationReferenceDataResponse>
            _organDonationReferenceDataMapper;

        private readonly IOrganDonationClient _organDonationClient;
        private readonly IMemoryCache _memoryCache;
        private readonly IOrganDonationConfig _config;

        public OrganDonationReferenceDataService(
            ILogger<OrganDonationReferenceDataService> logger,
            IMapper<OrganDonationResponse<ReferenceDataResponse>, OrganDonationReferenceDataResponse>
                organDonationReferenceDataMapper,
            IOrganDonationClient organDonationClient,
            IMemoryCache memoryCache,
            IOrganDonationConfig config)
        {
            _logger = logger;
            _organDonationClient = organDonationClient;
            _organDonationReferenceDataMapper = organDonationReferenceDataMapper;
            _memoryCache = memoryCache;
            _config = config;
        }

        public async Task<OrganDonationReferenceDataResult> GetReferenceData()
        {
            try
            {
                _logger.LogDebug("Attempting to retrieve organ donation reference data from cache");
                return await _memoryCache.GetOrCreateAsync(ReferenceDataCacheKey, GetAllReferenceData);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request to retrieve organ donation reference data");
                return new OrganDonationReferenceDataResult.SystemError();
            }
        }


        private async Task<OrganDonationReferenceDataResult> GetAllReferenceData(ICacheEntry entry)
        {
            _logger.LogDebug("Cache miss, fetching organ donation reference data");
            var referenceData = await _organDonationClient.GetAllReferenceData();

            if (referenceData.HasSuccessResponse)
            {
                _logger.LogDebug("Successfully retrieved organ donation reference data");
                entry.AbsoluteExpiration = referenceData.Expires ?? DateTimeOffset.UtcNow.AddHours(_config.ReferenceDataExpiryHours);
                var response = _organDonationReferenceDataMapper.Map(referenceData);
                return new OrganDonationReferenceDataResult.SuccessfullyRetrieved(response);
            }

            // do not persist it
            entry.AbsoluteExpiration = DateTimeOffset.UtcNow;

            switch (referenceData.StatusCode)
            {
                case HttpStatusCode.RequestTimeout:
                    _logger.LogDebug("The organ donation reference data retrieval timed-out");
                    return new OrganDonationReferenceDataResult.Timeout();
                default:
                    _logger.LogDebug("Something went wrong when retrieving organ donation reference data");
                    return new OrganDonationReferenceDataResult.UpstreamError();
            }
        }
    }
}