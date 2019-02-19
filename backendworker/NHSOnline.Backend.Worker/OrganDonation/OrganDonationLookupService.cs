using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.Worker.OrganDonation.ApiModels;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    internal class OrganDonationLookupService
    {
        private readonly ILogger<OrganDonationLookupService> _logger;
        private readonly IOrganDonationClient _organDonationClient;

        private readonly IMapper<DemographicsResponse, OrganDonationRegistration> _demographicsRegistrationMapper;
        private readonly IMapper<OrganDonationRegistration, RegistrationLookupRequest> _lookupRegistrationRequestMapper;
        private readonly IMapper<OrganDonationRegistration, RegistrationLookupResponse, OrganDonationRegistration>
            _registrationMapper;

        public OrganDonationLookupService(
            ILogger<OrganDonationLookupService> logger,
            IMapper<DemographicsResponse, OrganDonationRegistration> demographicsRegistrationMapper,
            IMapper<OrganDonationRegistration, RegistrationLookupResponse, OrganDonationRegistration>
                registrationMapper,
            IMapper<OrganDonationRegistration, RegistrationLookupRequest> lookupRegistrationRequestMapper,
            IOrganDonationClient organDonationClient)
        {
            _logger = logger;
            _demographicsRegistrationMapper = demographicsRegistrationMapper;
            _lookupRegistrationRequestMapper = lookupRegistrationRequestMapper;
            _registrationMapper = registrationMapper;
            _organDonationClient = organDonationClient;
        }

        public async Task<OrganDonationResult> GetOrganDonation(DemographicsResult myRecord, UserSession userSession)
        {
            if (!(myRecord is DemographicsResult.SuccessfullyRetrieved demographicsResult))
            {
                return GetDemographicsErrorResult(myRecord);
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

                switch (existingRegistrationRecord.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        _logger.LogDebug("Could not find an existing record");
                        break;
                    default:
                        return GetExistingRecordErrorResult(existingRegistrationRecord, response);
                }

            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request to retrieve existing organ donation record");
                return new OrganDonationResult.SearchSystemUnavailable();
            }

            return new OrganDonationResult.NewRegistration(response);
        }

        private OrganDonationResult GetExistingRecordErrorResult(
            OrganDonationResponse<RegistrationLookupResponse> existingRegistrationRecord,
            OrganDonationRegistration response)
        {
            switch (existingRegistrationRecord.StatusCode)
            {
                case HttpStatusCode.Conflict:
                    _logger.LogDebug($"Registration conflicted. {existingRegistrationRecord.ErrorForLogging}");
                    response.State = State.Conflicted;
                    return new OrganDonationResult.ExistingRegistration(response);
                case HttpStatusCode.BadRequest:
                    _logger.LogDebug("The organ donation request is invalid");
                    return new OrganDonationResult.BadSearchRequest();
                case HttpStatusCode.RequestTimeout:
                    _logger.LogDebug("The organ donation search timed-out");
                    return new OrganDonationResult.SearchTimeout();
                default:
                    _logger.LogDebug("Something went wrong when retrieving organ donation record");
                    return new OrganDonationResult.SearchError();
            }
        }

        private OrganDonationResult GetDemographicsErrorResult(DemographicsResult myRecord)
        {

            switch (myRecord)
            {
                case DemographicsResult.Unsuccessful _:
                    _logger.LogDebug("GP systems demographics call was unsuccessful");
                    return new OrganDonationResult.DemographicsBadGateway();
                case DemographicsResult.UserHasNoAccess _:
                    _logger.LogDebug("GP systems demographics user has no access");
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