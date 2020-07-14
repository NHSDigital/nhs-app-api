using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    internal class OrganDonationLookupService
    {
        private readonly ILogger<OrganDonationLookupService> _logger;
        private readonly IOrganDonationClient _organDonationClient;

        private readonly IMapper<DemographicsResponse, P9UserSession, OrganDonationRegistration>
            _demographicsRegistrationMapper;
        private readonly IMapper<OrganDonationRegistration, RegistrationLookupRequest> _lookupRegistrationRequestMapper;
        private readonly IMapper<OrganDonationRegistration, RegistrationLookupResponse, OrganDonationRegistration>
            _registrationMapper;
        private readonly IMapper<HttpStatusCode, OrganDonationResult> _organDonationResultErrorMapper;

        public OrganDonationLookupService(
            ILogger<OrganDonationLookupService> logger,
            IMapper<DemographicsResponse, P9UserSession, OrganDonationRegistration> demographicsRegistrationMapper,
            IMapper<OrganDonationRegistration, RegistrationLookupResponse, OrganDonationRegistration>
                registrationMapper,
            IMapper<OrganDonationRegistration, RegistrationLookupRequest> lookupRegistrationRequestMapper,
            IMapper<HttpStatusCode, OrganDonationResult> organDonationResultErrorMapper,
            IOrganDonationClient organDonationClient)
        {
            _logger = logger;
            _demographicsRegistrationMapper = demographicsRegistrationMapper;
            _lookupRegistrationRequestMapper = lookupRegistrationRequestMapper;
            _registrationMapper = registrationMapper;
            _organDonationResultErrorMapper = organDonationResultErrorMapper;
            _organDonationClient = organDonationClient;
        }

        public async Task<OrganDonationResult> GetOrganDonation(DemographicsResult myRecord, P9UserSession userSession)
        {
            if (!(myRecord is DemographicsResult.Success demographicsResult))
            {
                return GetDemographicsErrorResult(myRecord);
            }

            var response = _demographicsRegistrationMapper
                .Map(demographicsResult.Response, userSession);
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

                switch (existingRegistrationRecord.StatusCode)
                {
                    case HttpStatusCode.Conflict:
                        _logger.LogDebug($"Registration conflicted. {existingRegistrationRecord.ErrorForLogging}");
                        response.State = State.Conflicted;
                        return new OrganDonationResult.ExistingRegistration(response);
                    case HttpStatusCode.NotFound:
                        return new OrganDonationResult.NewRegistration(response);
                    default:
                        return _organDonationResultErrorMapper.Map(existingRegistrationRecord.StatusCode);
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request to retrieve existing organ donation record");
                return new OrganDonationResult.SearchError();
            }
        }

        private OrganDonationResult GetDemographicsErrorResult(DemographicsResult myRecord)
        {
            switch (myRecord)
            {
                case DemographicsResult.BadGateway _:
                    _logger.LogDebug("GP systems demographics call was unsuccessful");
                    return new OrganDonationResult.DemographicsBadGateway();
                case DemographicsResult.Forbidden _:
                    _logger.LogDebug("GP systems demographics forbidden");
                    return new OrganDonationResult.DemographicsForbidden();
                case DemographicsResult.InternalServerError _:
                    _logger.LogDebug("GP systems demographics threw an internal server error");
                    return new OrganDonationResult.DemographicsInternalServerError();
                default:
                    _logger.LogDebug("GP systems demographics record not successfully retrieved");
                    return new OrganDonationResult.DemographicsRetrievalFailed();
            }
        }
    }
}