using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Areas.OrganDonation.Models;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    public class OrganDonationService : IOrganDonationService
    {
        private const string ReferenceDataCacheKey = "_organDonationReferenceData";
        private readonly ILogger<OrganDonationService> _logger;
        private readonly IMapper<DemographicsResponse, OrganDonationRegistration> _demographicsRegistrationMapper;
        private readonly IMapper<OrganDonationRegistration, LookupRegistrationRequest> _lookupRegistrationRequestMapper;

        private readonly IMapper<OrganDonationRegistration, OrganDonationSuccessResponse<RegistrationLookupResponse>,
            OrganDonationRegistration> _registrationMapper;

        private readonly IMapper<OrganDonationResponse<ReferenceDataResponse>, OrganDonationReferenceDataResponse>
            _organDonationReferenceDataMapper;

        private readonly IOrganDonationClient _organDonationClient;
        private readonly IMemoryCache _memoryCache;
        private readonly IOrganDonationConfig _config;

        public OrganDonationService(
            ILoggerFactory loggerFactory,
            IMapper<DemographicsResponse, OrganDonationRegistration> demographicsRegistrationMapper,
            IMapper<OrganDonationRegistration, OrganDonationSuccessResponse<RegistrationLookupResponse>,
                OrganDonationRegistration> registrationMapper,
            IMapper<OrganDonationRegistration, LookupRegistrationRequest> lookupRegistrationRequestMapper,
            IMapper<OrganDonationResponse<ReferenceDataResponse>, OrganDonationReferenceDataResponse>
                organDonationReferenceDataMapper,
            IOrganDonationClient organDonationClient,
            IMemoryCache memoryCache,
            IOrganDonationConfig config)
        {
            _logger = loggerFactory.CreateLogger<OrganDonationService>();
            _demographicsRegistrationMapper = demographicsRegistrationMapper;
            _lookupRegistrationRequestMapper = lookupRegistrationRequestMapper;
            _registrationMapper = registrationMapper;
            _organDonationClient = organDonationClient;
            _organDonationReferenceDataMapper = organDonationReferenceDataMapper;
            _memoryCache = memoryCache;
            _config = config;
        }

        public async Task<OrganDonationResult> GetOrganDonation(DemographicsResult myRecord, UserSession userSession)
        {
            if (!(myRecord is DemographicsResult.SuccessfullyRetrieved demographicsResult))
            {
                _logger.LogDebug("GP systems demographics record not successfully retrieved");
                return new OrganDonationResult.DemographicsRetrievalFailed();
            }

            var response = _demographicsRegistrationMapper.Map(demographicsResult.Response);
            var lookupRegistrationRequest = _lookupRegistrationRequestMapper.Map(response);

            try
            {
                _logger.LogDebug("Fetching existing organ donation record");
                var existingRegistrationRecord =
                    await _organDonationClient.PostLookup(lookupRegistrationRequest, userSession);

                if (existingRegistrationRecord.HasSuccessResponse)
                {
                    _logger.LogDebug("Found existing record");
                    response = _registrationMapper.Map(response, existingRegistrationRecord.Body);

                    return new OrganDonationResult.ExistingRegistration(response);
                }
                else
                {
                    switch (existingRegistrationRecord.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                            _logger.LogDebug("Could not find an existing record");
                            break;
                        case HttpStatusCode.Conflict:
                            _logger.LogDebug("Conflict, there is more than one record");
                            return new OrganDonationResult.DuplicateRecord();
                        case HttpStatusCode.BadRequest:
                            _logger.LogDebug(
                                $"The organ donation request is invalid with message {JsonConvert.SerializeObject(lookupRegistrationRequest)}");
                            return new OrganDonationResult.BadSearchRequest();
                        case HttpStatusCode.RequestTimeout:
                            _logger.LogDebug("The organ donation search timed-out");
                            return new OrganDonationResult.SearchTimeout();
                        default:
                            _logger.LogDebug("Something went wrong when retrieving organ donation record");
                            return new OrganDonationResult.SearchError();
                    }
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request to retrieve existing organ donation record");
                return new OrganDonationResult.SearchSystemUnavailable();
            }

            return new OrganDonationResult.NewRegistration(response);
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
                entry.AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(_config.ReferenceDataExpiryHours);
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